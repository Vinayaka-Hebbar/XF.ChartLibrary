using Android.Views;
using SkiaSharp;
using System;

namespace XF.ChartLibrary.Gestures
{
    partial class ChartGestureRecognizer : GestureDetector.SimpleOnGestureListener, View.IOnTouchListener
    {
        private readonly PinchEvent pinchEvent;
        private readonly PanEvent panEvent;
        private readonly TapEvent tapEvent;

        private SKPoint mid;

        private float MinScalePointerDistance;
        private float DragTriggerDist;

        private VelocityTracker velocityTracker;

        public ChartGestureRecognizer()
        {
            pinchEvent = new PinchEvent();
            panEvent = new PanEvent();
            tapEvent = new TapEvent();
            MinScalePointerDistance = 3.5f.DpToPixel();
            DragTriggerDist = 3f.DpToPixel();
        }

        public bool NotInUse
        {
            get => pinchEvent.Mode == PinchState.None && panEvent.Mode == PanState.None;
        }

        public bool OnTouch(View v, MotionEvent e)
        {
            if (velocityTracker == null)
            {
                velocityTracker = VelocityTracker.Obtain();
            }
            velocityTracker.AddMovement(e);
            switch (e.ActionMasked)
            {
                case MotionEventActions.Down:
                    tapEvent.x = e.GetX();
                    tapEvent.y = e.GetY();
                    OnTap(tapEvent);
                    break;
                case MotionEventActions.PointerDown when e.PointerCount > 1:
                    DisableScroll(v);
                    var x = e.GetX();
                    var y = e.GetY();
                    pinchEvent.state = TouchState.Started;
                    float x1 = e.GetX(1);
                    float y1 = e.GetY(1);
                    float distX = x - x1;
                    float distY = y - y1;
                    pinchEvent.xDist = Math.Abs(distX);
                    pinchEvent.yDist = Math.Abs(distY);
                    pinchEvent.Spacing = Spacing(distX, distY);
                    if (pinchEvent.Spacing > 10)
                    {
                        mid.X = (x + x1) / 2;
                        mid.Y = (y + y1) / 2;
                        OnPinch(pinchEvent, x, y);
                    }
                    break;
                case MotionEventActions.Move:
                    if (panEvent.Mode == PanState.Drag)
                    {
                        DisableScroll(v);
                        panEvent.state = TouchState.Running;
                        panEvent.x = e.GetX();
                        panEvent.y = e.GetY();
                        OnPan(panEvent, panEvent.x - tapEvent.x, panEvent.y - tapEvent.y);
                    }
                    else if (pinchEvent.IsZooming)
                    {
                        DisableScroll(v);
                        if (e.PointerCount > 1)
                        {
                            pinchEvent.state = TouchState.Running;
                            distX = e.GetX(0) - e.GetX(1);
                            distY = e.GetY(0) - e.GetY(1);
                            var totalDist = MathF.Sqrt(distX * distX + distY * distY);
                            if (totalDist > MinScalePointerDistance)
                            {
                                if (pinchEvent.Mode == PinchState.PinchZoom)
                                {
                                    pinchEvent.Scale = totalDist / pinchEvent.Spacing;
                                }
                                else if (pinchEvent.Mode == PinchState.XZoom)
                                {
                                    pinchEvent.Scale = Math.Abs(distX) / pinchEvent.xDist;
                                }
                                else
                                {
                                    pinchEvent.Scale = Math.Abs(distY) / pinchEvent.yDist;
                                }

                                OnPinch(pinchEvent, mid.X, mid.Y);
                            }
                        }
                    }
                    else if (panEvent.Mode == PanState.None)
                    {
                        panEvent.state = TouchState.Started;
                        panEvent.x = e.GetX();
                        panEvent.y = e.GetY();
                        var distanceX = panEvent.x - tapEvent.x;
                        var distanceY = panEvent.y - tapEvent.y;
                        if (Math.Abs(Spacing(distanceX, distanceY)) > DragTriggerDist)
                        {
                            OnPan(panEvent, distanceY, distanceY);
                        }
                    }
                    break;
                case MotionEventActions.Up:
                    var tracker = velocityTracker;
                    int pointerId = e.GetPointerId(0);
                    tracker.ComputeCurrentVelocity(1000, ChartUtil.MaxFlingVelocity);
                    var velocityX = tracker.GetXVelocity(pointerId);
                    var velocityY = tracker.GetYVelocity(pointerId);
                    if (Math.Abs(velocityX) > ChartUtil.MinimumFlingVelocity ||
                        Math.Abs(velocityY) > ChartUtil.MinimumFlingVelocity)
                    {
                        if (panEvent.Mode == PanState.Drag)
                        {
                            panEvent.state = TouchState.Completed;
                            panEvent.x = e.GetX();
                            panEvent.y = e.GetY();
                            panEvent.velocityX = velocityX;
                            panEvent.velocityY = velocityY;
                            OnPan(panEvent, 0, 0);
                        }
                    }
                    else if (pinchEvent.IsZooming)
                    {
                        pinchEvent.state = TouchState.Completed;
                        OnPinch(pinchEvent, e.GetX(), e.GetY());
                    }
                    pinchEvent.Mode = PinchState.None;
                    panEvent.Mode = PanState.None;
                    EnableScroll(v);
                    if (velocityTracker != null)
                    {
                        velocityTracker.Recycle();
                        velocityTracker = null;
                    }
                    break;
                case MotionEventActions.PointerUp:
                    VelocityTrackerCleanUpIfNeeded(e, velocityTracker);
                    break;
                case MotionEventActions.Cancel:
                    pinchEvent.Reset();
                    panEvent.Reset();
                    if (velocityTracker != null)
                    {
                        velocityTracker.Recycle();
                        velocityTracker = null;
                    }
                    break;

            }
            return true;
        }

        static void VelocityTrackerCleanUpIfNeeded(MotionEvent e, VelocityTracker tracker)
        {
            tracker.ComputeCurrentVelocity(1000, ChartUtil.MaxFlingVelocity);
            var upIndex = e.ActionIndex;
            int pointerId = e.GetPointerId(upIndex);
            var velocityX = tracker.GetXVelocity(pointerId);
            var velocityY = tracker.GetYVelocity(pointerId);
            for (int i = 0, count = e.PointerCount; i < count; i++)
            {
                if (i == upIndex)
                    continue;
                var id = e.GetPointerId(i);
                var x = velocityX * tracker.GetXVelocity(id);
                var y = velocityY * tracker.GetYVelocity(id);
                var dot = x + y;
                if (dot < 0)
                {
                    tracker.Clear();
                    break;
                }
            }
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

        float Spacing(float x, float y)
        {
            return MathF.Sqrt(x * x + y * y);
        }

        public override bool OnDoubleTap(MotionEvent e)
        {
            OnDoubleTap(e.GetX(), e.GetY());
            return true;
        }

        protected override void Dispose(bool disposing)
        {
            if (velocityTracker != null)
            {
                velocityTracker.Recycle();
                velocityTracker = null;
            }
            base.Dispose(disposing);
        }
    }
}
