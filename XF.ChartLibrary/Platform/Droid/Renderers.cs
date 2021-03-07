using Xamarin.Forms;
using XF.ChartLibrary.Charts;

[assembly: ExportRenderer(typeof(LineChart), typeof(XF.ChartLibrary.Platform.Droid.ChartViewRenderer<LineChart>))]
[assembly: ExportRenderer(typeof(BarChart), typeof(XF.ChartLibrary.Platform.Droid.ChartViewRenderer<BarChart>))]
[assembly: ExportRenderer(typeof(PieChart), typeof(XF.ChartLibrary.Platform.Droid.ChartViewRenderer<PieChart>))]
