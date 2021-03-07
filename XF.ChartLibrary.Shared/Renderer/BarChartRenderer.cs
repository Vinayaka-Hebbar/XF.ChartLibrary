using XF.ChartLibrary.Interfaces.DataProvider;
using XF.ChartLibrary.Utils;

namespace XF.ChartLibrary.Renderer
{
    public partial class BarChartRenderer : BarLineScatterCandleBubbleRenderer
    {
        protected IBarDataProvider Chart;

        public BarChartRenderer(IBarDataProvider chart, Animation.Animator animator,
                                ViewPortHandler viewPortHandler) : base(animator, viewPortHandler)
        {
            Chart = chart;
        }

    }
}
