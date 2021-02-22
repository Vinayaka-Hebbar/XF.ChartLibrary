using SkiaSharp;
using Xamarin.Forms;
using XF.ChartLibrary.Charts;
using XF.ChartLibrary.Data;
using XF.ChartLibrary.Utils;
using Entry = XF.ChartLibrary.Data.Entry;

namespace Sample
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            var entries = new Entry[]
           {
                new Entry(0,0),
                new Entry(10,10),
                new Entry(20,20),
                new Entry(30,25),
                new Entry(40,15),
                new Entry(50,40),
                new Entry(60,12),
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
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Data = data,
                VisibleXRangeMaximum = 30,
                VisibleXRangeMinimum = 5,
                XAxis =
                {
                    SpaceMax = 1,
                },
                AxisLeft =
                {
                    AxisMaximum = 50,
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
            content.SetVisibleYRange(40, 10, XF.ChartLibrary.Components.YAxisDependency.Left);
            content.NotifyDataSetChanged();
            Grid.SetRow(content, 1);
            LayoutRoot.Children.Add(content);
        }
    }
}
