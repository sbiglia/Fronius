namespace FroniusDataCollector.Settings;

public class AppSettings
{
    public int DataRecordDataEvery { get; set; } = 30;
    public string DatabaseType { get; set; } = "POSTGRES";
}