using SkiaSharp;
using System.Windows;
using XF.ChartLibrary.Charts;
using XF.ChartLibrary.Data;
using XF.ChartLibrary.Utils;

namespace Sample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var entries = new Entry[]
           {
                new Entry(0,0),
                new Entry(10,13),
                new Entry(15,10),
                new Entry(20,4),
                new Entry(25,8)
           };
            var dataSets = new LineDataSet[]
            {
                new LineDataSet(entries, "Sample")
                {
                    Mode = LineDataSet.LineMode.CubicBezier,
                    ValueTextColor = SKColors.Black,
                    DrawFilled = true,
                    Fill = new GradientFill(SKColor.Parse("#266489"), SKColor.Parse("#68B9C0")),
                }.EnableDashedHighlightLine(10f,10f,0)
            };
            LineData data = new LineData(dataSets);
            data.NotifyDataChanged();
            var content = new LineChart()
            {
                Marker = new XF.ChartLibrary.Components.MarkerText(),
                MaxVisibleCount = 3,
                Data = data,
                VisibleXRangeMaximum = 15,
                VisibleXRangeMinimum = 5,
                XAxis =
                {
                    SpaceMax = 1,
                    GranularityEnabled = true,
                },
                AxisLeft =
                {
                    AxisMaximum = 30,
                    LimitLines =
                    {
                        new XF.ChartLibrary.Components.LimitLine(10, "Max")
                        .EnableDashedLine(10f,10f,0),
                        new XF.ChartLibrary.Components.LimitLine(0, "Min")
                        .EnableDashedLine(10f,10f,0),
                    }
                },
                AxisRight =
                {
                    IsEnabled = false
                },
                Lengend =
                {
                    Form = XF.ChartLibrary.Components.Form.Line
                }
            };
            content.SetVisibleYRange(15, 5, XF.ChartLibrary.Components.YAxisDependency.Left);
            content.NotifyDataSetChanged();
            Root.Content = content;
        }
    }
}
