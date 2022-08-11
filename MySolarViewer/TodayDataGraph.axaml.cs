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
        private readonly  DispatcherTimer _updateDataTimer = new();
        
        private Crosshair _plotCrosshair;
        private AvaPlot _plotControl;
        
        //private List<SolarDataLog> _todayLogs = new List<SolarDataLog>();
        private SignalPlot? _producedPlot;
        private SignalPlot? _consumedPlot;
        private Tooltip? _selectedValueToolTip;

        private int indexTracked = 0;
        
        private double[] _todayTimeRanges = new double[288];
        private double[] _todayPGrid = new double[288];
        private double[] _todayPPv = new double[288];
        private double[] _todayPLoad = new double[288];
        

        public TodayDataGraph()
        {
            // create a timer to modify the data
           // _updateDataTimer.Interval = TimeSpan.FromSeconds(60);
            //_updateDataTimer.Tick += UpdateDailyPlot;
            
            //InitializeDatesArray();
            InitializeComponent();

            //_updateDataTimer.Start();
            
        }

        private void InitializeDatesArray()
        {
            var startDate = DateTime.Now.Date;

            for (var x = 0; x < _todayTimeRanges.Length; ++x)
            {
                _todayTimeRanges[x] = startDate.AddMinutes(x * 5).ToOADate();
            }
        }

        private void UpdateDailyPlot(object? sender, EventArgs e)
        {
            var logs = HistoryDataClient.GetTodayLogs();
            var data = logs.GroupBy(x =>
                {
                    var stamp = x.TimeStamp.ToLocalTime();
                    stamp = stamp.AddMinutes(-(stamp.Minute % 5));
                    stamp = stamp.AddMilliseconds(-stamp.Millisecond - 1000 * stamp.Second);
                    return stamp;
                })
                .Select(g => 
                    new 
                    {
                        AoDateTime = g.Key.ToOADate(),
                        PGrid = Convert.ToDouble(g.Average(x => x.PGrid)),
                        Ppv = Convert.ToDouble(g.Average(x => x.Ppv)),
                        PLoad = Convert.ToDouble(g.Average(x => x.PLoad) * -1)
                    });

            var dataList = data.ToList();
            
            for (var x = 0; x < dataList.Count; ++x)
            {
                _todayPPv[x] = dataList[x].Ppv;
                _todayPLoad[x] = dataList[x].PLoad;
                _todayPGrid[x] = dataList[x].PGrid;
            }

            _plotControl.Refresh();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            //InitializePlotControl();
        }
        
        private void InitializePlotControl()
        {

            
        }

        private void OnMouseEnter(object? sender, PointerEventArgs e)
        {
            _plotCrosshair.IsVisible = true;
            _plotControl.Refresh();
        }

        private void OnMouseLeave(object? sender, PointerEventArgs e)
        {
            _plotCrosshair.IsVisible = false;
            
            if(_selectedValueToolTip != null)
                _selectedValueToolTip.IsVisible = false;
            
            _plotControl.Refresh();
        }

        private void OnMouseMove(object? sender, PointerEventArgs e)
        {

            var pixelX = (int)e.GetPosition(_plotControl).X;
            var pixelY = (int)e.GetPosition(_plotControl).Y;
            var xyRatio = _plotControl.Plot.XAxis.Dims.PxPerUnit / _plotControl.Plot.YAxis.Dims.PxPerUnit;

            (var coordinateX, var coordinateY) = _plotControl.GetMouseCoordinates();

            (var pointX, var pointY, var index) = _consumedPlot.GetPointNearestX(coordinateX);
            
            _plotCrosshair.X = pointX;
            _plotCrosshair.Y = 0;
            _plotCrosshair.Label = "";

            if (indexTracked != index)
            {
                indexTracked = index;
                
                _selectedValueToolTip ??= _plotControl.Plot.AddTooltip("", 0, 0);


                _selectedValueToolTip.X = pointX;
                _selectedValueToolTip.Y = coordinateY;
                _selectedValueToolTip.Label =
                    $"PPV: {_todayPPv[indexTracked]:0.00}W\r\nPGrid: {_todayPGrid[indexTracked]:0.00}W\r\nPLoad: {_todayPLoad[indexTracked]:0.00}W\r\n\r\n{DateTime.FromOADate(_todayTimeRanges[indexTracked]).ToString("t")}";
                //tooltip.GetLegendItems()[0].markerShape = MarkerShape.triUp;
                _selectedValueToolTip.IsVisible = true;

                _plotControl.Refresh();

            }
        }
    }
}
