using System;
using XF.ChartLibrary.Components;
using XF.ChartLibrary.Interfaces;
using XF.ChartLibrary.Interfaces.DataProvider;
using XF.ChartLibrary.Interfaces.DataSets;
using XF.ChartLibrary.Jobs;
using XF.ChartLibrary.Renderer;
using XF.ChartLibrary.Utils;

#if NETSTANDARD || SKIASHARP
using Rect = SkiaSharp.SKRect;
using Paint = SkiaSharp.SKPaint;
#elif __IOS__ || __TVOS__
using Rect = CoreGraphics.CGRect;
#elif __ANDROID__
using Rect = Android.Graphics.RectF;
using Paint = Android.Graphics.Paint;
#endif


namespace XF.ChartLibrary.Charts
{
    public abstract partial class BarLineChartBase<TData, TDataSet> : ChartBase<TData, TDataSet>, IBarLineScatterCandleBubbleDataProvider where TData : IChartData<TDataSet>, IChartData where TDataSet : IDataSet, IBarLineScatterCandleBubbleDataSet
    {
        /// <summary>
        /// flag that indicates if a custom viewport offset has been set
        /// </summary>
        private bool customViewPortEnabled = false;


        internal YAxisRenderer axisRendererLeft;
        internal YAxisRenderer axisRendererRight;

        protected Transformer LeftAxisTransformer;
        protected Transformer RightAxisTransformer;

        internal XAxisRenderer xAxisRenderer;

        public YAxisRenderer AxisRendererLeft
        {
            get => axisRendererLeft;
            set => axisRendererLeft = value;
        }

        public YAxisRenderer AxisRendererRight
        {
            get => axisRendererRight;
            set => axisRendererRight = value;
        }

        public XAxisRenderer XAxisRenderer
        {
            get => xAxisRenderer;
            set => xAxisRenderer = value;
        }

        /// <summary>
        /// Returns the lowest x-index (value on the x-axis) that is still visible on
        /// the chart.
        /// </summary>
        public float LowestVisibleX
        {
            get
            {
                var point = GetTransformer(YAxisDependency.Left).ValueByTouchPoint(ViewPortHandler.ContentLeft,
                   ViewPortHandler.ContentBottom);
                return (float)Math.Max(XAxis.axisMinimum, point.X);
            }
        }

        /// <summary>
        /// Returns the highest x-index (value on the x-axis) that is still visible
        /// on the chart.
        /// </summary>
        public float HighestVisibleX
        {
            get
            {
                var res = GetTransformer(YAxisDependency.Left).ValueByTouchPoint(ViewPortHandler.ContentRight,
               ViewPortHandler.ContentBottom);
                return (float)Math.Min(XAxis.AxisMaximum, res.X);
            }
        }

        /// <summary>
        /// Returns the range visible on the x-axis.
        /// </summary>
        public float VisibleXRange
        {
            get
            {
                return Math.Abs(HighestVisibleX - LowestVisibleX);
            }
        }


        /// <summary>
        ///  Sets the size of the area(range on the x-axis) that should be minimum
        /// visible at once(no further zooming in allowed). If this is e.g.set to
        /// 10, no less than a range of 10 on the x-axis can be viewed at once without
        /// scrolling.
        /// </summary>
        public float VisibleXRangeMinimum
        {
            set
            {
                ViewPortHandler.SetMaximumScaleX(XAxis.axisRange / value);
            }
        }

        /// <summary>
        /// Sets the size of the area(range on the x-axis) that should be maximum
        /// visible at once(no further zooming out allowed). If this is e.g.set to
        /// 10, no more than a range of 10 on the x-axis can be viewed at once without
        /// scrolling.
        /// </summary>
        public float VisibleXRangeMaximum
        {
            set
            {
                ViewPortHandler.SetMinimumScaleX(XAxis.axisRange / value);
            }
        }

        public override float YChartMax => Math.Max(AxisLeft.axisMaximum, AxisRight.axisMaximum);

        public override float YChartMin => Math.Min(AxisLeft.axisMinimum, AxisRight.axisMinimum);

        IChartData IBarLineScatterCandleBubbleProvider.Data
        {
            get
            {
                return data;
            }
        }

        public override void Initialize()
        {
            base.Initialize();
            AxisLeft = new YAxis(YAxisDependency.Left);
            AxisRight = new YAxis(YAxisDependency.Right);

            LeftAxisTransformer = new Transformer(ViewPortHandler);
            RightAxisTransformer = new Transformer(ViewPortHandler);

            axisRendererLeft = new YAxisRenderer(ViewPortHandler, AxisLeft, LeftAxisTransformer);
            axisRendererRight = new YAxisRenderer(ViewPortHandler, AxisRight, RightAxisTransformer);
            highlighter = new Highlight.ChartHighlighter<BarLineChartBase<TData, TDataSet>>(this);

            xAxisRenderer = new XAxisRenderer(ViewPortHandler, XAxis, LeftAxisTransformer);
        }

#if SKIASHARP
        public override void SetPaint(Paint p, PaintKind which)
        {
            base.SetPaint(p, which);

            switch (which)
            {
                case PaintKind.GridBackground:
                    GridBackgroundPaint = p;
                    break;
            }
        }

        public override Paint GetPaint(PaintKind which)
        {
            Paint p = base.GetPaint(which);
            if (p != null)
                return p;

            switch (which)
            {
                case PaintKind.GridBackground:
                    return GridBackgroundPaint;
            }

            return null;
        }
#endif

        protected void PrepareOffsetMatrix()
        {
            RightAxisTransformer.PrepareMatrixOffset(AxisRight.Inverted);
            LeftAxisTransformer.PrepareMatrixOffset(AxisLeft.Inverted);
        }

        protected void PrepareValuePxMatrix()
        {
            RightAxisTransformer.PrepareMatrixValuePx(XAxis.axisMinimum,
                    XAxis.axisRange,
                    AxisRight.axisRange,
                    AxisRight.axisMinimum);
            LeftAxisTransformer.PrepareMatrixValuePx(XAxis.axisMinimum,
                    XAxis.axisRange,
                    AxisLeft.axisRange,
                    AxisLeft.axisMinimum);
        }

        /// <summary>
        ///  Limits the maximum and minimum x range that can be visible by pinching and zooming.e.g.minRange=10, maxRange=100 the
        /// smallest range to be displayed at once is 10, and no more than a range of 100 values can be viewed at once without
        /// scrolling
        /// </summary>
        public void SetVisibleXRange(float minXRange, float maxXRange)
        {
            float minScale = XAxis.AxisRange / minXRange;
            float maxScale = XAxis.AxisRange / maxXRange;
            ViewPortHandler.SetMinMaxScaleX(minScale, maxScale);
        }

        /// <summary>
        ///  Sets the size of the area(range on the y-axis) that should be maximum
        /// visible at once.
        /// </summary>
        /// <param name="maxYRange">the maximum visible range on the y-axis</param>
        /// <param name="axis">the axis for which this limit should apply</param>
        public void SetVisibleYRangeMaximum(float maxYRange, YAxisDependency axis)
        {
            ViewPortHandler.SetMinimumScaleY(GetAxisRange(axis) / maxYRange);
        }

        /// <summary>
        /// Sets the size of the area(range on the y-axis) that should be minimum visible at once, no further zooming in possible.
        /// </summary>
        /// <param name="minYRange">Min Y Range</param>
        /// <param name="axis">the axis for which this limit should apply</param>
        public void SetVisibleYRangeMinimum(float minYRange, YAxisDependency axis)
        {
            ViewPortHandler.SetMaximumScaleY(GetAxisRange(axis) / minYRange);
        }

        /// <summary>
        /// Limits the maximum and minimum y range that can be visible by pinching and zooming.
        /// </summary>
        public void SetVisibleYRange(float minYRange, float maxYRange, YAxisDependency axis)
        {
            float range = GetAxisRange(axis);
            ViewPortHandler.SetMinMaxScaleY(range / minYRange, range / maxYRange);
        }

#if __ANDROID__ && !SKIASHARP
        protected Rect CalculateLegendOffsets(Rect offsets)
        {

#else

        protected Rect CalculateLegendOffsets()
        {
#endif
            var left = 0.0f;
            var right = 0.0f;
            var top = 0.0f;
            var bottom = 0.0f;
            Legend legend = this.legend;
            if (legend != null && legend.IsEnabled && legend.IsDrawInsideEnabled == false)
            {
                switch (legend.Orientation)
                {
                    case Orientation.Vertical:

                        switch (legend.HorizontalAlignment)
                        {
                            case Components.HorizontalAlignment.Left:
                                left = Math.Min(legend.NeededWidth,
                                        ViewPortHandler.ChartWidth * legend.MaxSizePercent)
                                        + legend.XOffset;
                                break;

                            case Components.HorizontalAlignment.Right:
                                right = Math.Min(legend.NeededWidth,
                                        ViewPortHandler.ChartWidth * legend.MaxSizePercent)
                                        + legend.XOffset;
                                break;

                            case Components.HorizontalAlignment.Center:
                                switch (legend.VerticalAlignment)
                                {
                                    case Components.VerticalAlignment.Top:
                                        top = Math.Min(legend.NeededHeight,
                                                ViewPortHandler.ChartHeight * legend.MaxSizePercent)
                                                + legend.YOffset;
                                        break;

                                    case Components.VerticalAlignment.Bottom:
                                        bottom = Math.Min(legend.NeededHeight,
                                                ViewPortHandler.ChartHeight * legend.MaxSizePercent)
                                                + legend.YOffset;
                                        break;

                                    default:
                                        break;
                                }
                                break;
                        }

                        break;

                    case Orientation.Horizontal:

                        switch (legend.VerticalAlignment)
                        {
                            case Components.VerticalAlignment.Top:
                                top = Math.Min(legend.NeededHeight,
                                        ViewPortHandler.ChartHeight * legend.MaxSizePercent)
                                        + legend.YOffset;
                                break;

                            case Components.VerticalAlignment.Bottom:
                                bottom = Math.Min(legend.NeededHeight,
                                        ViewPortHandler.ChartHeight * legend.MaxSizePercent)
                                        + legend.YOffset;


                                break;

                            default:
                                break;
                        }
                        break;
                }
            }
#if __ANDROID__ && !SKIASHARP
            offsets.Left = left;
            offsets.Right = right;
            offsets.Top = top;
            offsets.Bottom = bottom;
            return offsets;
#else
            return new Rect(top, left, right, bottom);
#endif
        }

        public override void NotifyDataSetChanged()
        {
            TData data = Data;
            if (data == null)
                return;

            if (renderer != null)
                renderer.InitBuffers();

            CalcMinMax();

            axisRendererLeft.ComputeAxis(AxisLeft.axisMinimum, AxisLeft.axisMaximum, AxisLeft.Inverted);
            axisRendererRight.ComputeAxis(AxisRight.axisMinimum, AxisRight.axisMaximum, AxisRight.Inverted);
            xAxisRenderer.ComputeAxis(XAxis.axisMinimum, XAxis.axisMaximum, false);

            if (legend != null)
                legendRenderer.ComputeLegend(data);

            CalculateOffsets();
        }

        /// <summary>
        /// Performs auto scaling of the axis by recalculating the minimum and maximum y-values based on the entries currently in view.
        /// </summary>
        protected void AutoScale()
        {
            float fromX = LowestVisibleX;
            float toX = HighestVisibleX;
            var data = Data;
            data.CalcMinMaxY(fromX, toX);

            XAxis.Calculate(data.XMin, data.XMax);

            // calculate axis range (min / max) according to provided data

            if (AxisLeft.IsEnabled)
                AxisLeft.Calculate(data.GetYMin(YAxisDependency.Left),
                        data.GetYMax(YAxisDependency.Left));

            if (AxisRight.IsEnabled)
                AxisRight.Calculate(data.GetYMin(YAxisDependency.Right),
                        data.GetYMax(YAxisDependency.Right));

            CalculateOffsets();
        }

        protected override void CalcMinMax()
        {
            CalcMinMax(data);
        }

        protected void CalcMinMax(TData data)
        {
            XAxis.Calculate(data.XMin, data.XMax);

            // calculate axis range (min / max) according to provided data
            AxisLeft.Calculate(data.GetYMin(YAxisDependency.Left), data.GetYMax(YAxisDependency.Left));
            AxisRight.Calculate(data.GetYMin(YAxisDependency.Right), data.GetYMax(YAxisDependency.Right));
        }

        /// <summary>
        /// Returns the Transformer class that contains all matrices and is
        /// responsible for transforming values into pixels on the screen and
        /// backwards.
        /// </summary>
        /// <returns></returns>
        public Transformer GetTransformer(YAxisDependency which)
        {
            if (which == YAxisDependency.Left)
                return LeftAxisTransformer;
            else
                return RightAxisTransformer;
        }


        public YAxis GetAxis(YAxisDependency axis)
        {
            if (axis == YAxisDependency.Left)
                return AxisLeft;
            else
                return AxisRight;
        }

        /// <summary>
        /// Returns the range of the specified axis.
        /// </summary>
        /// <param name="axis"></param>
        protected float GetAxisRange(YAxisDependency axis)
        {
            return axis == YAxisDependency.Left ? AxisLeft.axisRange : AxisRight.axisRange;
        }

        public bool IsInverted(YAxisDependency axis)
        {
            return axis == YAxisDependency.Left ? AxisLeft.Inverted : AxisRight.Inverted;
        }

        /// <summary>
        ///  Moves the left side of the current viewport to the specified x-position.
        /// This also refreshes the chart by calling invalidate().
        /// </summary>
        public void MoveViewToX(float xValue)
        {
            AddViewportJob(MoveViewJob.GetInstance(ViewPortHandler, xValue, 0f,
                    GetTransformer(YAxisDependency.Left), this));
        }

        /// <summary>
        /// This will move the left side of the current viewport to the specified
        /// x-value on the x-axis, and center the viewport to the specified y value on the y-axis.
        /// This also refreshes the chart by calling invalidate().
        /// </summary>
        /// <param name="xValue"></param>
        /// <param name="yValue"></param>
        /// <param name="axis">which axis should be used as a reference for the y-axis</param>
        public void MoveViewTo(float xValue, float yValue, YAxisDependency axis)
        {
            float yInView = GetAxisRange(axis) / ViewPortHandler.ScaleY;

            AddViewportJob(MoveViewJob.GetInstance(ViewPortHandler, xValue, yValue + yInView / 2f,
                    GetTransformer(axis), this));
        }

        public void Zoom(float scaleX, float scaleY, float x, float y)
        {
            ViewPortHandler.Refresh(ViewPortHandler.Zoom(scaleX: scaleX, scaleY: scaleY, x: x, y: -y), this, invalidate: false);

            // Range might have changed, which means that Y-axis labels could have changed in size, affecting Y-axis size. So we need to recalculate offsets.
            CalculateOffsets();
            this.InvalidateView();
        }

        /// <summary>
        /// Resets all custom offsets set via setViewPortOffsets(...) method. Allows
        /// the chart to again calculate all offsets automatically.
        /// </summary>
        public void ResetViewPortOffsets()
        {
            customViewPortEnabled = false;
            CalculateOffsets();
        }
    }
}
