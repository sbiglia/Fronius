using System.Data;
using FroniusDataCollector.Settings;
using FroniusSolarClient.Entities.SolarAPI.V1.PowerFlowRealtimeData;
using Npgsql;
using NpgsqlTypes;
using Serilog;

namespace FroniusDataCollector;

public class PostgresDataSaver : IDataSaver
{

    private NpgsqlConnection? _connection = null;
    private readonly DatabaseSettings _settings = new DatabaseSettings();

    public PostgresDataSaver(DatabaseSettings settings)
    {
        _settings = settings;
    }
    
    public void SaveData(PowerFlowRealtimeData data)
    {
        try
        {
            if (_connection == null)
            {
                _connection =
                    new NpgsqlConnection(
                        $"Host={_settings.Server};Database={_settings.DatabaseName};Username={_settings.User};Password={_settings.Password}");
                _connection.Open();
            }

            var sql = "INSERT INTO solar_production_log (timestamp, e_day, e_year, e_total, mode, p_akku, p_grid, p_load, p_pv, rel_autonomy, rel_self_consumption, backup_mode, battery_standby) VALUES ($1, $2, $3, $4, $5, $6, $7, $8, $9, $10, $11, $12, $13)";

            using var command = new NpgsqlCommand(sql, _connection)
            {
                Parameters =
                    {
                        new () {Value = DateTime.Now, NpgsqlDbType = NpgsqlDbType.Timestamp},
                        new () {Value = data.Site.EDay},
                        new () {Value = data.Site.EYear},
                        new () {Value = data.Site.ETotal},
                        new () {Value = data.Site.Mode},
                        new () {Value = data.Site.PAkku},
                        new () {Value = data.Site.PGrid},
                        new () {Value = data.Site.PLoad},
                        new () {Value = data.Site.PPV},
                        new () {Value = data.Site.RelAutonomy == null ? DBNull.Value : data.Site.RelAutonomy},
                        new () {Value = data.Site.RelSelfConsumption == null ? DBNull.Value : data.Site.RelSelfConsumption },
                        new () {Value = data.Site.BackupMode},
                        new () {Value = data.Site.BatteryStandby},
                    }
            };

            command.CommandType = CommandType.Text;
            command.ExecuteNonQuery();
            ConsoleWriter.WriteLogMessage($"Data Saved: [[PGRID: {data.Site.PGrid}]] [[PLOAD: {data.Site.PLoad}]] [[PAKKU: {data.Site.PAkku}]] [[PPV: {data.Site.PPV}]]");

        }
        catch (Exception ex)
        {
            _connection = null;
            Log.Logger.Error(ex, "Error saving data to the database!!");
            ConsoleWriter.WriteErrorMessage("Error saving data to the databse!!");
        }
    }
}