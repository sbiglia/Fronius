using FroniusSolarClient.Entities.SolarAPI.V1;
using System;
using System.Collections.Generic;
using System.Data;
using FroniusDataCollector.Settings;
using FroniusSolarClient;
using FroniusSolarClient.Entities.SolarAPI.V1.PowerFlowRealtimeData;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Npgsql;
using NpgsqlTypes;
using Serilog;
using Spectre.Console;

namespace FroniusDataCollector
{
    class Program
    {

        private static FroniusSettings _froniusSettings = new FroniusSettings();
        private static DatabaseSettings _databaseSettings = new DatabaseSettings();
        private static AppSettings _appSettings = new AppSettings();
        private static IDataSaver? _dataSaver = null;

        
        private static bool _stop = false;

        private static void Main(string[] args)
        {

            try
            {
                LoadConfiguration();
                _dataSaver = CreateDataSaver();
            }
            catch
            {
                ConsoleWriter.WriteErrorMessage("Configuration cannot be loaded! Please fix it!");
                return;
            }

            Log.Logger = new LoggerConfiguration()
                .WriteTo.File("datacollector.log", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7)
                .CreateLogger();

            AnsiConsole.Status()
                .AutoRefresh(true)
                .Spinner(Spinner.Known.Star)
                .Start("[yellow]Collecting data... (ESC) to exit.[/]", ctx =>
                {
                    var backgroundThread = new Thread(DoBackgroundWork);
                    backgroundThread.Start();

                    while (Console.ReadKey(true).Key != ConsoleKey.Escape)
                    {
                        // do nothing until escape
                    }
                     
                    _stop = true;

                    backgroundThread.Join();

                });

        }

        private static void LoadConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("settings.json");

            var config = builder.Build();
            _databaseSettings = config.GetRequiredSection("Database").Get<DatabaseSettings>();
            _froniusSettings = config.GetSection("Fronius").Get<FroniusSettings>();
            _appSettings = config.GetSection("AppSettings").Get<AppSettings>();

        }



        static void DoBackgroundWork()
        {
            var nullLogger = NullLoggerFactory.Instance.CreateLogger("DummyLogger");
            
            var client = new SolarClient(_froniusSettings.DataManagerIp, 1, nullLogger);

            while (!_stop)
            {
                var response = GetRealTimeData(client);

                if (response == null)
                {
                    Log.Logger.Information($"Response data from fronius: [null]!!!");
                }
                else
                {

                    Log.Logger.Information(
                        $"Response data from fronius: [Code: {response.Head.Status.Code}] [Reason: {response.Head.Status.Reason}] [UserMessage: {response.Head.Status.UserMessage}]");

                    _dataSaver?.SaveData(response.Body.Data, response.Head.Timestamp);
                }

                var secondsToWait = _appSettings.DataRecordDataEvery;

                while (secondsToWait > 0 && !_stop)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    secondsToWait--;
                }
            }

            ConsoleWriter.WriteLogMessage("Byebye;");

        }
        
        static IDataSaver? CreateDataSaver()
        {

            switch (_appSettings.DatabaseType)
            {
                case "POSTGRES":
                {
                    return new PostgresDataSaver(_databaseSettings);
                }
                case "MYSQL":
                {
                    return new MysqlDataSaver(_databaseSettings);
                }
            }

            return null;
        }


        #region RealtimeData

        static Response<FroniusSolarClient.Entities.SolarAPI.V1.PowerFlowRealtimeData.PowerFlowRealtimeData> GetRealTimeData(SolarClient client)
        {
            return client.GetPowerFlowRealtimeData();
        }

        #endregion

        #region NotUsed

        static void GetArchiveDataBetweenDates(SolarClient client)
        {
            var channels = new List<Channel>
                { Channel.Voltage_AC_Phase_1, Channel.Voltage_AC_Phase_2, Channel.Voltage_AC_Phase_3 };

            var dateFrom = DateTime.Parse("01/08/2019");
            var dateTo = DateTime.Parse("05/08/2019");

            var data = client.GetArchiveData(dateFrom, dateTo, channels);

            Console.WriteLine(data);
        }

        static void GetArchiveDataOverPast24Hours(SolarClient client)
        {
            var channels = new List<Channel>
                { Channel.Voltage_AC_Phase_1, Channel.Voltage_AC_Phase_2, Channel.Voltage_AC_Phase_3 };

            var data = client.GetArchiveData(DateTime.Now.AddDays(-1), DateTime.Now, channels);

            Console.WriteLine(data.Body);
        }

        
        static void GetPowerFlowRealtimeData(SolarClient client)
        {
            var data = client.GetPowerFlowRealtimeData();

            Console.WriteLine(data);
        }

        #endregion
    }
}
