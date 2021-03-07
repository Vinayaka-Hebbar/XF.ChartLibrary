using XF.ChartLibrary.Animation;
using XF.ChartLibrary.Utils;

namespace XF.ChartLibrary.Renderer
{
    public abstract partial class LineScatterCandleRadarRenderer : BarLineScatterCandleBubbleRenderer
    {
        protected LineScatterCandleRadarRenderer(Animator animator, ViewPortHandler viewPortHandler) : base(animator, viewPortHandler)
        {
        }
    }
}
