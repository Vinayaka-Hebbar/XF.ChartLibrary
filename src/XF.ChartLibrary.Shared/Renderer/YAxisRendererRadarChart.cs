using XF.ChartLibrary.Charts;
using XF.ChartLibrary.Components;
using XF.ChartLibrary.Utils;

namespace XF.ChartLibrary.Renderer
{
    public partial class YAxisRendererRadarChart : YAxisRenderer
    {
        protected RadarChart Chart;

        public YAxisRendererRadarChart(ViewPortHandler viewPortHandler, YAxis axis, RadarChart chart) : base(viewPortHandler, axis, null)
        {
            Chart = chart;
        }
    }
}
