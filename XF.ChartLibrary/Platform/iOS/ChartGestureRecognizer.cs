using Foundation;
using System;
using UIKit;

namespace XF.ChartLibrary.Gestures
{
    partial class ChartGestureRecognizer : NSObject, IUIGestureRecognizerDelegate
    {
        private readonly PinchEvent pinch;
        private readonly PanEvent pan;

        private UIScrollView outerScrollView;

        private readonly UITapGestureRecognizer doubleTapGestureRecognizer;
        private readonly UIPanGestureRecognizer panGestureRecognizer;
        private readonly UITapGestureRecognizer tapGestureRecognizer;
        private readonly UIPinchGestureRecognizer pinchGestureRecognizer;

        public ChartGestureRecognizer()
        {
            tapGestureRecognizer = new UITapGestureRecognizer();
            pinchGestureRecognizer = new UIPinchGestureRecognizer()
            {
                Delegate = this,
                ShouldRecognizeSimultaneously = GeestureRecognize
            };
            doubleTapGestureRecognizer = new UITapGestureRecognizer
            {
                NumberOfTapsRequired = 2
            };
            panGestureRecognizer = new UIPanGestureRecognizer()
            {
                Delegate = this,
                ShouldRecognizeSimultaneously = GeestureRecognize,
            };
            pinch = new PinchEvent();
            pan = new PanEvent();
        }

        bool GeestureRecognize(UIGestureRecognizer gestureRecognizer, UIGestureRecognizer otherGestureRecognizer)
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
                var scrollView = view.Superview;
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
                    if (foundScrollView?.ScrollEnabled == false)
                    {
                        foundScrollView = null;
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

        private UIGestureRecognizer.Token tapToken;
        private UIGestureRecognizer.Token pinchToken;
        private UIGestureRecognizer.Token doubleTapToken;
        private UIGestureRecognizer.Token panToken;

        private UIView view;

        public void OnInitialize(UIView view)
        {
            this.view = view;
            tapToken = tapGestureRecognizer.AddTarget(HandleTap);
            pinchToken = pinchGestureRecognizer.AddTarget(HandlePinch);
            doubleTapToken = doubleTapGestureRecognizer.AddTarget(HandleDoubleTap);
            panToken = panGestureRecognizer.AddTarget(HandlePan);
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
                pan.state = TouchState.Started;
                var location = recognizer.LocationOfTouch(0, view);
                pan.x = (float)location.X;
                pan.y = (float)location.Y;
                var translation = recognizer.TranslationInView(view);
                OnPan(pan, (float)translation.X, (float)translation.Y);
                if (pan.Mode == PanState.Drag && outerScrollView != null)
                {
                    outerScrollView = null;
                    pan.Mode = PanState.None;
                }
                else if (outerScrollView != null)
                {
                    outerScrollView.ScrollEnabled = false;
                }
            }
            else if (recognizer.State == UIGestureRecognizerState.Changed && pan.Mode == PanState.Drag)
            {
                pan.state = TouchState.Running;
                var location = recognizer.LocationOfTouch(0, view);
                pan.x = (float)location.X;
                pan.y = (float)location.Y;
                var translation = recognizer.TranslationInView(view);
                OnPan(pan, (float)translation.X, (float)translation.Y);
            }
            else if (recognizer.State == UIGestureRecognizerState.Ended || recognizer.State == UIGestureRecognizerState.Cancelled)
            {
                if (pan.Mode == PanState.Drag)
                {
                    if (recognizer.State == UIGestureRecognizerState.Ended)
                    {
                        pan.state = TouchState.Completed;
                        var velocity = recognizer.VelocityInView(view);
                        pan.velocityX = (float)velocity.X;
                        pan.velocityY = (float)velocity.Y;
                        OnPan(pan, 0, 0);
                    }
                    pan.Mode = PanState.None;
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
                var location = doubleTapGestureRecognizer.LocationInView(view);
                OnDoubleTap((float)location.X, (float)location.Y);
            }
        }

        void HandlePinch()
        {
            var recognizer = pinchGestureRecognizer;
            switch (recognizer.State)
            {
                case UIGestureRecognizerState.Began:
                    pinch.state = TouchState.Started;
                    var location = recognizer.LocationInView(view);
                    var locationInTouch = recognizer.LocationOfTouch(1, view);
                    pinch.xDist = Math.Abs((float)(location.X - locationInTouch.X));
                    pinch.yDist = Math.Abs((float)(location.Y - locationInTouch.Y));
                    OnPinch(pinch, pinch.xDist, pinch.yDist);
                    break;

                case UIGestureRecognizerState.Changed:
                    pinch.state = TouchState.Running;
                    location = recognizer.LocationInView(view);
                    OnPinch(pinch, (float)location.X, (float)location.Y);
                    break;
                case UIGestureRecognizerState.Cancelled:
                case UIGestureRecognizerState.Ended:
                    pinch.state = TouchState.Completed;
                    OnPinch(pinch, 0, 0);
                    pinch.Mode = PinchState.None;
                    break;
            }
        }

        void HandleTap()
        {
            var recognizer = tapGestureRecognizer;
            if (recognizer.State == UIGestureRecognizerState.Ended)
            {
                var location = recognizer.LocationInView(view);
                OnTap((float)location.X, (float)location.Y);
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
            if (view != null)
            {
                view.RemoveGestureRecognizer(tapGestureRecognizer);
                view.RemoveGestureRecognizer(pinchGestureRecognizer);
                view.RemoveGestureRecognizer(doubleTapGestureRecognizer);
                view.RemoveGestureRecognizer(panGestureRecognizer);
            }
            base.Dispose(disposing);
        }
    }
}
