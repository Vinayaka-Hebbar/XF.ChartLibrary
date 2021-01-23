using System;

namespace XF.ChartLibrary.Animation
{
    public partial class AnimatorDelegate : IAnimator
    {
        public virtual void AnimatorUpdated(Animator animator)
        {

        }
        public virtual void AnimatorStopped(Animator animator)
        {

        }
    }

    public partial class Animator
    {
        static readonly IAnimator Empty = new AnimatorDelegate();

        private WeakReference<IAnimator> _delegate;


        public IAnimator Delegate
        {
            get
            {
                if (_delegate == null || _delegate.TryGetTarget(out IAnimator animator) == false || animator == null)
                    return Empty;
                return animator;
            }
            set
            {
                if (_delegate == null)
                    _delegate = new WeakReference<IAnimator>(value);
                else
                    _delegate.SetTarget(value);
            }
        }

        public float PhaseX = 1.0f;

        public float PhaseY = 1.0f;

        public void Animate(long xAxisDuration, long yAxisDuration, EasingOption easingOptionX, EasingOption easingOptionY)
        {
            Animate(xAxisDuration: xAxisDuration, yAxisDuration: yAxisDuration, easingX: EasingFunctions.EasingFunctionFromOption(easingOptionX), easingY: EasingFunctions.EasingFunctionFromOption(easingOptionY));
        }
        public void Animate(long xAxisDuration, long yAxisDuration, EasingFunction easing)
        {
            Animate(xAxisDuration: xAxisDuration, yAxisDuration: yAxisDuration, easingX: easing, easingY: easing);
        }

        public void Animate(long xAxisDuration, long yAxisDuration, EasingOption easingOption = EasingOption.EaseInOutSine)
        {
            Animate(xAxisDuration: xAxisDuration, yAxisDuration: yAxisDuration, easing: EasingFunctions.EasingFunctionFromOption(easingOption));
        }

        public void AnimateX(long xAxisDuration, EasingOption easingOption = EasingOption.EaseInOutSine)
        {
            AnimateX(xAxisDuration: xAxisDuration, easing: EasingFunctions.EasingFunctionFromOption(easingOption));
        }

        public void AnimateY(long yAxisDuration, EasingOption easingOption = EasingOption.EaseInOutSine)
        {
            AnimateY(yAxisDuration: yAxisDuration, easing: EasingFunctions.EasingFunctionFromOption(easingOption));
        }
    }
}
