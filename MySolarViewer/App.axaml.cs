using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.Configuration;
using System.IO;
using System;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using Newtonsoft.Json.Linq;

namespace MySolarViewer
{
    public partial class App : Application
    {
        public static DatabaseSettings DatabaseSettings = new();
        
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
            LiveCharts.Configure(config => config.AddSkiaSharp().AddDarkTheme());
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                LoadSettings();
                desktop.MainWindow = new MainWindow();
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void LoadSettings()
        {
            try
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("settings.json");

                var config = builder.Build();
                DatabaseSettings = config.GetRequiredSection("Database").Get<DatabaseSettings>();
            }
            catch (Exception ex)
            {
                //SukiUI.MessageBox.MessageBox.Error(this, "Error", "Error loading configuration from settings.json, app cannot work without loading this file!");
            }

        }
    }
}
