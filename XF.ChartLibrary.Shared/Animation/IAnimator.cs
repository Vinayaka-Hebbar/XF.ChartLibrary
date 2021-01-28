namespace XF.ChartLibrary.Animation
{
    public interface IAnimator
#if __ANDROID__
: Android.Animation.ValueAnimator.IAnimatorUpdateListener { }
#else
    {
        void AnimatorStopped(Animator animator);
        void AnimatorUpdated(Animator animator);
    }
#endif
}