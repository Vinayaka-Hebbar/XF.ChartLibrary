using Foundation;
using System;
using System.Linq;
using UIKit;
using XF.ChartLibrary.Platform.iOS;

namespace XF.ChartLibrary.Gestures
{
    partial class PieRadarChartGesture
    {

        private TapEvent tap;

        private readonly UIGestureRecognizer tapGesture;

        private readonly UIGestureRecognizer touchGesture;

        private readonly UIGestureRecognizer.Token tapToken;

        public PieRadarChartGesture()
        {
            tapGesture = new UITapGestureRecognizer();
            touchGesture = new TouchGesture(this);
            tapToken = tapGesture.AddTarget(HandleTap);
        }

        private void HandleTap()
        {
            var recognizer = tapGesture;
            if (recognizer.State == UIGestureRecognizerState.Ended)
            {
                var location = recognizer.LocationInView(View);
                tap.state = TouchState.Ended;
                tap.x = (float)(location.X * Scale);
                tap.y = (float)(location.Y * Scale);
                OnTap(tap);
            }
            else if (recognizer.State == UIGestureRecognizerState.Began)
            {
                var location = recognizer.LocationInView(View);
                tap.state = TouchState.Begin;
                tap.x = (float)(location.X * Scale);
                tap.y = (float)(location.Y * Scale);
                OnTap(tap);
            }
        }

        public override void Attach(UIView view)
        {
            view.AddGestureRecognizer(tapGesture);
            view.AddGestureRecognizer(touchGesture);
        }

        public override void Clear()
        {
            if (View != null)
            {
                Detach(View);
                View = null;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (tapToken != null)
            {
                tapGesture.RemoveTarget(tapToken);
            }
            Clear();
            base.Dispose(disposing);
        }

        public override void Detach(UIView view)
        {
            view.RemoveGestureRecognizer(tapGesture);
            view.RemoveGestureRecognizer(touchGesture);
        }

    }

    class TouchGesture : UIGestureRecognizer
    {
        private readonly PieRadarChartGesture gesture;

        private readonly TouchEvent touchEvent;

        public TouchGesture(PieRadarChartGesture gesture)
        {
            this.gesture = gesture;
            touchEvent = new TouchEvent();
        }

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            var first = (UITouch)touches.FirstOrDefault();
            if (first != null)
            {
                touchEvent.state = TouchState.Begin;
                touchEvent.SetLocation(first.LocationInView(gesture.View), gesture.Scale);
                gesture.OnTouch(touchEvent);
            }

            if (touchEvent.mode == GestureMode.None)
            {
                base.TouchesBegan(touches, evt);
            }
        }

        public override void TouchesMoved(NSSet touches, UIEvent evt)
        {
            var first = (UITouch)touches.FirstOrDefault();
            if (first != null)
            {
                touchEvent.state = TouchState.Changed;
                touchEvent.SetLocation(first.LocationInView(gesture.View), gesture.Scale);
                gesture.OnTouch(touchEvent);
            }
            if (touchEvent.mode == GestureMode.None)
            {
                base.TouchesBegan(touches, evt);
            }
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            if (touchEvent.mode == GestureMode.None)
            {
                base.TouchesEnded(touches, evt);
            }
            var first = (UITouch)touches.FirstOrDefault();
            if (first != null)
            {
                touchEvent.state = TouchState.Ended;
                touchEvent.SetLocation(first.LocationInView(gesture.View), gesture.Scale);
                gesture.OnTouch(touchEvent);
            }
            touchEvent.mode = GestureMode.None;
        }
    }
}
