using XF.ChartLibrary.Animation;
using XF.ChartLibrary.Charts;
using XF.ChartLibrary.Utils;

namespace XF.ChartLibrary.Renderer
{
    public partial class RadarChartRenderer : LineRadarRenderer
    {
        protected RadarChart Chart;

        public RadarChartRenderer(RadarChart chart, Animator animator, ViewPortHandler viewPortHandler) : base(animator, viewPortHandler)
        {
            Chart = chart;
        }

        public override void InitBuffers()
        {
            // TODO Auto-generated method stub
        }
    }
}
