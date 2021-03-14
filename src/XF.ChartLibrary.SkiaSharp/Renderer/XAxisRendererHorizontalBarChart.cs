using SkiaSharp;
using System;
using XF.ChartLibrary.Components;

namespace XF.ChartLibrary.Renderer
{
    partial class XAxisRendererHorizontalBarChart
    {
        protected override void ComputeSize()
        {
            var longest = XAxis.GetLongestLabel();

            AxisLabelPaint.Typeface = XAxis.Typeface;
            AxisLabelPaint.TextSize = XAxis.TextSize;

            var labelSize = AxisLabelPaint.Measure(longest);

            float labelWidth = labelSize.Width + XAxis.XOffset * 3.5f;
            float labelHeight = labelSize.Height;

            var labelRotatedSize = ChartUtil.GetSizeOfRotatedRectangleByDegrees(
                    labelSize.Width,
                    labelHeight,
                    XAxis.LabelRotationAngle);


            XAxis.LabelWidth = (int)Math.Round(labelWidth);
            XAxis.LabelHeight = (int)Math.Round(labelHeight);
            XAxis.LabelRotatedWidth = (int)MathF.Round(labelRotatedSize.Width + XAxis.XOffset * 3.5f);
            XAxis.LabelRotatedHeight = (int)MathF.Round(labelRotatedSize.Height);
        }

        public override void RenderAxisLabels(SKCanvas c)
        {
            if (!XAxis.IsEnabled || !XAxis.IsDrawLabelsEnabled)
                return;

            float xoffset = XAxis.XOffset;

            AxisLabelPaint.Typeface = XAxis.Typeface;
            AxisLabelPaint.TextSize = XAxis.TextSize;
            AxisLabelPaint.Color = XAxis.TextColor;

            switch (XAxis.Position)
            {
                case XAxis.XAxisPosition.Top:
                    DrawLabels(c, ViewPortHandler.ContentRight + xoffset, new SKPoint(0.0f, 0.5f));

                    break;
                case XAxis.XAxisPosition.TopInside:
                    DrawLabels(c, ViewPortHandler.ContentRight - xoffset, new SKPoint(1.0f, 0.5f));

                    break;
                case XAxis.XAxisPosition.Bottom:
                    DrawLabels(c, ViewPortHandler.ContentLeft - xoffset, new SKPoint(1.0f, 0.5f));

                    break;
                case XAxis.XAxisPosition.BottomInside:
                    DrawLabels(c, ViewPortHandler.ContentLeft + xoffset, new SKPoint(1.0f, 0.5f));

                    break;
                default:
                    // BOTH SIDED
                    DrawLabels(c, ViewPortHandler.ContentRight + xoffset, new SKPoint(0.0f, 0.5f));
                    DrawLabels(c, ViewPortHandler.ContentLeft - xoffset, new SKPoint(1.0f, 0.5f));
                    break;
            }
        }

        protected override void DrawLabels(SKCanvas c, float pos, SKPoint anchor)
        {
            float labelRotationAngleDegrees = XAxis.LabelRotationAngle;
            var centeringEnabled = XAxis.IsCenterAxisLabelsEnabled;

            SKPoint[] positions = new SKPoint[XAxis.entryCount];

            for (int i = 0; i < positions.Length; i++)
            {
                // only fill x values
                if (centeringEnabled)
                {
                    positions[i].Y = XAxis.centeredEntries[i];
                }
                else
                {
                    positions[i].Y = XAxis.entries[i];
                }
            }

            positions = Trasformer.PointValuesToPixel(positions);

            for (int i = 0; i < positions.Length; i++)
            {
                float y = positions[i].Y;

                if (ViewPortHandler.IsInBoundsY(y))
                {
                    DrawLabel(c, XAxis.ValueFormatter.GetFormattedValue(XAxis.entries[i], XAxis), pos, y, anchor, labelRotationAngleDegrees);
                }
            }
        }

        public override SKRect GetGridClippingRect()
        {
            return ViewPortHandler.ContentRect.InsetVertically(Axis.GridLineWidth);
        }

        protected override void DrawGridLine(SKCanvas c, SKPoint pos, SKPath gridLinePath)
        {
            gridLinePath.MoveTo(ViewPortHandler.ContentRight, pos.Y);
            gridLinePath.LineTo(ViewPortHandler.ContentLeft, pos.Y);

            // draw a path because lines don't support dashing on lower android versions
            c.DrawPath(gridLinePath, GridPaint);

            gridLinePath.Reset();
        }

        public override void RenderAxisLine(SKCanvas c)
        {

            if (!XAxis.IsDrawAxisLineEnabled || !XAxis.IsEnabled)
                return;

            AxisLinePaint.Color = XAxis.AxisLineColor;
            AxisLinePaint.StrokeWidth = XAxis.AxisLineWidth;

            switch (XAxis.Position)
            {
                case XAxis.XAxisPosition.Top:
                case XAxis.XAxisPosition.TopInside:
                    c.DrawLine(ViewPortHandler.ContentRight, ViewPortHandler.ContentTop, ViewPortHandler.ContentRight, ViewPortHandler.ContentBottom, AxisLinePaint);
                    break;
                case XAxis.XAxisPosition.Bottom:
                case XAxis.XAxisPosition.BottomInside:
                    c.DrawLine(ViewPortHandler.ContentLeft, ViewPortHandler.ContentTop, ViewPortHandler.ContentLeft, ViewPortHandler.ContentBottom, AxisLinePaint);
                    break;
                case XAxis.XAxisPosition.BothSided:

                    c.DrawLine(ViewPortHandler.ContentRight, ViewPortHandler.ContentTop, ViewPortHandler.ContentRight, ViewPortHandler.ContentBottom, AxisLinePaint);
                    c.DrawLine(ViewPortHandler.ContentLeft, ViewPortHandler.ContentTop, ViewPortHandler.ContentLeft, ViewPortHandler.ContentBottom, AxisLinePaint);
                    break;
            }
        }

        public override void RenderLimitLines(SKCanvas c)
        {
            var limitLines = XAxis.LimitLines;

            if (limitLines == null || limitLines.Count <= 0)
                return;
            SKPath limitLinePath = LimitLinePath;
            limitLinePath.Reset();

            for (int i = 0; i < limitLines.Count; i++)
            {

                LimitLine l = limitLines[i];

                if (!l.IsEnabled)
                    continue;

                int clipRestoreCount = c.Save();
                c.ClipRect(ViewPortHandler.ContentRect.InsetVertically(l.LineWidth));

                LimitLinePaint.Style = SKPaintStyle.Stroke;
                LimitLinePaint.Color = l.LineColor;
                LimitLinePaint.StrokeWidth = l.LineWidth;
                LimitLinePaint.PathEffect = l.DashPathEffect;

                var pt = Trasformer.PointValueToPixel(0.0f, l.Limit);

                limitLinePath.MoveTo(ViewPortHandler.ContentLeft, pt.Y);
                limitLinePath.LineTo(ViewPortHandler.ContentRight, pt.Y);

                c.DrawPath(limitLinePath, LimitLinePaint);
                limitLinePath.Reset();
                // c.drawLines(pts, mLimitLinePaint);

                string label = l.Label;

                // if drawing the limit-value label is enabled
                if (string.IsNullOrEmpty(label) == false)
                {
                    LimitLinePaint.Style = l.TextStyle;
                    LimitLinePaint.PathEffect = null;
                    LimitLinePaint.Color = l.TextColor;
                    LimitLinePaint.StrokeWidth = 0.5f;
                    LimitLinePaint.TextSize = l.TextSize;

                    float labelLineHeight = LimitLinePaint.MeasureHeight(label);
                    float xOffset = 4f.DpToPixel() + l.XOffset;
                    float yOffset = l.LineWidth + labelLineHeight + l.YOffset;

                    LimitLine.LimitLabelPosition position = l.LabelPosition;

                    if (position == LimitLine.LimitLabelPosition.RightTop)
                    {
                        LimitLinePaint.TextAlign = SKTextAlign.Right;
                        c.DrawText(label,
                                ViewPortHandler.ContentRight - xOffset,
                                pt.Y - yOffset + labelLineHeight, LimitLinePaint);

                    }
                    else if (position == LimitLine.LimitLabelPosition.RightBottom)
                    {

                        LimitLinePaint.TextAlign = SKTextAlign.Right;
                        c.DrawText(label,
                                ViewPortHandler.ContentRight - xOffset,
                                pt.Y + yOffset, LimitLinePaint);

                    }
                    else if (position == LimitLine.LimitLabelPosition.LeftTop)
                    {

                        LimitLinePaint.TextAlign = SKTextAlign.Left;
                        c.DrawText(label,
                                ViewPortHandler.ContentLeft + xOffset,
                                pt.Y - yOffset + labelLineHeight, LimitLinePaint);

                    }
                    else
                    {
                        LimitLinePaint.TextAlign = SKTextAlign.Left;
                        c.DrawText(label,
                                ViewPortHandler.OffsetLeft + xOffset,
                                pt.Y + yOffset, LimitLinePaint);
                    }
                }

                c.RestoreToCount(clipRestoreCount);
            }
        }
    }
}
