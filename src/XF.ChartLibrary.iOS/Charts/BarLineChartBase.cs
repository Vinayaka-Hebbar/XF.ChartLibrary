using System;
using XF.ChartLibrary.Components;

namespace XF.ChartLibrary.Charts
{
    partial class BarLineChartBase<TData, TDataSet>
    {

        protected BarLineChartBase()
        {
            AxisLeft = new YAxis(YAxisDependency.Left);
            AxisRight = new YAxis(YAxisDependency.Right);
        }

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


        public override int MaxVisibleCount { get; set; } = 100;

        /// <summary>
        /// the object representing the labels on the left y-axis
        /// </summary>
        public YAxis AxisLeft { get; set; }

        /// <summary>
        /// the object representing the labels on the right y-axis
        /// </summary>
        public YAxis AxisRight { get; set; }

        protected override void CalculateOffsets()
        {
            if (!customViewPortEnabled)
            {

                var offset = CalculateLegendOffsets();

                var offsetLeft = (float)offset.Left;
                var offsetRight = (float)offset.Right;
                var offsetTop = (float)offset.Top;
                var offsetBottom = (float)offset.Bottom;

                // offsets for y-labels
                if (AxisLeft.NeedsOffset)
                {
                    offsetLeft += AxisLeft.RequiredSize().Width;
                }

                if (AxisRight.NeedsOffset)
                {
                    offsetRight += AxisRight.RequiredSize().Width;
                }

                if (XAxis.IsEnabled && XAxis.IsDrawLabelsEnabled)
                {
                    var xlabelheight = XAxis.LabelRotatedHeight + XAxis.YOffset;

                    // offsets for x-labels
                    if (XAxis.Position == XAxis.XAxisPosition.Bottom)
                    {
                        offsetBottom += xlabelheight;
                    }
                    else if (XAxis.Position == XAxis.XAxisPosition.Top)
                    {
                        offsetTop += xlabelheight;
                    }
                    else if (XAxis.Position == XAxis.XAxisPosition.BothSided)
                    {
                        offsetBottom += xlabelheight;
                        offsetTop += xlabelheight;
                    }
                }

                offsetTop += ExtraTopOffset;
                offsetRight += ExtraRightOffset;
                offsetBottom += ExtraBottomOffset;
                offsetLeft += ExtraLeftOffset;
                var minOffset = MinOffset;

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
