using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Dapper;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using ReactiveUI;
using ScottPlot;
using ScottPlot.Avalonia;
using ScottPlot.Control.EventProcess.Events;
using ScottPlot.Plottable;
using ScottPlot.Renderable;

namespace MySolarViewer
{
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        
        protected override void OnOpened(EventArgs e)
        {
            base.OnOpened(e);
        }
        
    }
}
