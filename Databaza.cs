using Microsoft.Data.Sqlite;
using System.IO;

public class Databaza
{
    private static readonly string cesta = Path.Combine(Directory.GetCurrentDirectory(), "vodicak.db");

    protected static string spojenie = $"Data Source={cesta}";

    public static SqliteConnection OtvorSpojenie()
    {
        var spojenieObj = new SqliteConnection(spojenie);
        spojenieObj.Open();
        return spojenieObj;
    }
}
