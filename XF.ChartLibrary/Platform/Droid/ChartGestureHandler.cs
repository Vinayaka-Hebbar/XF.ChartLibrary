using Android.Views;
using System;

namespace XF.ChartLibrary.Gestures
{
    internal class TapGestureHandler : GestureDetector.SimpleOnGestureListener
    {
        private readonly TapEvent tapEvent;

        private readonly ChartGesture gesture;

        private readonly View view;

        private bool _isScrolling;

        private static float s_displayDensity = float.MinValue;

        public TapGestureHandler(ChartGesture gesture, View view)
        {
            this.gesture = gesture;
            this.view = view;
            if (s_displayDensity == float.MinValue)
            {
                using var metrics = view.Context.Resources.DisplayMetrics;
                s_displayDensity = metrics.Density;
            }
            tapEvent = new TapEvent();
        }

        public override bool OnDown(MotionEvent e)
        {
            OnTap(e, TapAction.Entered);
            return true;
        }

        public override bool OnSingleTapConfirmed(MotionEvent e)
        {
            DisableScroll(view);
            OnTap(e, TapAction.Pressed);
            return true;
        }

        public override bool OnScroll(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
        {
            if (_isScrolling == false)
            {
                gesture.OnPan(Xamarin.Forms.GestureStatus.Started, distanceX / s_displayDensity, distanceY / s_displayDensity);
                _isScrolling = true;
                DisableScroll(view);
            }
            gesture.OnPan(Xamarin.Forms.GestureStatus.Running, distanceX / s_displayDensity, distanceY / s_displayDensity);
            return true;
        }

        public override bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
        {
            gesture.OnPan(Xamarin.Forms.GestureStatus.Completed, velocityX / s_displayDensity, velocityY / s_displayDensity);
            EnableScroll(view);
            return true;
        }

        private void OnTap(MotionEvent e, TapAction action)
        {
            tapEvent.Action = action;
            tapEvent.X = e.GetX();
            tapEvent.Y = e.GetY();
            tapEvent.XDist = Math.Abs(e.GetX(0) - e.GetX(1));
            tapEvent.YDist = Math.Abs(e.GetY(0) - e.GetY(1));
            gesture.OnTap(tapEvent);
        }

        public override bool OnDoubleTap(MotionEvent e)
        {
            gesture.OnDoubleTap(e.GetX(), e.GetY());
            return base.OnDoubleTap(e);
        }

        void DisableScroll(View v)
        {
            var parent = v.Parent;
            if (parent != null)
                parent.RequestDisallowInterceptTouchEvent(true);
        }

        void EnableScroll(View v)
        {
            var parent = v.Parent;
            if (parent != null)
                parent.RequestDisallowInterceptTouchEvent(false);
        }
    }

}
