using XF.ChartLibrary.Charts;
using XF.ChartLibrary.Utils;

namespace XF.ChartLibrary.Jobs
{
    partial class AnimatedViewPortJob
    {
        private long startTime;
        private long duration;
        private long endTime;
        protected Animation.EasingFunction easing;

        public AnimatedViewPortJob(ViewPortHandler viewPortHandler, float xValue, float yValue, Transformer trans, IChartBase v, float xOrigin, float yOrigin, long duration) :
            base(viewPortHandler, xValue, yValue, trans, v)
        {
            this.xOrigin = xOrigin;
            this.yOrigin = yOrigin;
            this.duration = duration;
        }
    }
}
