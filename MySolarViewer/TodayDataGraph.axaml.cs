using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using ScottPlot.Avalonia;
using ScottPlot.Plottable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using static MySolarViewer.MainWindow;

namespace MySolarViewer
{
    public partial class TodayDataGraph : UserControl
    {
        

        public TodayDataGraph()
        {
            // create a timer to modify the data
           // _updateDataTimer.Interval = TimeSpan.FromSeconds(60);
            //_updateDataTimer.Tick += UpdateDailyPlot;
            
            //InitializeDatesArray();
            InitializeComponent();

            //_updateDataTimer.Start();
            
        }
        
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        
        
    }
}
