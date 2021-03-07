using Android.Animation;

namespace XF.ChartLibrary.Animation
{
    partial class Ticker : Java.Lang.Object, ValueAnimator.IAnimatorUpdateListener, Android.Animation.Animator.IAnimatorListener
    {
        ValueAnimator _val;

        public void OnAnimationCancel(Android.Animation.Animator animation)
        {
            OnStop();
        }

        public void OnAnimationEnd(Android.Animation.Animator animation)
        {
            OnStop();
        }

        public void OnAnimationRepeat(Android.Animation.Animator animation)
        {
        }

        public void OnAnimationStart(Android.Animation.Animator animation)
        {
        }

        public void OnAnimationUpdate(ValueAnimator animation)
        {
            OnUpdate(animation.CurrentPlayTime);
        }

        partial void OnCancel()
        {
            if(_val != null)
            {
                _val.Cancel();
                _val = null;
            }
        }

        partial void OnStart()
        {
            _val = ValueAnimator.OfFloat(0f,1f);
            _val.SetDuration(duration);
            _val.AddUpdateListener(this);
            _val.AddListener(this);
            _val.Start();
        }
    }
}
