using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;


[ApiController]
[Route("[controller]")]
public class PouzivatelController : ControllerBase

{
    [HttpPost("registracia")]
    public IActionResult Registruj([FromForm] string meno, [FromForm] string email, [FromForm] string heslo)
    {
        // Salt generalas
        var salt = Guid.NewGuid().ToString().Substring(0, 8);
        var hash = Zhashuj(heslo + salt);

        using var spojenie = Databaza.OtvorSpojenie();

        var prikaz = spojenie.CreateCommand();
        prikaz.CommandText = @"
            INSERT INTO Pouzivatel (meno, email, hesloHash, hesloSalt)
            VALUES ($meno, $email, $hesloHash, $hesloSalt)";
        prikaz.Parameters.AddWithValue("$meno", meno);
        prikaz.Parameters.AddWithValue("$email", email);
        prikaz.Parameters.AddWithValue("$hesloHash", hash);
        prikaz.Parameters.AddWithValue("$hesloSalt", salt);

        try
        {
            prikaz.ExecuteNonQuery();
            return Ok("Používateľ zaregistrovaný.");
        }
        catch (Exception ex)
        {
            return BadRequest("Chyba pri registrácii: " + ex.Message);
        }
    }

    // Hash generalas SHA256 
    private static string Zhashuj(string vstup)
    {
        using var sha = System.Security.Cryptography.SHA256.Create();
        var bajty = System.Text.Encoding.UTF8.GetBytes(vstup);
        var hash = sha.ComputeHash(bajty);
        return Convert.ToHexString(hash); 
    }
    //bejelentkezes
    [HttpPost("prihlasenie")]
    public IActionResult Prihlasenie([FromForm] string email, [FromForm] string heslo)
    {
        using var spojenie = Databaza.OtvorSpojenie();

        var prikaz = spojenie.CreateCommand();
        prikaz.CommandText = "SELECT id, hesloHash, hesloSalt FROM Pouzivatel WHERE email = $email";
        prikaz.Parameters.AddWithValue("$email", email);

        using var reader = prikaz.ExecuteReader();
        if (!reader.Read())
            return Unauthorized("Používateľ neexistuje.");

        var id = reader.GetInt64(0);
        var hesloHash = reader.GetString(1);
        var hesloSalt = reader.GetString(2);

        var overovanyHash = Zhashuj(heslo + hesloSalt);

        if (overovanyHash != hesloHash)
            return Unauthorized("Nesprávne heslo.");

        var sessionId = Guid.NewGuid().ToString();

        Response.Cookies.Append("session", sessionId);
        var sessionPrikaz = spojenie.CreateCommand();
        sessionPrikaz.CommandText = @"
    INSERT INTO Session (pouzivatelId, sessionId)
    VALUES ($pouzivatelId, $sessionId)";
        sessionPrikaz.Parameters.AddWithValue("$pouzivatelId", id);
        sessionPrikaz.Parameters.AddWithValue("$sessionId", sessionId);
        sessionPrikaz.ExecuteNonQuery();

        return Ok("Prihlásenie úspešné.");
    }

[HttpGet("kto")]
public IActionResult KtoSom()
{
    var sessionId = Request.Cookies["session"];
    if (string.IsNullOrEmpty(sessionId))
        return Unauthorized("Nie ste prihlásený.");

    using var spojenie = Databaza.OtvorSpojenie();

    var prikaz = spojenie.CreateCommand();
    prikaz.CommandText = @"
        SELECT Pouzivatel.id, meno, email
        FROM Session
        JOIN Pouzivatel ON Session.pouzivatelId = Pouzivatel.id
        WHERE sessionId = $sessionId";
    prikaz.Parameters.AddWithValue("$sessionId", sessionId);

    using var reader = prikaz.ExecuteReader();
    if (!reader.Read())
        return Unauthorized("Neplatný session.");

    var meno = reader.GetString(1);
    var email = reader.GetString(2);

    return Ok($"Prihlásený ako: {meno} ({email})");
}

[HttpPost("odhlasit")]
public IActionResult Odhlasit()
{
    var sessionId = Request.Cookies["session"];
    if (!string.IsNullOrEmpty(sessionId))
    {
        using var spojenie = Databaza.OtvorSpojenie();
        var prikaz = spojenie.CreateCommand();
        prikaz.CommandText = "DELETE FROM Session WHERE sessionId = $sessionId";
        prikaz.Parameters.AddWithValue("$sessionId", sessionId);
        prikaz.ExecuteNonQuery();
    }

    Response.Cookies.Delete("session");

    return Ok("Odhlásenie úspešné.");
}

}
