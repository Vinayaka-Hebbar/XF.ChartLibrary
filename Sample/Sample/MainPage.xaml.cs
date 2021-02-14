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
            LineData data = new LineData(dataSets);
            data.NotifyDataChanged();
            var content = new LineChart()
            {
                MaxVisibleCount = 3,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Data = data,
                VisibleXRangeMaximum = 15,
                VisibleXRangeMinimum = 5,
                XAxis =
                {
                    SpaceMax = 1,
                    DrawGridLinesBehindData = false,
                    GranularityEnabled = true,
                },
                AxisLeft =
                {
                    AxisMaximum = 30,
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
            content.SetVisibleYRange(15, 5, XF.ChartLibrary.Components.YAxisDependency.Left);
            content.NotifyDataSetChanged();
            Grid.SetRow(content, 1);
            LayoutRoot.Children.Add(content);
        }
    }
}
