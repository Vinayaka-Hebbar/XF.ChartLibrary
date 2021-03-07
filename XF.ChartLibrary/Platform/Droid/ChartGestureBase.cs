using Android.Views;

namespace XF.ChartLibrary.Gestures
{
    partial class ChartGestureBase : Java.Lang.Object, View.IOnTouchListener
    {
        public abstract void OnInitialize(View view);

        public abstract bool OnTouch(View view, MotionEvent e);


        protected float Spacing(float x, float y)
        {
            return System.MathF.Sqrt(x * x + y * y);
        }

        protected void DisableScroll(View v)
        {
            var parent = v.Parent;
            if (parent != null)
                parent.RequestDisallowInterceptTouchEvent(true);
        }

        protected void EnableScroll(View v)
        {
            var parent = v.Parent;
            if (parent != null)
                parent.RequestDisallowInterceptTouchEvent(false);
        }
    }
}
