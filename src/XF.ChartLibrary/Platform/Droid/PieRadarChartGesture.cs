using Android.Views;

namespace XF.ChartLibrary.Gestures
{
    partial class PieRadarChartGesture
    {
        private bool alwaysInTapRegion;

        private GestureMode lastState;

        private TapEvent tapEvent;

        private readonly TouchEvent touchEvent;

        public PieRadarChartGesture()
        {
            touchEvent = new TouchEvent();
        }

        public override void OnInitialize(View view)
        {
        }

        public override bool OnTouch(View v, MotionEvent e)
        {
            // double tap, pan, pinch, single tap features from android source code and MPChartAndroid

            bool handled = true;
            var x = e.GetX();
            var y = e.GetY();
            switch (e.ActionMasked)
            {
                case MotionEventActions.Down:
                    touchEvent.state = tapEvent.state = TouchState.Begin;
                    touchEvent.x = tapEvent.x =x;
                    touchEvent.y = tapEvent.y = y;
                    touchEvent.state = TouchState.Begin;
                    if (lastState == GestureMode.None)
                    {
                        alwaysInTapRegion = true;
                    }
                    OnTap(tapEvent);
                    OnTouch(touchEvent);
                    break;
                case MotionEventActions.Move:
                    touchEvent.state = TouchState.Changed;
                    touchEvent.x = x;
                    touchEvent.y = y;
                    OnTouch(touchEvent);
                    if (lastState == GestureMode.None && touchEvent.mode == GestureMode.Rotate)
                    {
                        DisableScroll(v);
                        // clear the tap action
                        alwaysInTapRegion = false;
                    }
                    lastState = touchEvent.mode;
                    break;
                case MotionEventActions.PointerDown:
                    alwaysInTapRegion = false;
                    break;
                case MotionEventActions.Up:
                    if (alwaysInTapRegion)
                    {
                        handled = OnSingleTapUp(e);
                    }
                    else
                    {
                        touchEvent.state = TouchState.Ended;
                        touchEvent.x = x;
                        touchEvent.y = y;
                        OnTouch(touchEvent);
                    }
                    touchEvent.mode = GestureMode.None;
                    lastState = GestureMode.None;
                    EnableScroll(v);
                    break;
                case MotionEventActions.Cancel:
                    alwaysInTapRegion = false;
                    break;

            }
            return handled;
        }

        public virtual bool OnSingleTapUp(MotionEvent e)
        {
            tapEvent.state = TouchState.Ended;
            tapEvent.x = e.GetX();
            tapEvent.y = e.GetY();
            OnTap(tapEvent);
            return true;
        }
    }
}
