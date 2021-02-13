using Xamarin.Forms;
using XF.ChartLibrary.Charts;
using XF.ChartLibrary.Data;
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
                    ValueTextColor = SkiaSharp.SKColors.Black,
                    DrawFilled = true,
                }
            };
            var content = new LineChart()
            {
                Margin = new Thickness(15),
                MaxVisibleCount = 3,
                VisibleXRangeMaximum = 10,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Data = new LineData(dataSets),
                XAxis =
                {
                    SpaceMax = 1,
                    DrawGridLinesBehindData = false,
                    GranularityEnabled = true,
                },
                AxisLeft =
                {
                    LimitLines =
                    {

                        new XF.ChartLibrary.Components.LimitLine(10, "Max")
                        {

                        },
                        new XF.ChartLibrary.Components.LimitLine(0, "Min")
                        {

                        },
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
            Grid.SetRow(content, 1);
            LayoutRoot.Children.Add(content);
        }
    }
}
