using System;
using UIKit;

namespace XF.ChartLibrary.Gestures
{
    partial class BarLineChartGesture 
    {
        private readonly PinchEvent pinch;
        private readonly PanEvent pan;
        private TapEvent tap;

        private UIScrollView outerScrollView;

        private readonly UITapGestureRecognizer doubleTapGestureRecognizer;
        private readonly UIPanGestureRecognizer panGestureRecognizer;
        private readonly UITapGestureRecognizer tapGestureRecognizer;
        private readonly UIPinchGestureRecognizer pinchGestureRecognizer;


        private readonly UIGestureRecognizer.Token tapToken;
        private readonly UIGestureRecognizer.Token pinchToken;
        private readonly UIGestureRecognizer.Token doubleTapToken;
        private readonly UIGestureRecognizer.Token panToken;

        public BarLineChartGesture()
        {
            tapGestureRecognizer = new UITapGestureRecognizer();
            pinchGestureRecognizer = new UIPinchGestureRecognizer()
            {
                ShouldRecognizeSimultaneously = GestureRecognize
            };
            doubleTapGestureRecognizer = new UITapGestureRecognizer
            {
                NumberOfTapsRequired = 2
            };
            panGestureRecognizer = new UIPanGestureRecognizer()
            {
                ShouldRecognizeSimultaneously = GestureRecognize,
            };
            tapToken = tapGestureRecognizer.AddTarget(HandleTap);
            pinchToken = pinchGestureRecognizer.AddTarget(HandlePinch);
            doubleTapToken = doubleTapGestureRecognizer.AddTarget(HandleDoubleTap);
            panToken = panGestureRecognizer.AddTarget(HandlePan);
            pinch = new PinchEvent();
            pan = new PanEvent();
        }

        bool GestureRecognize(UIGestureRecognizer gestureRecognizer, UIGestureRecognizer otherGestureRecognizer)
        {
#if !__TVOS__
            if ((gestureRecognizer is UIPinchGestureRecognizer && otherGestureRecognizer is UIPanGestureRecognizer) ||
                (gestureRecognizer is UIPanGestureRecognizer && otherGestureRecognizer is UIPinchGestureRecognizer))
            {
                return true;
            }
#endif
            if (gestureRecognizer is UIPanGestureRecognizer &&
            otherGestureRecognizer is UIPanGestureRecognizer &&
            gestureRecognizer == panGestureRecognizer)
            {
                var scrollView = View.Superview;
                while (scrollView != null && !(scrollView is UIScrollView))
                {
                    scrollView = scrollView.Superview;
                }

                // If there is two scrollview together, we pick the superview of the inner scrollview.
                // In the case of UITableViewWrepperView, the superview will be UITableView
                if (scrollView?.Superview is UIScrollView superViewOfScrollView)
                {
                    scrollView = superViewOfScrollView;
                }

                if (scrollView is UIScrollView foundScrollView)
                {
                    if (foundScrollView.ScrollEnabled == false)
                    {
                        foundScrollView = null;
                        return false;
                    }

                    var scrollViewPanGestureRecognizer = Array.Find(foundScrollView.GestureRecognizers, g => g is UIPanGestureRecognizer);

                    if (otherGestureRecognizer == scrollViewPanGestureRecognizer)
                    {
                        outerScrollView = foundScrollView;
                        return true;
                    }
                }
            }
            return false;
        }

        public override void Attach(UIView view)
        {
            view.AddGestureRecognizer(tapGestureRecognizer);
            view.AddGestureRecognizer(pinchGestureRecognizer);
            view.AddGestureRecognizer(doubleTapGestureRecognizer);
            view.AddGestureRecognizer(panGestureRecognizer);
        }


        void HandlePan()
        {
            var recognizer = panGestureRecognizer;
            if (recognizer.State == UIGestureRecognizerState.Began && recognizer.NumberOfTouches > 0)
            {
                pan.state = TouchState.Begin;
                var location = recognizer.LocationOfTouch(0, View);
                pan.x = (float)(location.X * Scale);
                pan.y = (float)(location.Y * Scale);
                var translation = recognizer.TranslationInView(View);
                OnPan(pan, (float)(translation.X * Scale), (float)(translation.Y * Scale));
                // user drag
                if (pan.mode == PanMode.Drag && outerScrollView != null)
                {
                    outerScrollView = null;
                    pan.mode = PanMode.None;
                }
                else if (outerScrollView != null)
                {
                    outerScrollView.ScrollEnabled = false;
                }
            }
            else if (recognizer.State == UIGestureRecognizerState.Changed && pan.mode == PanMode.Drag)
            {
                pan.state = TouchState.Changed;
                var location = recognizer.LocationOfTouch(0, View);
                pan.x = (float)(location.X * Scale);
                pan.y = (float)(location.Y * Scale);
                var translation = recognizer.TranslationInView(View);
                OnPan(pan, (float)(translation.X * Scale), (float)(translation.Y * Scale));
            }
            else if (recognizer.State == UIGestureRecognizerState.Ended || recognizer.State == UIGestureRecognizerState.Cancelled)
            {
                if (pan.mode == PanMode.Drag)
                {
                    if (recognizer.State == UIGestureRecognizerState.Ended)
                    {
                        pan.state = TouchState.Ended;
                        var velocity = recognizer.VelocityInView(View);
                        pan.velocityX = (float)velocity.X;
                        pan.velocityY = (float)velocity.Y;
                        OnPan(pan, 0, 0);
                    }
                    pan.mode = PanMode.None;
                }
                if (outerScrollView != null)
                {
                    outerScrollView.ScrollEnabled = true;
                    outerScrollView = null;
                }
            }
        }

        void HandleDoubleTap()
        {
            if (doubleTapGestureRecognizer.State == UIGestureRecognizerState.Ended)
            {
                var location = doubleTapGestureRecognizer.LocationInView(View);
                OnDoubleTap((float)(location.X * Scale), (float)(location.Y * Scale));
            }
        }

        void HandlePinch()
        {
            var recognizer = pinchGestureRecognizer;
            switch (recognizer.State)
            {
                case UIGestureRecognizerState.Began:
                    pinch.state = TouchState.Begin;
                    var location = recognizer.LocationInView(View);
                    var locationInTouch = recognizer.LocationOfTouch(1, View);
                    pinch.xDist = Math.Abs((float)((location.X - locationInTouch.X) * Scale));
                    pinch.yDist = Math.Abs((float)((location.Y - locationInTouch.Y) * Scale));
                    OnPinch(pinch, pinch.xDist, pinch.yDist);
                    break;

                case UIGestureRecognizerState.Changed:
                    pinch.state = TouchState.Changed;
                    location = recognizer.LocationInView(View);
                    OnPinch(pinch, (float)(location.X * Scale), (float)(location.Y * Scale));
                    break;
                case UIGestureRecognizerState.Cancelled:
                case UIGestureRecognizerState.Ended:
                    pinch.state = TouchState.Ended;
                    OnPinch(pinch, 0, 0);
                    pinch.mode = PinchMode.None;
                    break;
            }
        }

        void HandleTap()
        {
            var recognizer = tapGestureRecognizer;
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

        protected override void Dispose(bool disposing)
        {
            if (tapToken != null)
            {
                tapGestureRecognizer.RemoveTarget(tapToken);
            }
            if (pinchToken != null)
            {
                pinchGestureRecognizer.RemoveTarget(pinchToken);
            }
            if (doubleTapToken != null)
            {
                doubleTapGestureRecognizer.RemoveTarget(doubleTapToken);
            }
            if (panToken != null)
            {
                panGestureRecognizer.RemoveTarget(panToken);
            }
            Clear();
            base.Dispose(disposing);
        }

        public override void Clear()
        {
            if (View != null)
            {
                Detach(View);
                View = null;
            }
        }

        public override void Detach(UIView view)
        {
            view.RemoveGestureRecognizer(tapGestureRecognizer);
            view.RemoveGestureRecognizer(pinchGestureRecognizer);
            view.RemoveGestureRecognizer(doubleTapGestureRecognizer);
            view.RemoveGestureRecognizer(panGestureRecognizer);
        }
    }
}
