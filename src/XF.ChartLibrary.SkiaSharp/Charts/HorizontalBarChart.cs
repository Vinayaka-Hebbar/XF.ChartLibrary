using SkiaSharp;
using System;
using XF.ChartLibrary.Components;
using SKHorizontalAlignment = XF.ChartLibrary.Components.HorizontalAlignment;
using SKVerticalAlignment = XF.ChartLibrary.Components.VerticalAlignment;

namespace XF.ChartLibrary.Charts
{
    partial class HorizontalBarChart
    {
        protected override SKRect CalculateLegendOffsets()
        {
            float left = 0.0f;
            float right = 0.0f;
            float top = 0.0f;
            float bottom = 0.0f;
            Legend legend = Legend;
            if (legend != null && legend.IsEnabled && legend.IsDrawInsideEnabled == false)
            {
                switch (legend.Orientation)
                {
                    case Orientation.Vertical:

                        switch (legend.HorizontalAlignment)
                        {
                            case SKHorizontalAlignment.Left:
                                left = Math.Min(legend.NeededWidth,
                                        ViewPortHandler.ChartWidth * legend.MaxSizePercent)
                                        + legend.XOffset;
                                break;

                            case SKHorizontalAlignment.Right:
                                right = Math.Min(legend.NeededWidth,
                                        ViewPortHandler.ChartWidth * legend.MaxSizePercent)
                                        + legend.XOffset;
                                break;

                            case SKHorizontalAlignment.Center:
                                switch (legend.VerticalAlignment)
                                {
                                    case SKVerticalAlignment.Top:
                                        top = Math.Min(legend.NeededHeight,
                                                ViewPortHandler.ChartHeight * legend.MaxSizePercent)
                                                + legend.YOffset;
                                        break;

                                    case SKVerticalAlignment.Bottom:
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
                            case SKVerticalAlignment.Top:
                                top = Math.Min(legend.NeededHeight,
                                        ViewPortHandler.ChartHeight * legend.MaxSizePercent)
                                        + legend.YOffset;
                                if (AxisLeft is YAxis axisLeft && axisLeft.IsEnabled && axisLeft.IsDrawLabelsEnabled)
                                    top += axisLeft.GetRequiredWidthSpace(axisRendererLeft.AxisLabelPaint);
                                break;

                            case SKVerticalAlignment.Bottom:
                                bottom = Math.Min(legend.NeededHeight,
                                        ViewPortHandler.ChartHeight * legend.MaxSizePercent)
                                        + legend.YOffset;

                                if (AxisRight is YAxis axisRight && axisRight.IsEnabled && axisRight.IsDrawLabelsEnabled)
                                    bottom += axisRight.GetRequiredWidthSpace(axisRendererRight.AxisLabelPaint);
                                break;

                            default:
                                break;
                        }
                        break;
                }
            }
            return new SKRect(top, left, right, bottom);
        }

        public override void CalculateOffsets()
        {
            var offset = CalculateLegendOffsets();

            var offsetLeft = offset.Left;
            var offsetTop = offset.Top;
            var offsetRight = offset.Right;
            var offsetBottom = offset.Bottom;

            // offsets for y-labels
            if (AxisLeft.NeedsOffset)
            {
                offsetTop += AxisLeft.GetRequiredHeightSpace(axisRendererLeft
                        .AxisLabelPaint);
            }

            if (AxisRight.NeedsOffset)
            {
                offsetBottom += AxisRight.GetRequiredHeightSpace(axisRendererRight.AxisLabelPaint);
            }

            if (XAxis.IsEnabled && XAxis.IsDrawLabelsEnabled)
            {

                float xLabelWidth = XAxis.LabelRotatedWidth;

                // offsets for x-labels
                if (XAxis.Position == XAxis.XAxisPosition.Bottom)
                {

                    offsetLeft += xLabelWidth;

                }
                else if (XAxis.Position == XAxis.XAxisPosition.Top)
                {

                    offsetRight += xLabelWidth;

                }
                else if (XAxis.Position == XAxis.XAxisPosition.BottomInside)
                {

                    offsetBottom += xLabelWidth;
                    offsetTop += xLabelWidth;
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



            PrepareOffsetMatrix();
            PrepareValuePxMatrix();
        }

        protected override void PrepareValuePxMatrix()
        {
            RightAxisTransformer.PrepareMatrixValuePx(AxisRight.axisMinimum,
                   AxisRight.axisRange,
                   XAxis.axisRange,
                   XAxis.axisMinimum);
            LeftAxisTransformer.PrepareMatrixValuePx(AxisLeft.axisMinimum,
                    AxisLeft.axisRange,
                    XAxis.axisRange,
                    XAxis.axisMinimum);
        }

        protected override SKPoint GetMarkerPosition(Highlight.Highlight value)
        {
            return new SKPoint(value.DrawY, value.DrawX);
        }

        public override Highlight.Highlight GetHighlightByTouchPoint(float x, float y)
        {
            if (data == null)
            {
                System.Diagnostics.Trace.TraceError("Can't select by touch. No data set.");
                return null;
            }
            else
            {
                return Highlighter.GetHighlight(y, x);
            }
        }

    }
}