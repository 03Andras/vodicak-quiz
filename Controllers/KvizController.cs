using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using System.Text.Json;
using System.Collections.Generic;

[ApiController]
[Route("[controller]")]
public class KvizController : ControllerBase
{
    [HttpGet("start")]
    public IActionResult Start()
    {
        using var spojenie = Databaza.OtvorSpojenie();

        // random 10 kerdes lekeres
        var otazkyPrikaz = spojenie.CreateCommand();
        otazkyPrikaz.CommandText = @"
            SELECT id, text FROM Otazka
            ORDER BY RANDOM()
            LIMIT 10";

        var otazky = new List<object>();

        using var reader = otazkyPrikaz.ExecuteReader();
        while (reader.Read())
        {
            var otazkaId = reader.GetInt64(0);
            var text = reader.GetString(1);

            // valaszok
            var odpovedPrikaz = spojenie.CreateCommand();
            odpovedPrikaz.CommandText = @"
                SELECT id, text FROM Odpoved
                WHERE otazkaId = $id
                ORDER BY RANDOM()";
            odpovedPrikaz.Parameters.AddWithValue("$id", otazkaId);

            var odpovede = new List<object>();
            using var odpovedReader = odpovedPrikaz.ExecuteReader();
            while (odpovedReader.Read())
            {
                odpovede.Add(new
                {
                    id = odpovedReader.GetInt64(0),
                    text = odpovedReader.GetString(1)
                });
            }

            // kep lekerdezes
        var obrazokPrikaz = spojenie.CreateCommand();
        obrazokPrikaz.CommandText = "SELECT subor FROM Obrazok WHERE otazkaId = $id";
        obrazokPrikaz.Parameters.AddWithValue("$id", otazkaId);

        var obrazky = new List<string>();
        using var obrazokReader = obrazokPrikaz.ExecuteReader();
        while (obrazokReader.Read())
        {
            obrazky.Add(obrazokReader.GetString(0));
        }

        otazky.Add(new {
            id = otazkaId,
            text = text,
            odpovede = odpovede,
            obrazky = obrazky
        });

        }

        return Ok(otazky);
    }

    public class OdpovedInput
    {
        public long otazkaId { get; set; }
        public long? odpovedId { get; set; }
    }

    [HttpPost("odosli")]
    public IActionResult Odosli([FromBody] List<OdpovedInput> odpovede)
    {
        var sessionId = Request.Cookies["session"];
        if (string.IsNullOrEmpty(sessionId))
            return Unauthorized("Nie ste prihlásený.");

        using var spojenie = Databaza.OtvorSpojenie();

        var sessionPrikaz = spojenie.CreateCommand();
        sessionPrikaz.CommandText = "SELECT pouzivatelId FROM Session WHERE sessionId = $sessionId";
        sessionPrikaz.Parameters.AddWithValue("$sessionId", sessionId);
        var pouzivatelId = Convert.ToInt64(sessionPrikaz.ExecuteScalar());

        var kvizPrikaz = spojenie.CreateCommand();
        kvizPrikaz.CommandText = @"
        INSERT INTO Kviz (pouzivatelId) VALUES ($pid);
        SELECT last_insert_rowid();";
        kvizPrikaz.Parameters.AddWithValue("$pid", pouzivatelId);
        var kvizId = Convert.ToInt64(kvizPrikaz.ExecuteScalar());

        int bodov = 0;

        foreach (var odpoved in odpovede)
        {
            var spravnaPrikaz = spojenie.CreateCommand();
            spravnaPrikaz.CommandText = @"
            SELECT jeSpravna FROM Odpoved
            WHERE id = $id";
            spravnaPrikaz.Parameters.AddWithValue("$id", odpoved.odpovedId ?? -1);
            var jeSpravna = Convert.ToInt32(spravnaPrikaz.ExecuteScalar() ?? 0);

            if (jeSpravna == 1) bodov++;

            var insertPrikaz = spojenie.CreateCommand();
            insertPrikaz.CommandText = @"
            INSERT INTO KvizOdpoved (kvizId, otazkaId, odpovedId)
            VALUES ($kvizId, $otazkaId, $odpovedId)";
            insertPrikaz.Parameters.AddWithValue("$kvizId", kvizId);
            insertPrikaz.Parameters.AddWithValue("$otazkaId", odpoved.otazkaId);
            insertPrikaz.Parameters.AddWithValue("$odpovedId", odpoved.odpovedId.HasValue ? odpoved.odpovedId : DBNull.Value);
            insertPrikaz.ExecuteNonQuery();
        }

        return Ok(new { bodov = bodov, max = odpovede.Count });
    }
[HttpGet("moje")]
public IActionResult Moje()
{
    var sessionId = Request.Cookies["session"];
    if (string.IsNullOrEmpty(sessionId))
        return Unauthorized("Nie ste prihlásený.");

    using var spojenie = Databaza.OtvorSpojenie();

    var prikaz = spojenie.CreateCommand();
    prikaz.CommandText = "SELECT pouzivatelId FROM Session WHERE sessionId = $sessionId";
    prikaz.Parameters.AddWithValue("$sessionId", sessionId);
    var pouzivatelId = Convert.ToInt64(prikaz.ExecuteScalar());

    var prikaz2 = spojenie.CreateCommand();
    prikaz2.CommandText = @"
        SELECT 
            Kviz.id,
            Kviz.datum,
            COUNT(KvizOdpoved.id) AS max,
            SUM(
                CASE
                    WHEN Odpoved.jeSpravna = 1 THEN 1
                    ELSE 0
                END
            ) AS bodov
        FROM Kviz
        LEFT JOIN KvizOdpoved ON Kviz.id = KvizOdpoved.kvizId
        LEFT JOIN Odpoved ON KvizOdpoved.odpovedId = Odpoved.id
        WHERE Kviz.pouzivatelId = $pid
        GROUP BY Kviz.id
        ORDER BY Kviz.datum DESC";
    prikaz2.Parameters.AddWithValue("$pid", pouzivatelId);

    var zoznam = new List<object>();
    using var reader = prikaz2.ExecuteReader();
    while (reader.Read())
    {
        zoznam.Add(new {
            id = reader.GetInt64(0),
            datum = reader.GetString(1),
            max = reader.GetInt32(2),
            bodov = reader.IsDBNull(3) ? 0 : reader.GetInt32(3)
        });
    }

    return Ok(zoznam);
}
[HttpGet("detail/{id}")]
public IActionResult Detail(long id)
{
    var sessionId = Request.Cookies["session"];
    if (string.IsNullOrEmpty(sessionId))
        return Unauthorized("Nie ste prihlásený.");

    using var spojenie = Databaza.OtvorSpojenie();

    var userCmd = spojenie.CreateCommand();
    userCmd.CommandText = "SELECT pouzivatelId FROM Session WHERE sessionId = $sessionId";
    userCmd.Parameters.AddWithValue("$sessionId", sessionId);
    var pouzivatelId = Convert.ToInt64(userCmd.ExecuteScalar());

    var kontrolaCmd = spojenie.CreateCommand();
    kontrolaCmd.CommandText = "SELECT COUNT(*) FROM Kviz WHERE id = $id AND pouzivatelId = $pid";
    kontrolaCmd.Parameters.AddWithValue("$id", id);
    kontrolaCmd.Parameters.AddWithValue("$pid", pouzivatelId);
    var count = Convert.ToInt32(kontrolaCmd.ExecuteScalar());
    if (count == 0)
        return Forbid("Tento kvíz nie je váš.");

    var prikaz = spojenie.CreateCommand();
    prikaz.CommandText = @"
        SELECT 
            Otazka.text AS otazka,
            Odpoved.text AS odpoved,
            Odpoved.jeSpravna
        FROM KvizOdpoved
        JOIN Otazka ON Otazka.id = KvizOdpoved.otazkaId
        LEFT JOIN Odpoved ON Odpoved.id = KvizOdpoved.odpovedId
        WHERE KvizOdpoved.kvizId = $id";
    prikaz.Parameters.AddWithValue("$id", id);

    var vysledky = new List<object>();
    using var reader = prikaz.ExecuteReader();
    while (reader.Read())
    {
        vysledky.Add(new {
            otazka = reader.GetString(0),
            odpoved = reader.IsDBNull(1) ? "Žiadna odpoveď" : reader.GetString(1),
            jeSpravna = !reader.IsDBNull(2) && reader.GetInt32(2) == 1
        });
    }

    return Ok(vysledky);
}
[HttpPost("odhlasit")]
public IActionResult Odhlasit()
{
    var sessionId = Request.Cookies["session"];
    if (string.IsNullOrEmpty(sessionId))
        return Ok("Neboli ste prihlásený.");

    using var spojenie = Databaza.OtvorSpojenie();

    var prikaz = spojenie.CreateCommand();
    prikaz.CommandText = "DELETE FROM Session WHERE sessionId = $id";
    prikaz.Parameters.AddWithValue("$id", sessionId);
    prikaz.ExecuteNonQuery();


    Response.Cookies.Delete("session");

    return Ok("Boli ste odhlásený.");
}


}
