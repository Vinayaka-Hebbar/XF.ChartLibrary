using XF.ChartLibrary.Animation;
using XF.ChartLibrary.Interfaces.DataProvider;
using XF.ChartLibrary.Utils;

namespace XF.ChartLibrary.Renderer
{
    public abstract partial class LineChartRenderer : LineRadarRenderer
    {
        protected readonly ILineChartDataProvider Chart;

        protected LineChartRenderer(ILineChartDataProvider chart, Animator animator, ViewPortHandler viewPortHandler) : base(animator, viewPortHandler)
        {
            Chart = chart;
        }
    }
}
