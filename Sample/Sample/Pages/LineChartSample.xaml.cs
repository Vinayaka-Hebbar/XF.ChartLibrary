using SkiaSharp;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XF.ChartLibrary.Components;
using XF.ChartLibrary.Data;
using XF.ChartLibrary.Utils;
using Entry = XF.ChartLibrary.Data.Entry;

namespace Sample.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LineChartSample : ContentPage
    {
        static readonly SKColor Color1 = SKColor.Parse("#3498db");
        static readonly SKColor Color2 = SKColor.Parse("#e74c3c");

        public LineChartSample()
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
            var xAxis = Chart.XAxis;
            xAxis.AxisRange = 100;
            xAxis.SpaceMax = 1;
            var axisLeft = Chart.AxisLeft;
            axisLeft.AxisMaximum = 50;
            axisLeft.DrawLimitLinesBehindData = true;
            axisLeft.LimitLines.Add(new LimitLine(10, "Max")
        .EnableDashedLine(10f, 10f, 0));
            axisLeft.LimitLines.Add(new LimitLine(0, "Min")
                        .EnableDashedLine(10f, 10f, 0));

            Chart.AxisRight.IsEnabled = false;
            Chart.Legend.Form = Form.Circle;
            Chart.Data = data;
            Chart.Marker = new MarkerViewXY();
            Chart.XAxis.ResetAxisMaximum();
            Chart.SetVisibleYRange(50, 10, YAxisDependency.Left);
            Chart.NotifyDataSetChanged();
        }

        protected override void OnAppearing()
        {
            Chart.Animator.AnimateY(3000, XF.ChartLibrary.Animation.EasingOption.EaseOutSine);
        }
    }
}