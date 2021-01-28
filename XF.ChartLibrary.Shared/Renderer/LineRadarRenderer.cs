using XF.ChartLibrary.Animation;
using XF.ChartLibrary.Utils;

namespace XF.ChartLibrary.Renderer
{
    public abstract partial class LineRadarRenderer : LineScatterCandleRadarRenderer
    {
        protected LineRadarRenderer(Animator animator, ViewPortHandler viewPortHandler) : base(animator, viewPortHandler)
        {
        }
    }
}
