using System;
using XF.ChartLibrary.Components;
using XF.ChartLibrary.Interfaces;
using XF.ChartLibrary.Interfaces.DataSets;
using XF.ChartLibrary.Renderer;
using XF.ChartLibrary.Utils;

#if NETSTANDARD || SKIASHARP
using Rect = SkiaSharp.SKRect;
#elif __IOS__ || __TVOS__
using Rect = CoreGraphics.CGRect;
#elif __ANDROID__
using Rect = Android.Graphics.RectF;
#endif


namespace XF.ChartLibrary.Charts
{
    public abstract partial class BarLineChartBase<TData, TDataSet> : ChartBase<TData, TDataSet> where TData : IChartData<TDataSet>, IChartData where TDataSet : IDataSet, IBarLineScatterCandleBubbleDataSet
    {
        /// <summary>
        /// flag that indicates if a custom viewport offset has been set
        /// </summary>
        private bool mCustomViewPortEnabled = false;

        /// <summary>
        /// flag indicating if the grid background should be drawn or not
        /// </summary>
        protected bool IsDrawGridBackground = false;

        protected bool mDrawBorders = false;

        /// <summary>
        ///  flag that indicates if auto scaling on the y axis is enabled
        /// </summary>
        protected bool mAutoScaleMinMaxEnabled = false;

        protected bool mClipDataToContent = true;

        protected bool mClipValuesToContent = false;
        /// <summary>
        /// Sets the minimum offset (padding) around the chart, defaults to 15
        /// </summary>
        protected float MinOffset = 15.0f;

        protected YAxisRenderer mAxisRendererLeft;
        protected YAxisRenderer mAxisRendererRight;

        protected Transformer mLeftAxisTransformer;
        protected Transformer mRightAxisTransformer;

        protected XAxisRenderer mXAxisRenderer;

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

        public override float YChartMax => Math.Max(AxisLeft.axisMaximum, AxisRight.axisMaximum);

        public override float YChartMin => Math.Min(AxisLeft.axisMinimum, AxisRight.axisMinimum);

        public override void Initialize()
        {
            base.Initialize();
            AxisLeft = new YAxis(YAxisDependency.Left);
            AxisRight = new YAxis(YAxisDependency.Right);

            mLeftAxisTransformer = new Transformer(ViewPortHandler);
            mRightAxisTransformer = new Transformer(ViewPortHandler);

            mAxisRendererLeft = new YAxisRenderer(ViewPortHandler, AxisLeft, mLeftAxisTransformer);
            mAxisRendererRight = new YAxisRenderer(ViewPortHandler, AxisRight, mRightAxisTransformer);

            mXAxisRenderer = new XAxisRenderer(ViewPortHandler, XAxis, mLeftAxisTransformer);
        }

        protected void PrepareOffsetMatrix()
        {
            mRightAxisTransformer.PrepareMatrixOffset(AxisRight.Inverted);
            mLeftAxisTransformer.PrepareMatrixOffset(AxisLeft.Inverted);
        }

        protected void PrepareValuePxMatrix()
        {
            mRightAxisTransformer.PrepareMatrixValuePx(XAxis.axisMinimum,
                    XAxis.AxisRange,
                    AxisRight.AxisRange,
                    AxisRight.axisMinimum);
            mLeftAxisTransformer.PrepareMatrixValuePx(XAxis.axisMinimum,
                    XAxis.AxisRange,
                    AxisLeft.AxisRange,
                    AxisLeft.axisMinimum);
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
            Legend legend = Legend;
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
                                        ViewPortHandler.ChartWidth * Legend.MaxSizePercent)
                                        + Legend.XOffset;
                                break;

                            case Components.HorizontalAlignment.Center:
                                switch (Legend.VerticalAlignment)
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

                        switch (Legend.VerticalAlignment)
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

            if (Renderer != null)
                Renderer.InitBuffers();

            CalcMinMax();

            mAxisRendererLeft.ComputeAxis(AxisLeft.axisMinimum, AxisLeft.axisMaximum, AxisLeft.Inverted);
            mAxisRendererRight.ComputeAxis(AxisRight.axisMinimum, AxisRight.axisMaximum, AxisRight.Inverted);
            mXAxisRenderer.ComputeAxis(XAxis.axisMinimum, XAxis.axisMaximum, false);

            if (Legend != null)
                LegendRenderer.ComputeLegend(data);

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
            CalcMinMax(Data);
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
                return mLeftAxisTransformer;
            else
                return mRightAxisTransformer;
        }


        public YAxis GetAxis(YAxisDependency axis)
        {
            if (axis == YAxisDependency.Left)
                return AxisLeft;
            else
                return AxisRight;
        }

        public bool IsInverted(YAxisDependency axis)
        {
            return axis == YAxisDependency.Left ? AxisLeft.Inverted : AxisRight.Inverted;
        }
    }
}
