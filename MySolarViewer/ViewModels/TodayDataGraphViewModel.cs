using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Threading;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace MySolarViewer.ViewModels
{
    public class TodayDataGraphViewModel
    {
        private readonly ObservableCollection<DateTimePoint> _producedPoints;
        private readonly ObservableCollection<DateTimePoint> _consumedPoints;

        private readonly DispatcherTimer _updateDataTimer = new();

        public ObservableCollection<ISeries> Series { get; set; }

        private static readonly SKColor Blue = new(25, 118, 210);
        private static readonly SKColor Red = new(229, 57, 53);
        private static readonly SKColor Yellow = new(198, 167, 0);

        public Axis[] XAxes { get; set; } =
        {
            new Axis
            {
                Labeler = value => new DateTime((long) value).ToString("HH:mm"),
                UnitWidth = TimeSpan.FromMinutes(5).Ticks,
                MinStep = TimeSpan.FromMinutes(5).Ticks,
                TextSize = 8
            }
        };


        public TodayDataGraphViewModel()
        {
            _producedPoints = new ObservableCollection<DateTimePoint>();
            _consumedPoints = new ObservableCollection<DateTimePoint>();

            Series = new ObservableCollection<ISeries>()
            {
                new LineSeries<DateTimePoint>
                {
                    Values = _producedPoints,
                    Fill = new SolidColorPaint(SKColors.LightGoldenrodYellow),
                    GeometrySize = 0,
                    //GeometryStroke = new SolidColorPaint(SKColors.CadetBlue, 2),
                    Stroke = new SolidColorPaint(Yellow, 2),
                    Name = "Produced",
                    TooltipLabelFormatter = (chart) => $"Produced: {chart.PrimaryValue:N2}W"

                },
                new LineSeries<DateTimePoint>
                {
                    Values = _consumedPoints,
                    Fill = null,
                    GeometrySize = 2,
                    GeometryStroke = new SolidColorPaint(Blue, 2),
                    Stroke = new SolidColorPaint(Blue, 2),
                    Name="Consumed",
                    TooltipLabelFormatter = (chart) => $"Consumed: {chart.PrimaryValue:N2}W | {chart.SecondaryValue}"
                }
            };

            _updateDataTimer.Interval = TimeSpan.FromSeconds(60);
            _updateDataTimer.Tick += UpdateTimer;
            
            GenerateEmptyData();
            UpdateData();

            _updateDataTimer.Start();
        }

        private void UpdateTimer(object? sender, EventArgs e)
        {
            UpdateData();
        }

        private void UpdateData()
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
                        DateTime = g.Key,
                        PGrid = Convert.ToDouble(g.Average(x => x.PGrid)),
                        Ppv = Convert.ToDouble(g.Average(x => x.Ppv)),
                        PLoad = Convert.ToDouble(g.Average(x => x.PLoad) * -1)
                    });

            var dataList = data.ToList();

            foreach (var log in dataList)
            {
                var produced = _producedPoints.SingleOrDefault(x => x.DateTime == log.DateTime);

                if (produced == null)
                {
                    _producedPoints.Add(new DateTimePoint(log.DateTime, log.Ppv));
                }
                else
                {
                    produced.Value = log.Ppv;
                }

                var consumed = _consumedPoints.SingleOrDefault(x => x.DateTime == log.DateTime);

                if (consumed == null)
                {
                    _consumedPoints.Add(new DateTimePoint(log.DateTime, log.PLoad));
                }
                else
                {
                    consumed.Value = log.PLoad;
                }
            }
        }

        public void GenerateEmptyData()
        {
            var startDate = DateTime.Now.Date;
            var endDate = startDate.AddDays(1);
            
            while (startDate < endDate)
            {
                _producedPoints.Add(new DateTimePoint(startDate, null));
                _consumedPoints.Add(new DateTimePoint(startDate, null));
                startDate = startDate.AddMinutes(5);
            }
        }
    }
}
