using Android.Animation;
using Java.Interop;
using System;
using XF.ChartLibrary.Charts;
using XF.ChartLibrary.Utils;

namespace XF.ChartLibrary.Jobs
{
    partial class AnimatedViewPortJob : ValueAnimator.IAnimatorUpdateListener, Animator.IAnimatorListener
    {
        protected ObjectAnimator animator;

        [ExportField("phase")]
        public float GetPhase() => phase;


        public AnimatedViewPortJob(ViewPortHandler viewPortHandler, float xValue, float yValue, Transformer trans, IChartBase v, float xOrigin, float yOrigin, long duration) :
            base(viewPortHandler, xValue, yValue, trans, v)
        {
            this.xOrigin = xOrigin;
            this.yOrigin = yOrigin;
            animator = ObjectAnimator.OfFloat(this, "phase", 0f, 1f);
            animator.SetDuration(duration);
            animator.AddUpdateListener(this);
            animator.AddListener(this);
        }

        public abstract void RecycleSelf();

        public void OnAnimationCancel(Animator animation)
        {
            try
            {
                RecycleSelf();
            }
            catch (ArgumentException)
            {
                // don't worry about it.
            }
        }

        public void OnAnimationEnd(Animator animation)
        {
            try
            {
                RecycleSelf();
            }
            catch (ArgumentException)
            {
                // don't worry about it.
            }
        }

        public void OnAnimationRepeat(Animator animation)
        {
        }

        public void OnAnimationStart(Animator animation)
        {
        }

        public void OnAnimationUpdate(ValueAnimator animation)
        {
        }
    }
}
