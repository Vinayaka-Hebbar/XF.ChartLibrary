using SkiaSharp;
using System;
using Xamarin.Forms;
using XF.ChartLibrary.Components;
using XF.ChartLibrary.Gestures;
using XF.ChartLibrary.Interfaces.DataSets;
using XF.ChartLibrary.Utils;

namespace XF.ChartLibrary.Charts
{
    partial class BarLineChartBase<TData, TDataSet> : IGestureController
    {
        public static readonly BindableProperty DragXEnabledProperty = BindableProperty.Create(nameof(DragXEnabled), typeof(bool), typeof(BarLineChartBase<TData, TDataSet>), true);

        public static readonly BindableProperty DragYEnabledProperty = BindableProperty.Create(nameof(DragYEnabled), typeof(bool), typeof(BarLineChartBase<TData, TDataSet>), true);

        public static readonly BindableProperty ScaleXEnabledProperty = BindableProperty.Create(nameof(ScaleXEnabled), typeof(bool), typeof(BarLineChartBase<TData, TDataSet>), true);

        public static readonly BindableProperty ScaleYEnabledProperty = BindableProperty.Create(nameof(ScaleYEnabled), typeof(bool), typeof(BarLineChartBase<TData, TDataSet>), true);

        public static readonly BindableProperty HighlightPerDragEnabledProperty = BindableProperty.Create(nameof(HighlightPerDragEnabled), typeof(bool), typeof(BarLineChartBase<TData, TDataSet>), true);

        public static readonly BindableProperty AxisLeftProperty = BindableProperty.Create(nameof(AxisLeft), typeof(YAxis), typeof(BarLineChartBase<TData, TDataSet>), defaultValue: new YAxis(YAxisDependency.Left));

        public static readonly BindableProperty AxisRightProperty = BindableProperty.Create(nameof(AxisRight), typeof(YAxis), typeof(BarLineChartBase<TData, TDataSet>), defaultValue: new YAxis(YAxisDependency.Right));

        public static readonly BindableProperty MaxVisibleCountProperty = BindableProperty.Create(nameof(MaxVisibleCount), typeof(int), typeof(BarLineChartBase<TData, TDataSet>), defaultValue: 100);

        public static readonly BindableProperty PinchZoomEnabledProperty = BindableProperty.Create(nameof(PinchZoomEnabled), typeof(bool), typeof(BarLineChartBase<TData, TDataSet>), true);

        public static readonly BindableProperty DoubleTapToZoomEnabledProperty = BindableProperty.Create(nameof(DoubleTapToZoomEnabled), typeof(bool), typeof(BarLineChartBase<TData, TDataSet>), true);


        private SKPoint touchStartPoint;

        #region Scale & Pan
        private IBarLineScatterCandleBubbleDataSet closestDataSetToTouch;
        private SKPoint lastPanPoint;
        private SKPoint decelerationVelocity = SKPoint.Empty;
        private long decelerationLastTime;
        #endregion

        public override IChartGesture Gesture { get; }

        public BarLineChartBase()
        {
            gridBackgroundPaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                // Color = SKColors.White;
                Color = new SKColor(240, 240, 240) // light
                                                   // grey
            };

            borderPaint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = SKColors.Black,
                StrokeWidth = 1f
            };

            ChartGestureRecognizer gesture = new ChartGestureRecognizer();
            Gesture = gesture;
            gesture.Tap += OnTap;
            gesture.Pan += OnPan;
            gesture.DoubleTap += OnDoubleTap;
            gesture.Pinch += OnPinch;
            GestureRecognizers.Add(Gesture);
        }

        public bool DoubleTapToZoomEnabled
        {
            get => (bool)GetValue(DoubleTapToZoomEnabledProperty);
            set => SetValue(DoubleTapToZoomEnabledProperty, value);
        }

        public bool PinchZoomEnabled
        {
            get => (bool)GetValue(PinchZoomEnabledProperty);
            set => SetValue(PinchZoomEnabledProperty, value);
        }

        /// <summary>
        /// `true` if either the left or the right or both axes are inverted.
        /// </summary>
        public bool IsAnyAxisInverted
        {
            get
            {
                return AxisLeft.Inverted || AxisRight.Inverted;
            }
        }

        private bool IsTouchInverted()
        {
            return IsAnyAxisInverted &&
                closestDataSetToTouch != null &&
                GetAxis(closestDataSetToTouch.AxisDependency).Inverted;
        }

        private void OnDoubleTap(float x, float y)
        {
            if (data is null)
                return;
            if (DoubleTapToZoomEnabled && (data.EntryCount > 0))
            {
                x -= ViewPortHandler.OffsetLeft;


                if (IsTouchInverted())
                {
                    y = -y + ViewPortHandler.OffsetTop;
                }
                else
                {
                    y += ViewPortHandler.OffsetBottom - ViewPortHandler.ChartHeight;
                }
                Zoom(scaleX: ScaleXEnabled ? 1.4f : 1.0f, scaleY: ScaleYEnabled ? 1.4f : 1.0f, x: x, y: y);
            }
        }

        private void OnPinch(PinchEvent e, float x, float y)
        {
            bool scaleXEnabled;
            bool scaleYEnabled;
            if (e.state == TouchState.Started)
            {
                StopDeceleration();
                SaveTouchStart(x, y);
                if (PinchZoomEnabled)
                {
                    e.Mode = PinchState.PinchZoom;
                }
                else
                {
                    if (ScaleXEnabled != ScaleYEnabled)
                    {
                        e.Mode = ScaleXEnabled ? PinchState.XZoom : PinchState.YZoom;
                    }
                    else
                    {
                        e.Mode = e.xDist > e.yDist ? PinchState.XZoom : PinchState.YZoom;
                    }
                }
            }
            else if (e.state == TouchState.Completed)
            {
                CalculateOffsets();
                InvalidateSurface();
            }
            else if (e.state == TouchState.Running)
            {
                scaleXEnabled = ScaleXEnabled;
                scaleYEnabled = ScaleYEnabled;
                var isZoomingOut = e.Scale < 1;
                var canZoomMoreX = isZoomingOut ? ViewPortHandler.CanZoomOutMoreX : ViewPortHandler.CanZoomInMoreX;
                var canZoomMoreY = isZoomingOut ? ViewPortHandler.CanZoomOutMoreY : ViewPortHandler.CanZoomInMoreY;
                canZoomMoreX = canZoomMoreX && scaleXEnabled && (e.Mode == PinchState.PinchZoom || e.Mode == PinchState.XZoom);
                canZoomMoreY = canZoomMoreY && scaleYEnabled && (e.Mode == PinchState.PinchZoom || e.Mode == PinchState.YZoom);
                if (canZoomMoreX || canZoomMoreY)
                {
                    x -= ViewPortHandler.OffsetLeft;


                    if (IsTouchInverted())
                    {
                        y = -(y - ViewPortHandler.OffsetTop);
                    }
                    else
                    {
                        y = -(ViewPortHandler.ChartHeight - y - ViewPortHandler.OffsetBottom);
                    }

                    var scaleX = canZoomMoreX ? e.Scale : 1.0f;
                    var scaleY = canZoomMoreY ? e.Scale : 1.0f;

                    ViewPortHandler.Refresh(ViewPortHandler.touchMatrix.PostConcat(SKMatrix.CreateScale(scaleX, scaleY, x, y)), chart: this, invalidate: true);
                    // Deletegate
                }
            }
        }

        private void OnPan(PanEvent e, float distanceX, float distanceY)
        {
            if (e.state == TouchState.Started)
            {
                if (data is null)
                    return;
                if (!ViewPortHandler.HasNoDragOffset || !ViewPortHandler.IsFullyZoomedOut)
                {
                    e.Mode = PanState.Drag;
                    lastPanPoint.X = distanceX;
                    lastPanPoint.Y = distanceY;
                    closestDataSetToTouch = GetDataSetByTouchPoint(e.X, e.Y);
                    if (!DragXEnabled)
                    {
                        distanceX = 0.0f;
                    }
                    else if (!DragYEnabled)
                    {
                        distanceY = 0.0f;
                    }


                    // Check to see if user dragged at all and if so, can the chart be dragged by the given amount
                    if ((distanceX != 0.0f || distanceY != 0.0f) && !PerformPanChange(translation: lastPanPoint))
                    {
                        // We can stop dragging right now, and let the scroll view take control
                        e.Mode = PanState.None;
                    }

                }
                else if (HighlightPerDragEnabled)
                {
                    // We will only handle highlights on Changed
                    e.Mode = PanState.None;
                }
            }
            else if (e.state == TouchState.Running)
            {
                if (e.Mode == PanState.Drag)
                {
                    var translation = new SKPoint(x: distanceX - lastPanPoint.X, y: distanceY - lastPanPoint.Y);
                    lastPanPoint.X = distanceX;
                    lastPanPoint.Y = distanceY;

                    if (!DragXEnabled)
                    {
                        translation.X = 0.0f;
                    }
                    else if (!DragYEnabled)
                    {
                        translation.Y = 0.0f;
                    }

                    _ = PerformPanChange(translation: translation);

                }
                else if (HighlightPerDragEnabled)
                {
                    var h = GetHighlightByTouchPoint(distanceX, distanceY);

                    var lastHighlighted = this.lastHighlighted;

                    if (h != lastHighlighted)
                    {
                        this.lastHighlighted = h;
                        HighlightValue(h, true);
                    }
                }
            }
            else if (e.state == TouchState.Completed)
            {
                if (e.Mode == PanState.Drag)
                {
                    StopDeceleration();

                    decelerationLastTime = Environment.TickCount;
                    decelerationVelocity.X = e.VelocityX;
                    decelerationVelocity.Y = e.VelocityY;
                    Dispatcher.BeginInvokeOnMainThread(DecelerationLoop);
                }


                // Chart did pan ended
            }
        }

        void DecelerationLoop()
        {
            var currentTime = Environment.TickCount;

            decelerationVelocity.X *= DragDecelerationFrictionCoef;
            decelerationVelocity.Y *= DragDecelerationFrictionCoef;


            var timeInterval = (currentTime - decelerationLastTime) / 1000;


            var distance = new SKPoint(
                x: decelerationVelocity.X * timeInterval,
                y: decelerationVelocity.Y * timeInterval
            );


            if (!PerformPanChange(translation: distance))
            {
                // We reached the edge, stop
                decelerationVelocity.X = 0.0f;
                decelerationVelocity.Y = 0.0f;
            }

            decelerationLastTime = currentTime;


            if (Math.Abs(decelerationVelocity.X) < 0.001f && Math.Abs(decelerationVelocity.Y) < 0.001f)
            {
                StopDeceleration();

                // Range might have changed, which means that Y-axis labels could have changed in size, affecting Y-axis size. So we need to recalculate offsets.
                CalculateOffsets();
                InvalidateSurface();
            }
        }

        private bool PerformPanChange(SKPoint translation)
        {
            if (IsTouchInverted())
            {
                if (false)
                {
                    // TODO for horizontal bar
                    // translation.X = -translation.X;
                }
                else
                {
                    translation.Y = -translation.Y;
                }
            }

            var originalMatrix = ViewPortHandler.touchMatrix;
            var matrix = ViewPortHandler.Refresh(originalMatrix.PostConcat(SKMatrix.CreateTranslation(translation.X, y: translation.Y)), chart: this, invalidate: false);


            if (matrix != originalMatrix)
            {
                InvalidateSurface();
                // Chart translated
            }

            // Did we managed to actually drag or did we reach the edge?
            return matrix.TransX != originalMatrix.TransX || matrix.TransY != originalMatrix.TransY;
        }

        private void OnTap(float x, float y)
        {
            if (!HighlightPerDragEnabled)
            {
                return;
            }

            var h = GetHighlightByTouchPoint(x, y);
            if (h == null || h.Equals(lastHighlighted))
            {
                HighlightValue(null, true);
                lastHighlighted = null;
            }
            else
            {
                HighlightValue(h, true);
                lastHighlighted = h;
            }
        }


        public void StopDeceleration()
        {
            decelerationVelocity = SKPoint.Empty;
        }

        private void SaveTouchStart(float x, float y)
        {
            touchStartPoint.X = x;
            touchStartPoint.Y = y;
            closestDataSetToTouch = GetDataSetByTouchPoint(x, y);
        }

        public bool IsDragEnabled
        {
            get => DragXEnabled || DragYEnabled;
        }

        public bool ScaleXEnabled
        {
            get => (bool)GetValue(ScaleXEnabledProperty);
            set => SetValue(ScaleXEnabledProperty, value);
        }

        public bool ScaleYEnabled
        {
            get => (bool)GetValue(ScaleYEnabledProperty);
            set => SetValue(ScaleYEnabledProperty, value);
        }
    }
}
