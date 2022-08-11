using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;

namespace MySolarViewer;

public static class HistoryDataClient
{

    public static List<SolarDataLog> GetTodayLogs()
    {

        var start = DateTime.Now.Date;
        var end = start.AddDays(1).AddSeconds(-1);

        var sql =
            "SELECT timestamp AS TimeStamp, e_day AS Eday, e_year AS EYear, e_total AS ETotal, mode AS Mode, p_akku AS PAkku, p_grid AS PGrid, p_load AS PLoad, p_pv AS Ppv, rel_autonomy AS RelAutonomy, rel_self_consumption AS RelSelfConsumption, backup_mode AS BackupMode, battery_standby  AS BatteryStandBy FROM solar_production_log WHERE timestamp BETWEEN ?startDate AND ?endDate";

        using var connection = new MySqlConnection(
            $"server={App.DatabaseSettings.Server};user={App.DatabaseSettings.User};password={App.DatabaseSettings.Password};database={App.DatabaseSettings.DatabaseName}");

        var solarData = connection.Query<SolarDataLog>(sql, new { startDate = start.ToUniversalTime(), endDate = end.ToUniversalTime() });
        return solarData.ToList();

    }
}