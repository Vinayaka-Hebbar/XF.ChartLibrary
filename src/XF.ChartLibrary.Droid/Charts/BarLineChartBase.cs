using Android.Content;
using Android.Graphics;
using System;
using XF.ChartLibrary.Components;

namespace XF.ChartLibrary.Charts
{
    partial class BarLineChartBase<TData, TDataSet>
    {
        /// <summary>
        /// flag that indicates if pinch-zoom is enabled. if true, both x and y axis
        /// can be scaled with 2 fingers, if false, x and y axis can be scaled
        /// separately
        /// </summary>
        protected bool pinchZoomEnabled = false;

        /// <summary>
        ///  flag that indicates if double tap zoom is enabled or not
        /// </summary>
        protected bool doubleTapToZoomEnabled = true;

        /// <summary>
        /// flag that indicates if highlighting per dragging over a fully zoomed out
        /// chart is enabled
        /// </summary>
        protected bool highlightPerDragEnabled = true;

        /// <summary>
        /// if true, dragging is enabled for the chart
        /// </summary>
        private bool mDragXEnabled = true;
        private bool mDragYEnabled = true;

        private bool mScaleXEnabled = true;
        private bool mScaleYEnabled = true;

        /// <summary>
        /// flag indicating if the grid background should be drawn or not
        /// </summary>
        public bool IsDrawGridBackground { get; set; }

        public bool DrawBorders { get; set; }

        /// <summary>
        ///  flag that indicates if auto scaling on the y axis is enabled
        /// </summary>
        public bool AutoScaleMinMaxEnabled { get; set; }

        public bool ClipDataToContent { get; set; } = true;

        public bool ClipValuesToContent { get; set; }
        /// <summary>
        /// Sets the minimum offset (padding) around the chart, defaults to 15
        /// </summary>
        public float MinOffset { get; set; } = 15.0f;



        protected BarLineChartBase(Context context) : base(context)
        {
            AxisLeft = new YAxis(YAxisDependency.Left);
            AxisRight = new YAxis(YAxisDependency.Right);
        }

        public override int MaxVisibleCount { get; set; } = 100;

        /// <summary>
        /// the object representing the labels on the left y-axis
        /// </summary>
        public YAxis AxisLeft { get; set; }

        /// <summary>
        /// the object representing the labels on the right y-axis
        /// </summary>
        public YAxis AxisRight { get; set; }

        private readonly RectF offset = new RectF();


        protected override void CalculateOffsets()
        {
            if (!customViewPortEnabled)
            {
                CalculateLegendOffsets(offset);

                var offsetLeft = offset.Left;
                var offsetTop = offset.Top;
                var offsetRight = offset.Right;
                var offsetBottom = offset.Bottom;

                // offsets for y-labels
                if (AxisLeft.NeedsOffset)
                {
                    offsetLeft += AxisLeft.GetRequiredWidthSpace(axisRendererLeft
                            .AxisLabelPaint);
                }

                if (AxisRight.NeedsOffset)
                {
                    offsetRight += AxisRight.GetRequiredWidthSpace(axisRendererRight.AxisLabelPaint);
                }

                if (XAxis.IsEnabled && XAxis.IsDrawLabelsEnabled)
                {

                    float xLabelHeight = XAxis.LabelRotatedHeight + XAxis.YOffset;

                    // offsets for x-labels
                    if (XAxis.Position == XAxis.XAxisPosition.Bottom)
                    {

                        offsetBottom += xLabelHeight;

                    }
                    else if (XAxis.Position == XAxis.XAxisPosition.Top)
                    {

                        offsetTop += xLabelHeight;

                    }
                    else if (XAxis.Position == XAxis.XAxisPosition.BottomInside)
                    {

                        offsetBottom += xLabelHeight;
                        offsetTop += xLabelHeight;
                    }
                }

                offsetTop += ExtraTopOffset;
                offsetRight += ExtraRightOffset;
                offsetBottom += ExtraBottomOffset;
                offsetLeft += ExtraLeftOffset;

                float minOffset = MinOffset.DpToPixel();

                ViewPortHandler.RestrainViewPort(
                        Math.Max(minOffset, offsetLeft),
                        Math.Max(minOffset, offsetTop),
                        Math.Max(minOffset, offsetRight),
                        Math.Max(minOffset, offsetBottom));
            }

            PrepareOffsetMatrix();
            PrepareValuePxMatrix();
        }
    }
}
