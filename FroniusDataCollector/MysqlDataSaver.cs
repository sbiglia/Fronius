using System.ComponentModel.DataAnnotations;
using System.Data;
using FroniusDataCollector.Settings;
using FroniusSolarClient.Entities.SolarAPI.V1.PowerFlowRealtimeData;
using MySqlConnector;
using Serilog;

namespace FroniusDataCollector;

public class MysqlDataSaver : IDataSaver
{
    private MySqlConnection? _connection = null;
    private readonly DatabaseSettings _settings;

    public MysqlDataSaver(DatabaseSettings settings)
    {
        _settings = settings;
    }

    public void SaveData(PowerFlowRealtimeData data, DateTime timestamp)
    {
        try
        {
            if (_connection == null)
            {
                _connection =
                    new MySqlConnection(
                        $"server={_settings.Server};database={_settings.DatabaseName};user={_settings.User};password={_settings.Password}");
                _connection.Open();
            }

            var sql = "INSERT INTO solar_production_log (timestamp, e_day, e_year, e_total, mode, p_akku, p_grid, p_load, p_pv, rel_autonomy, rel_self_consumption, backup_mode, battery_standby) VALUES (@time, @eday, @eyear, @etotal, @mode, @pakku, @pgrid, @pload, @ppv, @relAutonomy, @relSelfConsumption, @backupMode, @batteryStandby)";

            using var command = new MySqlCommand(sql, _connection);
            command.Parameters.AddWithValue("@time", timestamp.ToUniversalTime());
            command.Parameters.AddWithValue("@eday", data.Site.EDay);
            command.Parameters.AddWithValue("@eyear", data.Site.EYear);
            command.Parameters.AddWithValue("@etotal", data.Site.ETotal);
            command.Parameters.AddWithValue("@mode", data.Site.Mode);
            command.Parameters.AddWithValue("@pakku", data.Site.PAkku);
            command.Parameters.AddWithValue("@pgrid", data.Site.PGrid);
            command.Parameters.AddWithValue("@pload", data.Site.PLoad);
            command.Parameters.AddWithValue("@ppv", data.Site.PPV);
            command.Parameters.AddWithValue("@relAutonomy",
                data.Site.RelAutonomy == null ? DBNull.Value : data.Site.RelAutonomy);
            command.Parameters.AddWithValue("@relSelfConsumption",
                data.Site.RelSelfConsumption == null ? DBNull.Value : data.Site.RelSelfConsumption);
            command.Parameters.AddWithValue("@backupMode", data.Site.BackupMode);
            command.Parameters.AddWithValue("@batteryStandby", data.Site.BatteryStandby);
            command.CommandType = CommandType.Text;
            command.ExecuteNonQuery();
            ConsoleWriter.WriteLogMessage($"Data Saved: [[PGRID: {data.Site.PGrid}]] [[PLOAD: {data.Site.PLoad}]] [[PAKKU: {data.Site.PAkku}]] [[PPV: {data.Site.PPV}]] [[Time: {timestamp.ToUniversalTime()}]]");

        }
        catch (Exception ex)
        {
            _connection = null;
            Log.Logger.Error(ex, "Error saving data to the database!!");
            ConsoleWriter.WriteErrorMessage("Error saving data to the databse!!");
        }
    }
}