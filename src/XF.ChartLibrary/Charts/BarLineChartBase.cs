using SkiaSharp;
using System;
using Xamarin.Forms;
using XF.ChartLibrary.Components;
using XF.ChartLibrary.Gestures;
using XF.ChartLibrary.Interfaces.DataSets;
using XF.ChartLibrary.Utils;

namespace XF.ChartLibrary.Charts
{
    partial class BarLineChartBase<TData, TDataSet> : IGestureController, IBarLineChartBase
    {
        public static readonly BindableProperty DragXEnabledProperty = BindableProperty.Create(nameof(DragXEnabled), typeof(bool), typeof(BarLineChartBase<TData, TDataSet>), true);

        public static readonly BindableProperty DragYEnabledProperty = BindableProperty.Create(nameof(DragYEnabled), typeof(bool), typeof(BarLineChartBase<TData, TDataSet>), true);

        public static readonly BindableProperty ScaleXEnabledProperty = BindableProperty.Create(nameof(ScaleXEnabled), typeof(bool), typeof(BarLineChartBase<TData, TDataSet>), true);

        public static readonly BindableProperty ScaleYEnabledProperty = BindableProperty.Create(nameof(ScaleYEnabled), typeof(bool), typeof(BarLineChartBase<TData, TDataSet>), true);

        public static readonly BindableProperty HighlightPerDragEnabledProperty = BindableProperty.Create(nameof(HighlightPerDragEnabled), typeof(bool), typeof(BarLineChartBase<TData, TDataSet>), true);

        public static readonly BindableProperty AxisLeftProperty = BindableProperty.Create(nameof(AxisLeft), typeof(YAxis), typeof(BarLineChartBase<TData, TDataSet>), defaultBindingMode: BindingMode.OneWayToSource);

        public static readonly BindableProperty AxisRightProperty = BindableProperty.Create(nameof(AxisRight), typeof(YAxis), typeof(BarLineChartBase<TData, TDataSet>), defaultBindingMode: BindingMode.OneWayToSource);

        public static readonly BindableProperty MaxVisibleCountProperty = BindableProperty.Create(nameof(MaxVisibleCount), typeof(int), typeof(BarLineChartBase<TData, TDataSet>), defaultValue: 100);

        public static readonly BindableProperty PinchZoomEnabledProperty = BindableProperty.Create(nameof(PinchZoomEnabled), typeof(bool), typeof(BarLineChartBase<TData, TDataSet>), true);

        public static readonly BindableProperty DoubleTapToZoomEnabledProperty = BindableProperty.Create(nameof(DoubleTapToZoomEnabled), typeof(bool), typeof(BarLineChartBase<TData, TDataSet>), true);

        public static readonly BindableProperty GridBackgroundColorProperty = BindableProperty.Create(nameof(GridBackgroundColor), typeof(Color), typeof(BarLineChartBase<TData, TDataSet>), propertyChanged: OnGridBackgroundColorChanged);

        public static readonly BindableProperty IsDrawGridBackgroundProperty = BindableProperty.Create(nameof(IsDrawGridBackground), typeof(bool), typeof(BarLineChartBase<TData, TDataSet>));

        public static readonly BindableProperty DrawBordersProperty = BindableProperty.Create(nameof(DrawBorders), typeof(bool), typeof(BarLineChartBase<TData, TDataSet>));

        public static readonly BindableProperty AutoScaleMinMaxEnabledProperty = BindableProperty.Create(nameof(AutoScaleMinMaxEnabled), typeof(bool), typeof(BarLineChartBase<TData, TDataSet>));

        public static readonly BindableProperty ClipDataToContentProperty = BindableProperty.Create(nameof(ClipDataToContent), typeof(bool), typeof(BarLineChartBase<TData, TDataSet>), true);

        public static readonly BindableProperty ClipValuesToContentProperty = BindableProperty.Create(nameof(ClipValuesToContent), typeof(bool), typeof(BarLineChartBase<TData, TDataSet>));

        public static readonly BindableProperty MinOffsetProperty = BindableProperty.Create(nameof(MinOffset), typeof(float), typeof(BarLineChartBase<TData, TDataSet>), defaultValue: 15.0f);

        public static readonly BindableProperty KeepPositionOnRotationProperty = BindableProperty.Create(nameof(KeepPositionOnRotation), typeof(bool), typeof(BarLineChartBase<TData, TDataSet>));

        static void OnGridBackgroundColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((BarLineChartBase<TData, TDataSet>)bindable).GridBackgroundPaint.Color = ((Color)newValue).ToSKColor();
        }

        public long DecelerationDuration { get; set; } = 2500;

        private SKPoint touchStartPoint;
        private Animation.Ticker ticker;

        #region Scale & Pan
        private SKMatrix savedMatrix;
        private IBarLineScatterCandleBubbleDataSet closestDataSetToTouch;
        private SKPoint lastPanPoint;
        private SKPoint decelerationVelocity = SKPoint.Empty;
        private long decelerationLastTime;
        #endregion

        public override IChartGesture Gesture { get; }

        public BarLineChartBase()
        {
            GridBackgroundPaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                // Color = SKColors.White;
                Color = new SKColor(240, 240, 240) // light
                                                   // grey
            };

            BorderPaint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = SKColors.Black,
                StrokeWidth = 1f
            };

            BarLineChartGesture gesture = new BarLineChartGesture();
            Gesture = gesture;
            gesture.Tap += OnTap;
            gesture.Pan += OnPan;
            gesture.DoubleTap += OnDoubleTap;
            gesture.Pinch += OnPinch;
            savedMatrix = SKMatrix.Identity;
        }

        public Color GridBackgroundColor
        {
            get => GridBackgroundPaint.Color.ToFormsColor();
            set => SetValue(GridBackgroundColorProperty, value);
        }

        /// <summary>
        /// true if keeping the position on rotation is enabled and false if not.
        /// </summary>
        public bool KeepPositionOnRotation
        {
            get => (bool)GetValue(KeepPositionOnRotationProperty);
            set => SetValue(KeepPositionOnRotationProperty, value);
        }

        /// <summary>
        /// flag indicating if the grid background should be drawn or not
        /// </summary>
        public bool IsDrawGridBackground
        {
            get => (bool)GetValue(IsDrawGridBackgroundProperty);
            set => SetValue(IsDrawGridBackgroundProperty, value);
        }

        public bool DrawBorders
        {
            get => (bool)GetValue(DrawBordersProperty);
            set => SetValue(DrawBordersProperty, value);
        }

        /// <summary>
        ///  flag that indicates if auto scaling on the y axis is enabled
        /// </summary>
        public bool AutoScaleMinMaxEnabled
        {
            get => (bool)GetValue(AutoScaleMinMaxEnabledProperty);
            set => SetValue(AutoScaleMinMaxEnabledProperty, value);
        }

        public bool ClipDataToContent
        {
            get => (bool)GetValue(ClipDataToContentProperty);
            set => SetValue(ClipDataToContentProperty, value);
        }

        public bool ClipValuesToContent
        {
            get => (bool)GetValue(ClipValuesToContentProperty);
            set => SetValue(ClipValuesToContentProperty, value);
        }
        /// <summary>
        /// Sets the minimum offset (padding) around the chart, defaults to 15
        /// </summary>
        public float MinOffset
        {
            get => (float)GetValue(MinOffsetProperty);
            set => SetValue(MinOffsetProperty, value);
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
            if (e.State == TouchState.Begin)
            {
                StopDeceleration();
                SaveTouchStart(x, y);
                if (PinchZoomEnabled)
                {
                    e.Mode = PinchMode.PinchZoom;
                }
                else
                {
                    var scaleXEnabled = ScaleXEnabled;
                    if (scaleXEnabled != ScaleYEnabled)
                    {
                        e.Mode = scaleXEnabled ? PinchMode.XZoom : PinchMode.YZoom;
                    }
                    else
                    {
                        e.Mode = e.XDist > e.YDist ? PinchMode.XZoom : PinchMode.YZoom;
                    }
                }
            }
            else if (e.State == TouchState.Ended)
            {
                CalculateOffsets();
                InvalidateSurface();
            }
            else if (e.State == TouchState.Changed)
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
                var isZoomingOut = e.Scale < 1;
                if (e.Mode == PinchMode.PinchZoom)
                {
                    var canZoomMoreX = isZoomingOut ? ViewPortHandler.CanZoomOutMoreX : ViewPortHandler.CanZoomInMoreX;
                    var canZoomMoreY = isZoomingOut ? ViewPortHandler.CanZoomOutMoreY : ViewPortHandler.CanZoomInMoreY;
                    if (canZoomMoreX || canZoomMoreY)
                    {
                        ViewPortHandler.Refresh(savedMatrix.PostConcat(SKMatrix.CreateScale(ScaleXEnabled ? e.Scale : 1f, ScaleYEnabled ? e.Scale : 1f, x, y)), chart: this, invalidate: true);
                    }
                }
                else if (e.Mode == PinchMode.XZoom && ScaleXEnabled && (isZoomingOut ? ViewPortHandler.CanZoomOutMoreX : ViewPortHandler.CanZoomInMoreX))
                {
                    ViewPortHandler.Refresh(savedMatrix.PostConcat(SKMatrix.CreateScale(e.Scale, 1f, x, y)), chart: this, invalidate: true);
                }
                else if (e.Mode == PinchMode.YZoom && ScaleYEnabled && (isZoomingOut ? ViewPortHandler.CanZoomOutMoreY : ViewPortHandler.CanZoomInMoreY))
                {
                    ViewPortHandler.Refresh(savedMatrix.PostConcat(SKMatrix.CreateScale(1f, e.Scale, x, y)), chart: this, invalidate: true);
                }

                // Deletegate after change
            }
        }

        private void OnPan(PanEvent e, float distanceX, float distanceY)
        {
            if (e.State == TouchState.Begin)
            {
                if (data is null)
                    return;
                if (!ViewPortHandler.HasNoDragOffset || !ViewPortHandler.IsFullyZoomedOut)
                {
                    e.Mode = PanMode.Drag;
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
                    if ((distanceX != 0.0f || distanceY != 0.0f) && !PerformPanChange(lastPanPoint.X, lastPanPoint.Y))
                    {
                        // We can stop dragging right now, and let the scroll view take control
                        e.Mode = PanMode.None;
                    }

                }
                else if (HighlightPerDragEnabled)
                {
                    // We will only handle highlights on Changed
                    e.Mode = PanMode.None;
                }
            }
            else if (e.State == TouchState.Changed)
            {
                if (e.Mode == PanMode.Drag)
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

                    _ = PerformPanChange(translation.X, translation.Y);

                }
                else if (HighlightPerDragEnabled)
                {
                    var h = GetHighlightByTouchPoint(distanceX, distanceY);

                    var lastHighlighted = this.LastHighlighted;

                    if (h != lastHighlighted)
                    {
                        LastHighlighted = h;
                        HighlightValue(h, true);
                    }
                }
            }
            else if (e.State == TouchState.Ended)
            {
                if (e.Mode == PanMode.Drag)
                {
                    StopDeceleration();
                    decelerationLastTime = Environment.TickCount;
                    decelerationVelocity.X = e.VelocityX;
                    decelerationVelocity.Y = e.VelocityY;
                    if (ticker == null)
                    {
                        ticker = new Animation.Ticker();
                        ticker.Update += OnDecelerationLoop;
                    }
                    ticker.Start(DecelerationDuration);
                }


                // Chart did pan ended
            }
        }

        void OnDecelerationLoop(float _)
        {
            decelerationVelocity.X *= _dragDecelerationFrictionCoef;
            decelerationVelocity.Y *= _dragDecelerationFrictionCoef;

            var current = Environment.TickCount;

            var timeInterval = (current - decelerationLastTime) / 1000f;
            PerformPanChange(decelerationVelocity.X * timeInterval, decelerationVelocity.Y * timeInterval);

            decelerationLastTime = current;

            if (Math.Abs(decelerationVelocity.X) < 0.001f && Math.Abs(decelerationVelocity.Y) < 0.001f)
            {
                StopDeceleration();

                // Range might have changed, which means that Y-axis labels could have changed in size, affecting Y-axis size. So we need to recalculate offsets.
                CalculateOffsets();
                InvalidateSurface();
            }
        }

        bool PerformPanChange(float tranX, float tranY)
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
                    tranY = -tranY;
                }
            }

            var originalMatrix = ViewPortHandler.touchMatrix;
            var matrix = ViewPortHandler.Refresh(originalMatrix.PostConcat(SKMatrix.CreateTranslation(tranX, tranY)), chart: this, invalidate: true);

            // Did we managed to actually drag or did we reach the edge?
            return matrix.TransX != originalMatrix.TransX || matrix.TransY != originalMatrix.TransY;
        }

        private void OnTap(TapEvent e)
        {
            if(e.State == TouchState.Begin)
            {
                SaveTouchStart(e.X, e.Y);
                return;
            }
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


        public void StopDeceleration()
        {
            if (ticker != null)
            {
                ticker.Cancel();
            }
            decelerationVelocity = SKPoint.Empty;
        }

        private void SaveTouchStart(float x, float y)
        {
            savedMatrix = ViewPortHandler.touchMatrix;
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

        public override void OnSizeChanged(float w, float h)
        {
            SKPoint pt = SKPoint.Empty;
            if (KeepPositionOnRotation)
            {
                pt = GetTransformer(YAxisDependency.Left).PixelsToValue(ViewPortHandler.ContentLeft, ViewPortHandler.ContentTop);
            }

            //Superclass transforms chart.
            base.OnSizeChanged(w, h);

            if (KeepPositionOnRotation)
            {
                //Restoring old position of chart.
                pt = GetTransformer(YAxisDependency.Left).PointValueToPixel(pt.X, pt.Y);
                ViewPortHandler.CenterViewPort(pt, this);
            }
            else
            {
                ViewPortHandler.Refresh(ViewPortHandler.touchMatrix, this, true);
            }
        }
    }
}
