using XF.ChartLibrary.Animation;
using XF.ChartLibrary.Interfaces.DataProvider;
using XF.ChartLibrary.Utils;

namespace XF.ChartLibrary.Renderer
{
    public partial class HorizontalBarChartRenderer : BarChartRenderer
    {
        public HorizontalBarChartRenderer(IBarDataProvider chart, Animator animator, ViewPortHandler viewPortHandler) : base(chart, animator, viewPortHandler)
        {
        }
    }
}
