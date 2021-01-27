using Android.Animation;
using Android.Content;
using Android.Views;

namespace XF.ChartLibrary.Charts
{
    public abstract partial class ChartBase<TData, TDataSet> : View, Android.Animation.ValueAnimator.IAnimatorUpdateListener
    {
        protected ChartBase(Context context) : base(context)
        {
        }

        public void OnAnimationUpdate(ValueAnimator animation)
        {
            PostInvalidate();
        }
    }
}
