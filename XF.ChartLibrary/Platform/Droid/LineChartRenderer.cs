using Android.Content;
using Xamarin.Forms;
using XF.ChartLibrary.Charts;
using XF.ChartLibrary.Data;
using XF.ChartLibrary.Platform.Droid;

[assembly: ExportRenderer(typeof(LineChart), typeof(LineChartRenderer))]
namespace XF.ChartLibrary.Platform.Droid
{
    public class LineChartRenderer : ChartViewBaseRenderer<LineData, LineDataSet>
    {
        public LineChartRenderer(Context context) : base(context)
        {
        }
    }
}
