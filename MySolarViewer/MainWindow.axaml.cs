using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Dapper;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using ReactiveUI;
using ScottPlot.Avalonia;

namespace MySolarViewer
{
    public partial class MainWindow : Window
    {
        public ICommand ReloadDataCommand;
        private DatabaseSettings? _databaseSettings = null;

        private List<SolarDataLog> _todayLogs = new List<SolarDataLog>();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            LoadPlots();
            ReloadDataCommand = ReactiveCommand.Create(LoadData);

        }

        private void LoadPlots()
        {
            double[] dataX = new double[] { 1, 2, 3, 4, 5 };
            double[] dataY = new double[] { 1, 4, 9, 16, 25 };
            AvaPlot avaPlot1 = this.Find<AvaPlot>("AvaPlot1");
            avaPlot1.Plot.AddScatter(dataX, dataY);
            avaPlot1.Refresh();
        }

        protected override void OnOpened(EventArgs e)
        {
            base.OnOpened(e);
            LoadConfiguration();
        }

        private void LoadData()
        {
            try
            {
                var start = DateTime.Now.Date;
                var end = start.AddDays(1).AddSeconds(-1);

                var sql =
                    "SELECT timestamp AS TimeStamp, e_day AS Eday, e_year AS EYear, e_total AS ETotal, mode AS Mode, p_akku AS PAkku, p_grid AS PGrid, p_load AS PLoad, p_pv AS Ppv, rel_autonomy AS RelAutonomy, rel_self_consumption AS RelSelfConsumption, backup_mode AS BackupMode, battery_standby  AS BatteryStandBy FROM solar_production_log WHERE timestamp BETWEEN ?startDate AND ?endDate";
                
                using var connection = new MySqlConnection(
                    $"server={_databaseSettings.Server};user={_databaseSettings.User};password={_databaseSettings.Password};database={_databaseSettings.DatabaseName}");

                var solarData = connection.Query<SolarDataLog>(sql, new { startDate = start, endDate = end });
                _todayLogs = solarData.ToList();

            }
            catch (Exception ex)
            {
                SukiUI.MessageBox.MessageBox.Error(this, "Error", "Data cannot be loaded, a connection to the database could not be established or some other error ocurred.!");
            }

        }

        private void LoadConfiguration()
        {
            try
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("settings.json");

                var config = builder.Build();
                _databaseSettings = config.GetRequiredSection("Database").Get<DatabaseSettings>();
            }
            catch (Exception ex)
            {
                SukiUI.MessageBox.MessageBox.Error(this, "Error", "Error loading configuration from settings.json, app cannot work without loading this file!");
            }

        }

        private void Refresh_OnClick(object? sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void Redraw_OnClick(object? sender, RoutedEventArgs e)
        {
            DrawDaily();
        }


        private void DrawDaily()
        {
            var data = _todayLogs.GroupBy(x =>
            {
                var stamp = x.TimeStamp;
                stamp = stamp.AddMinutes(-(stamp.Minute % 5));
                stamp = stamp.AddMilliseconds(-stamp.Millisecond - 1000 * stamp.Second);
                return stamp;
            })
                .Select(g => new
                {
                    Date = g.Key, PGrid = g.Average(x => x.PGrid), Ppv = g.Average(x => x.Ppv),
                    PLoad = g.Average(x => x.PLoad)
                });

            var dataList = data.ToList();


        }
    }
}
