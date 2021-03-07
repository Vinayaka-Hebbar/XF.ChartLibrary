using SkiaSharp;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using XF.ChartLibrary.Gestures;

namespace XF.ChartLibrary.Charts
{
    partial class PieRadarChartBase<TData, TDataSet>
    {
        public static readonly BindableProperty RotationEnabledProperty = BindableProperty.Create(nameof(RotationEnabled), typeof(bool), typeof(PieRadarChartBase<TData, TDataSet>), defaultValue: true);

        public override IChartGesture Gesture { get; }

        private readonly IList<AngularVelocitySample> _velocitySamples;

        private float startAngle;

        private readonly float minRotationDistance = 8f.DpToPixel();

        private SKPoint touchStartPoint;

        private float decelerationAngularVelocity = 0.0f;

        private float decelerationLastTime;

        private Animation.Ticker delecelerationTimer;

        public PieRadarChartBase()
        {
            var gesture = new PieRadarChartGesture();
            gesture.Tap += OnTap;
            gesture.Touch += OnTouch;
            Gesture = gesture;
            GestureRecognizers.Add(gesture);
            _velocitySamples = new List<AngularVelocitySample>();
        }

        public long DecelerationDuration { get; set; } = 2500;
        /// <summary>
        /// If this is true then rotation / spinning of the chart by touch.
        /// Set it to false to disable it.Default: true
        /// </summary>
        public bool RotationEnabled
        {
            get => (bool)GetValue(RotationEnabledProperty);
            set => SetValue(RotationEnabledProperty, value);
        }

        void StopDeceleration()
        {
            if (delecelerationTimer != null)
            {
                delecelerationTimer.Cancel();
            }
        }

        private void OnTouch(TouchEvent e)
        {
            if (e.State == TouchState.Changed)
            {
                if (DragDecelerationEnabled)
                    SampleVelocity(e.X, e.Y);
                if (e.Mode == GestureMode.None && Distance(e, touchStartPoint) > minRotationDistance)
                {
                    e.Mode = GestureMode.Rotate;
                }
                else if (e.Mode == GestureMode.Rotate)
                {
                    RotationAngle = GetAngleForPoint(e.X, e.Y) - startAngle;
                    InvalidateSurface();
                }
            }
            else if (e.State == TouchState.Begin)
            {
                _velocitySamples.Clear();
                if (DragDecelerationEnabled)
                {
                    SampleVelocity(e.X, e.Y);
                }
                startAngle = GetAngleForPoint(e.X, e.Y) - RawRotationAngle;
                touchStartPoint.X = e.X;
                touchStartPoint.Y = e.Y;
            }
            else
            {
                StopDeceleration();
                SampleVelocity(e.X, e.Y);

                decelerationAngularVelocity = CalculateVelocity();

                if (decelerationAngularVelocity != 0.0f)
                {
                    decelerationLastTime = Environment.TickCount;
                    if (delecelerationTimer == null)
                    {
                        delecelerationTimer = new Animation.Ticker();
                        delecelerationTimer.Update += OnDecelerationLoop;
                    }
                    delecelerationTimer.Start(DecelerationDuration);
                }
            }
        }

        private void OnDecelerationLoop(float _)
        {
            var currentTime = Environment.TickCount;

            decelerationAngularVelocity *= DragDecelerationFrictionCoef;

            var timeInterval = (currentTime - decelerationLastTime) / 1000.0f;

            RotationAngle += decelerationAngularVelocity * timeInterval;

            decelerationLastTime = currentTime;

            if (Math.Abs(decelerationAngularVelocity) < 0.001)
            {
                StopDeceleration();
                return;
            }
            InvalidateSurface();
        }

        float Distance(TouchEvent e, SKPoint start)
        {
            var dx = e.X - start.X;
            var dy = e.Y - start.Y;
            return MathF.Sqrt(dx * dx + dy * dy);
        }

        private void OnTap(TapEvent e)
        {
            if (e.State == TouchState.Ended)
            {
                if (!HighlightPerTapEnabled)
                {
                    return;
                }

                var h = GetHighlightByTouchPoint(e.X, e.Y);
                if (h == null || h.Equals(LastHighlighted))
                {
                    HighlightValue(null, true);
                    LastHighlighted = null;
                }
                else
                {
                    HighlightValue(h, true);
                    LastHighlighted = h;
                }
            }
        }

        private void SampleVelocity(float touchLocationX, float touchLocationY)
        {
            long currentTime = Environment.TickCount;

            _velocitySamples.Add(new AngularVelocitySample(currentTime, GetAngleForPoint(touchLocationX, touchLocationY)));

            // Remove samples older than our sample time - 1 seconds
            for (int i = 0, count = _velocitySamples.Count; i < count - 2; i++)
            {
                if (currentTime - _velocitySamples[i].time > 1000)
                {
                    _velocitySamples.RemoveAt(0);
                    i--;
                    count--;
                }
                else
                {
                    break;
                }
            }
        }

        private float CalculateVelocity()
        {
            if (_velocitySamples.Count == 0)
                return 0.0f;

            AngularVelocitySample firstSample = _velocitySamples[0];
            AngularVelocitySample lastSample = _velocitySamples[_velocitySamples.Count - 1];

            // Look for a sample that's closest to the latest sample, but not the same, so we can deduce the direction
            AngularVelocitySample beforeLastSample = firstSample;
            for (int i = _velocitySamples.Count - 1; i >= 0; i--)
            {
                beforeLastSample = _velocitySamples[i];
                if (beforeLastSample.angle != lastSample.angle)
                {
                    break;
                }
            }

            // Calculate the sampling time
            float timeDelta = (lastSample.time - firstSample.time) / 1000.0f;
            if (timeDelta == 0.0f)
            {
                timeDelta = 0.1f;
            }

            // Calculate clockwise/ccw by choosing two values that should be closest to each other,
            // so if the angles are two far from each other we know they are inverted "for sure"
            var clockwise = lastSample.angle >= beforeLastSample.angle;
            if (Math.Abs(lastSample.angle - beforeLastSample.angle) > 270.0f)
            {
                clockwise = !clockwise;
            }

            // Now if the "gesture" is over a too big of an angle - then we know the angles are inverted, and we need to move them closer to each other from both sides of the 360.0 wrapping point
            if (lastSample.angle - firstSample.angle > 180.0)
            {
                firstSample.angle += 360.0f;
            }
            else if (firstSample.angle - lastSample.angle > 180.0)
            {
                lastSample.angle += 360.0f;
            }

            // The velocity
            float velocity = Math.Abs((lastSample.angle - firstSample.angle) / timeDelta);

            // Direction?
            if (!clockwise)
            {
                velocity = -velocity;
            }

            return velocity;
        }

        private class AngularVelocitySample
        {
            public long time;
            public float angle;

            public AngularVelocitySample(long time, float angle)
            {
                this.time = time;
                this.angle = angle;
            }
        }
    }
}
