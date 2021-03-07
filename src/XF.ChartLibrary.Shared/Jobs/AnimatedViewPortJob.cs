using XF.ChartLibrary.Charts;
using XF.ChartLibrary.Utils;

namespace XF.ChartLibrary.Jobs
{
    public abstract partial class AnimatedViewPortJob : ViewPortJob
    {
        protected float phase;

        protected float xOrigin;
        protected float yOrigin;
    }
}
