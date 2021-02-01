using XF.ChartLibrary.Animation;
using XF.ChartLibrary.Interfaces.DataProvider;
using XF.ChartLibrary.Utils;

namespace XF.ChartLibrary.Renderer
{
    public partial class LineChartRenderer : LineRadarRenderer
    {
        protected readonly ILineChartDataProvider Chart;

        public LineChartRenderer(ILineChartDataProvider chart, Animator animator, ViewPortHandler viewPortHandler) : base(animator, viewPortHandler)
        {
            Chart = chart;
        }

        public override void InitBuffers()
        {
        }
    }
}
