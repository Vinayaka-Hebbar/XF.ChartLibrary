using Android.OS;
using Android.Views;
using SkiaSharp;
using System;

namespace XF.ChartLibrary.Gestures
{
    partial class ChartGestureRecognizer : GestureDetector.SimpleOnGestureListener, View.IOnTouchListener
    {
        internal const int ShowPress = 1;
        internal const int LongPress = 2;
        internal const int Click = 3;

        private const MotionEventFlags GeneratedGesture = (MotionEventFlags)0x8;

        private readonly Handler handler;
        private bool isDoubleTapping;
        private bool alwaysInBiggerTapRegion;
        private bool alwaysInTapRegion;
        private bool inLongPress;
        internal bool stillDown;

        private bool isLongpressEnabled;

        private static readonly int TAP_TIMEOUT = ViewConfiguration.TapTimeout;
        private static readonly int DOUBLE_TAP_TIMEOUT = ViewConfiguration.DoubleTapTimeout;
        private static readonly int DOUBLE_TAP_MIN_TIME = 40;

        private readonly PinchEvent pinchEvent;
        private readonly PanEvent panEvent;
        private SKPoint touchStart;

        private MotionEvent currentDownEvent;
        private MotionEvent previousUpEvent;

        private SKPoint mid;

        private float MinScalePointerDistance;
        private float DragTriggerDist;
        private int DoubleTapSlopSquare;

        private VelocityTracker velocityTracker;

        public bool IsLongPressEnabled
        {
            get => isLongpressEnabled;
            set
            {
                isLongpressEnabled = value;
                OnPropertyChanged(nameof(IsLongPressEnabled));
            }
        }

        public ChartGestureRecognizer()
        {
            pinchEvent = new PinchEvent();
            panEvent = new PanEvent();
            handler = new GestureHandler(this);
            isLongpressEnabled = true;
        }

        public void OnInitialize(View view)
        {
            // Fallback to support pre-donuts releases
            ViewConfiguration configuration = ViewConfiguration.Get(view.Context);
            var doubleTapSlop = configuration.ScaledDoubleTapSlop;
            DoubleTapSlopSquare = doubleTapSlop * doubleTapSlop;
            DragTriggerDist = 3f.DpToPixel();
            MinScalePointerDistance = configuration.ScaledTouchSlop;
        }

        public bool OnTouch(View v, MotionEvent e)
        {
            if (velocityTracker == null)
            {
                velocityTracker = VelocityTracker.Obtain();
            }
            velocityTracker.AddMovement(e);
            bool handled = true;
            switch (e.ActionMasked)
            {
                case MotionEventActions.Down:
                    touchStart.X = e.GetX();
                    touchStart.Y = e.GetY();
                    bool hadTapMessage = handler.HasMessages(Click);
                    if (hadTapMessage)
                        handler.RemoveMessages(Click);
                    if ((currentDownEvent != null) && (previousUpEvent != null)
                            && hadTapMessage
                            && IsConsideredDoubleTap(currentDownEvent, previousUpEvent, e))
                    {
                        // This is a second tap
                        isDoubleTapping = true;
                        // Give a callback with the first tap of the double-tap
                        handled = OnDoubleTap(currentDownEvent);
                    }
                    else
                    {
                        // This is a first tap
                        handler.SendEmptyMessageDelayed(Click, DOUBLE_TAP_TIMEOUT);
                    }
                    if (currentDownEvent != null)
                    {
                        currentDownEvent.Recycle();
                    }
                    currentDownEvent = MotionEvent.Obtain(e);
                    alwaysInTapRegion = true;
                    alwaysInBiggerTapRegion = true;
                    stillDown = true;
                    inLongPress = false;

                    if (isLongpressEnabled)
                    {
                        handler.RemoveMessages(LongPress);
                        handler.SendMessageAtTime(
                                handler.ObtainMessage(
                                        LongPress,
                                        3,
                                        0 /* arg2 */),
                                currentDownEvent.DownTime
                                        + ViewConfiguration.LongPressTimeout);
                    }
                    handler.SendEmptyMessageAtTime(ShowPress,
                           currentDownEvent.DownTime + TAP_TIMEOUT);
                    break;
                case MotionEventActions.PointerDown when e.PointerCount > 1:
                    DisableScroll(v);
                    float x, y;
                    touchStart.X = x = e.GetX();
                    touchStart.Y = y = e.GetY();
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
                        OnPinch(pinchEvent, x, y);
                        ClearTaps();
                    }
                    mid.X = (x + x1) / 2;
                    mid.Y = (y + y1) / 2;
                    break;
                case MotionEventActions.PointerDown:
                    handler.RemoveMessages(ShowPress);
                    handler.RemoveMessages(LongPress);
                    handler.RemoveMessages(Click);
                    isDoubleTapping = false;
                    alwaysInTapRegion = false;
                    alwaysInBiggerTapRegion = false;
                    inLongPress = false;
                    stillDown = false;
                    break;
                case MotionEventActions.Move:
                    if (panEvent.Mode == PanState.Drag)
                    {
                        DisableScroll(v);
                        panEvent.state = TouchState.Running;
                        panEvent.x = e.GetX();
                        panEvent.y = e.GetY();
                        OnPan(panEvent, panEvent.x - touchStart.X, panEvent.y - touchStart.Y);
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
                        var distanceX = panEvent.x - touchStart.X;
                        var distanceY = panEvent.y - touchStart.Y;
                        if (Math.Abs(Spacing(distanceX, distanceY)) > DragTriggerDist)
                        {
                            OnPan(panEvent, distanceY, distanceY);
                            ClearTaps();
                        }
                    }
                    break;
                case MotionEventActions.Up:
                    stillDown = false;
                    MotionEvent currentUpEvent = MotionEvent.Obtain(e);
                    if (isDoubleTapping)
                    {
                        handled = false;
                    }
                    else if (inLongPress)
                    {
                        handler.RemoveMessages(Click);
                        inLongPress = false;
                    }
                    else if (alwaysInTapRegion)
                    {
                        handled = OnSingleTapUp(e);
                    }
                    else
                    {
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
                    }

                    pinchEvent.Mode = PinchState.None;
                    panEvent.Mode = PanState.None;
                    EnableScroll(v);
                    if (previousUpEvent != null)
                    {
                        previousUpEvent.Recycle();
                    }
                    if (velocityTracker != null)
                    {
                        velocityTracker.Recycle();
                        velocityTracker = null;
                    }
                    // Hold the event we obtained above - listeners may have changed the original.
                    previousUpEvent = currentUpEvent;
                    isDoubleTapping = false;
                    handler.RemoveMessages(ShowPress);
                    handler.RemoveMessages(LongPress);
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
                    handler.RemoveMessages(Click);
                    handler.RemoveMessages(ShowPress);
                    handler.RemoveMessages(LongPress);
                    isDoubleTapping = false;
                    alwaysInTapRegion = false;
                    alwaysInBiggerTapRegion = false;
                    inLongPress = false;
                    stillDown = false;
                    break;

            }
            return handled;
        }

        void ClearTaps()
        {
            alwaysInTapRegion = false;
            handler.RemoveMessages(Click);
            handler.RemoveMessages(ShowPress);
            handler.RemoveMessages(LongPress);
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

        bool IsConsideredDoubleTap(MotionEvent firstDown, MotionEvent firstUp,
           MotionEvent secondDown)
        {
            if (!alwaysInBiggerTapRegion)
            {
                return false;
            }

            long deltaTime = secondDown.EventTime - firstUp.EventTime;
            if (deltaTime > DOUBLE_TAP_TIMEOUT || deltaTime < DOUBLE_TAP_MIN_TIME)
            {
                return false;
            }

            int deltaX = (int)firstDown.GetX() - (int)secondDown.GetX();
            int deltaY = (int)firstDown.GetY() - (int)secondDown.GetY();
            var isGeneratedGesture =
                    (firstDown.Flags & GeneratedGesture) != 0;
            int slopSquare = isGeneratedGesture ? 0 : DoubleTapSlopSquare;
            return (deltaX * deltaX + deltaY * deltaY < slopSquare);
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

        public override bool OnSingleTapUp(MotionEvent e)
        {
            OnTap(e.GetX(), e.GetY());
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

        internal void DispatchLongPress()
        {
            handler.RemoveMessages(Click);
            inLongPress = true;
            OnLongPress(currentDownEvent);
        }
    }

    internal class GestureHandler : Handler
    {
        private readonly ChartGestureRecognizer gesture;

        internal GestureHandler(ChartGestureRecognizer gesture)
        {
            this.gesture = gesture;
        }

        public override void HandleMessage(Message msg)
        {
            switch (msg.What)
            {
                case ChartGestureRecognizer.LongPress:
                    gesture.DispatchLongPress();
                    break;
            }
        }
    }

}
