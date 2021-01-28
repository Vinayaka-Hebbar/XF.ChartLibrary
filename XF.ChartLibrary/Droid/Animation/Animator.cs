using Android.Animation;

namespace XF.ChartLibrary.Animation
{
    public partial class AnimatorDelegate : Java.Lang.Object, ValueAnimator.IAnimatorUpdateListener
    {
        public virtual void OnAnimationUpdate(ValueAnimator animation)
        {
        }
    }

    public partial class Animator : Java.Lang.Object
    {
        [Java.Interop.ExportField("phaseX")]
        public float GetPhaseX() => PhaseX;

        [Java.Interop.ExportField("phaseY")]
        public float GetPhaseY() => PhaseX;

        ObjectAnimator XAnimator(long duration, EasingFunction easing)
        {
            ObjectAnimator animatorX = ObjectAnimator.OfFloat(this, "phaseX", 0f, 1f);
            animatorX.SetInterpolator(new AnimationInterpolator(easing));
            animatorX.SetDuration(duration);
            return animatorX;
        }

        private ObjectAnimator YAnimator(long duration, EasingFunction easing)
        {

            ObjectAnimator animatorY = ObjectAnimator.OfFloat(this, "phaseY", 0f, 1f);
            animatorY.SetInterpolator(new AnimationInterpolator(easing));
            animatorY.SetDuration(duration);

            return animatorY;
        }

        public void Animate(long xAxisDuration, long yAxisDuration, EasingFunction easingX, EasingFunction easingY)
        {
            var xAnimator = XAnimator(xAxisDuration, easingX);
            var yAnimator = YAnimator(yAxisDuration, easingY);

            if (xAxisDuration > yAxisDuration)
            {
                xAnimator.AddUpdateListener(Delegate);
            }
            else
            {
                yAnimator.AddUpdateListener(Delegate);
            }

            xAnimator.Start();
            yAnimator.Start();
        }

        public void AnimateX(long xAxisDuration, EasingFunction easing)
        {
            ObjectAnimator animatorX = XAnimator(xAxisDuration, easing);
            animatorX.AddUpdateListener(Delegate);
            animatorX.Start();
        }

        public void AnimateY(long yAxisDuration, EasingFunction easing)
        {
            ObjectAnimator animatorY = YAnimator(yAxisDuration, easing);
            animatorY.AddUpdateListener(Delegate);
            animatorY.Start();
        }
    }

    internal class AnimationInterpolator : Java.Lang.Object, ITimeInterpolator
    {
        private readonly EasingFunction function;

        public AnimationInterpolator(EasingFunction function)
        {
            this.function = function;
        }

        public float GetInterpolation(float input)
        {
            return function(input);
        }
    }
}
