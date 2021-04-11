using XF.ChartLibrary.Charts;
using XF.ChartLibrary.Components;
using XF.ChartLibrary.Utils;

namespace XF.ChartLibrary.Renderer
{
    public partial class XAxisRendererRadarChart : XAxisRenderer
    {
        protected RadarChart Chart;

        public XAxisRendererRadarChart(ViewPortHandler viewPortHandler, XAxis axis, RadarChart chart) : base(viewPortHandler, axis, null)
        {
            Chart = chart;
        }
    }
}
