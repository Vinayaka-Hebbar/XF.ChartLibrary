using SkiaSharp;
using Xamarin.Forms;
using XF.ChartLibrary.Charts;
using XF.ChartLibrary.Components;
using XF.ChartLibrary.Data;
using XF.ChartLibrary.Utils;
using Entry = XF.ChartLibrary.Data.Entry;

namespace Sample
{
    public partial class MainPage : ContentPage, XF.ChartLibrary.Formatter.IAxisValueFormatter
    {
        static readonly SKColor Color1 = SKColor.Parse("#3498db");
        static readonly SKColor Color2 = SKColor.Parse("#e74c3c");

        private readonly LineChart chart;
        public MainPage()
        {
            InitializeComponent();
            var dataSets = new LineDataSet[]
            {
                new LineDataSet(new Entry[]
           {
                new Entry(0,0),
                new Entry(10,10),
                new Entry(20,20),
                new Entry(30,25),
                new Entry(40,15),
                new Entry(50,40),
                new Entry(60,12),
           }, "Sample1")
                {
                    ValueFormatter = null,
                    Mode = LineDataSet.LineMode.CubicBezier,
                    ValueTextColor = SKColors.Black,
                    DrawFilled = true,
                    Color = Color1,
                    FillColor = Color1,
                }.EnableDashedHighlightLine(10f,10f,0),
                new LineDataSet(new Entry[]
           {
                new Entry(0,0),
                new Entry(5,10),
                new Entry(15,20),
                new Entry(25,22),
                new Entry(40,15),
                new Entry(50,30),
                new Entry(60,12),
           }, "Sample2")
                {
                    ValueFormatter = null,
                    Mode = LineDataSet.LineMode.CubicBezier,
                    ValueTextColor = SKColors.Black,
                    DrawFilled = true,
                    Color = Color2,
                    FillColor = Color2,
                }.EnableDashedHighlightLine(10f,10f,0)
            };
            LineData data = new LineData(dataSets);
            data.NotifyDataChanged();
            chart = new LineChart()
            {
                Marker = new MarkerViewXY(),
                MaxVisibleCount = 3,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                XAxis =
                {
                    AxisRange = 100,
                    SpaceMax = 1,
                    ValueFormatter = this
                },
                VisibleXRangeMaximum = 30,
                VisibleXRangeMinimum = 5,
                Data = data,
                AxisLeft =
                {
                    AxisMaximum = 50,
                    DrawLimitLinesBehindData = true,
                    LimitLines =
                    {
                        new LimitLine(10, "Max")
                        .EnableDashedLine(10f,10f,0),
                        new LimitLine(0, "Min")
                        .EnableDashedLine(10f,10f,0),
                    }
                },
                AxisRight =
                {
                    IsEnabled = false
                },
                Legend =
                {
                    Form = XF.ChartLibrary.Components.Form.Circle
                }
            };
            chart.SetVisibleYRange(40, 10, XF.ChartLibrary.Components.YAxisDependency.Left);
            chart.NotifyDataSetChanged();
            Grid.SetRow(chart, 1);
            LayoutRoot.Children.Add(chart);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            chart.Animator.AnimateY(3000, XF.ChartLibrary.Animation.EasingOption.EaseOutSine);
        }

        public string GetFormattedValue(float value, AxisBase axis)
        {
            return value.ToString();
        }
    }
}
