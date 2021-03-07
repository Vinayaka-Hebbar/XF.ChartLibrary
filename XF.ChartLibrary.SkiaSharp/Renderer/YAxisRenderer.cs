using SkiaSharp;
using System;
using System.Collections.Generic;
using XF.ChartLibrary.Components;

namespace XF.ChartLibrary.Renderer
{
    partial class YAxisRenderer
    {
        protected SKPaint ZeroLinePaint;

        protected override void Initialize()
        {
            base.Initialize();
            AxisLabelPaint.Color = SKColors.Black;
            AxisLabelPaint.TextSize = 10f.DpToPixel();

            ZeroLinePaint = new SKPaint
            {
                IsAntialias = true,
                Color = SKColors.Gray,
                StrokeWidth = 1f,
                Style = SKPaintStyle.Stroke
            };
        }

        /// <summary>
        /// draws the y-axis labels to the screen
        /// </summary>
        public void RenderAxisLabels(SKCanvas c)
        {

            if (!YAxis.IsEnabled || !YAxis.IsDrawLabelsEnabled)
                return;

            SKPoint[] positions = GetTransformedPositions();

            AxisLabelPaint.Typeface = YAxis.Typeface;
            AxisLabelPaint.TextSize = YAxis.TextSize;
            AxisLabelPaint.Color = YAxis.TextColor;

            float xoffset = YAxis.XOffset;
            float yoffset = AxisLabelPaint.MeasureHeight("A") / 2.5f + YAxis.YOffset;

            YAxisDependency dependency = YAxis.AxisDependency;
            YAxis.YAxisLabelPosition labelPosition = YAxis.Position;

            float xPos;
            if (dependency == YAxisDependency.Left)
            {

                if (labelPosition == YAxis.YAxisLabelPosition.OutSideChart)
                {
                    AxisLabelPaint.TextAlign = SKTextAlign.Right;
                    xPos = ViewPortHandler.OffsetLeft - xoffset;
                }
                else
                {
                    AxisLabelPaint.TextAlign = SKTextAlign.Left;
                    xPos = ViewPortHandler.OffsetLeft + xoffset;
                }

            }
            else
            {

                if (labelPosition == YAxis.YAxisLabelPosition.OutSideChart)
                {
                    AxisLabelPaint.TextAlign = SKTextAlign.Left;
                    xPos = ViewPortHandler.ContentRight + xoffset;
                }
                else
                {
                    AxisLabelPaint.TextAlign = SKTextAlign.Right;
                    xPos = ViewPortHandler.ContentRight - xoffset;
                }
            }

            DrawYLabels(c, xPos, positions, yoffset);
        }

        public void RenderAxisLine(SKCanvas c)
        {

            if (!YAxis.IsEnabled || !YAxis.IsDrawAxisLineEnabled)
                return;

            AxisLinePaint.Color = YAxis.AxisLineColor;
            AxisLinePaint.StrokeWidth = YAxis.AxisLineWidth;

            if (YAxis.AxisDependency == YAxisDependency.Left)
            {
                c.DrawLine(ViewPortHandler.ContentLeft, ViewPortHandler.ContentTop, ViewPortHandler.ContentLeft,
                        ViewPortHandler.ContentBottom, AxisLinePaint);
            }
            else
            {
                c.DrawLine(ViewPortHandler.ContentRight, ViewPortHandler.ContentTop, ViewPortHandler.ContentRight,
                        ViewPortHandler.ContentBottom, AxisLinePaint);
            }
        }

        /// <summary>
        /// draws the y-labels on the specified x-position
        /// </summary>
        protected void DrawYLabels(SKCanvas c, float fixedPosition, SKPoint[] positions, float offset)
        {

            int from = YAxis.DrawBottomYLabelEntry ? 0 : 1;
            int to = YAxis.DrawTopYLabelEntry
                    ? YAxis.entryCount
                    : (YAxis.entryCount - 1);

            float xOffset = YAxis.XLabelOffset;

            // draw
            for (int i = from; i < to; i++)
            {
                string text = YAxis.GetFormattedLabel(i);

                c.DrawText(text,
                        fixedPosition + xOffset,
                        positions[i].Y + offset,
                        AxisLabelPaint);
            }
        }

        protected SKPath RenderGridLinesPath = new SKPath();

        public void RenderGridLines(SKCanvas c)
        {

            if (!YAxis.IsEnabled)
                return;

            if (YAxis.IsDrawGridLinesEnabled)
            {

                int clipRestoreCount = c.Save();
                c.ClipRect(GetGridClippingRect());

                SKPoint[] positions = GetTransformedPositions();

                GridPaint.Color = YAxis.GridColor;
                GridPaint.StrokeWidth = YAxis.GridLineWidth;
                GridPaint.PathEffect = YAxis.GridDashedLine;

                SKPath gridLinePath = RenderGridLinesPath;
                gridLinePath.Reset();

                // draw the grid
                foreach (SKPoint pos in positions)
                {
                    // draw a path because lines don't support dashing on lower android versions
                    c.DrawPath(LinePath(gridLinePath, pos), GridPaint);
                    gridLinePath.Reset();
                }

                c.RestoreToCount(clipRestoreCount);
            }

            if (YAxis.DrawZeroLine)
            {
                DrawZeroLine(c);
            }
        }

        public SKRect GetGridClippingRect()
        {
            return ViewPortHandler.ContentRect.InsetVertically(Axis.GridLineWidth);
        }

        /// <summary>
        /// Calculates the path for a grid line.
        /// </summary>
        protected SKPath LinePath(SKPath p, SKPoint position)
        {

            p.MoveTo(ViewPortHandler.OffsetLeft, position.Y);
            p.LineTo(ViewPortHandler.ContentRight, position.Y);

            return p;
        }

        protected SKPoint[] TransformedPositionsBuffer = new SKPoint[2];

        /// <summary>
        /// Transforms the values contained in the axis entries to screen pixels and returns them in form of a float array
        /// of x- and y-coordinates.
        /// </summary>
        /// <returns></returns>
        protected SKPoint[] GetTransformedPositions()
        {

            if (TransformedPositionsBuffer.Length != YAxis.entryCount)
            {
                TransformedPositionsBuffer = new SKPoint[YAxis.entryCount];
            }
            SKPoint[] positions = TransformedPositionsBuffer;

            for (int i = 0; i < positions.Length; i++)
            {
                // only fill y values, x values are not needed for y-labels
                positions[i].Y = YAxis.entries[i];
            }

            return Trasformer.PointValuesToPixel(positions);
        }

        protected SKPath DrawZeroLinePath = new SKPath();

        /// <summary>
        /// Draws the zero line.
        /// </summary>
        protected void DrawZeroLine(SKCanvas c)
        {

            int clipRestoreCount = c.Save();
            SKRect rect = ViewPortHandler.ContentRect.InsetVertically(YAxis.ZeroLineWidth);
            c.ClipRect(rect);

            // draw zero line
            SKPoint pos = Trasformer.PixelsToValue(0f, 0f);

            ZeroLinePaint.Color = YAxis.ZeroLineColor;
            ZeroLinePaint.StrokeWidth = YAxis.ZeroLineWidth;

            SKPath zeroLinePath = DrawZeroLinePath;
            zeroLinePath.Reset();

            zeroLinePath.MoveTo(ViewPortHandler.ContentLeft, (float)pos.Y);
            zeroLinePath.LineTo(ViewPortHandler.ContentRight, (float)pos.Y);

            // draw a path because lines don't support dashing on lower android versions
            c.DrawPath(zeroLinePath, ZeroLinePaint);

            c.RestoreToCount(clipRestoreCount);
        }

        protected SKPath RenderLimitLinesPath = new SKPath();

        public void RenderLimitLines(SKCanvas c)
        {
            IList<LimitLine> limitLines = YAxis.LimitLines;

            if (limitLines == null || limitLines.Count <= 0)
                return;

            SKPath limitLinePath = RenderLimitLinesPath;
            limitLinePath.Reset();

            foreach (LimitLine l in limitLines)
            {
                if (!l.IsEnabled)
                    continue;

                int clipRestoreCount = c.Save();
                SKRect mLimitLineClippingRect = ViewPortHandler.ContentRect.InsetVertically(l.LineWidth);
                c.ClipRect(mLimitLineClippingRect);

                LimitLinePaint.Style = SKPaintStyle.Stroke;
                LimitLinePaint.Color = l.LineColor;
                LimitLinePaint.StrokeWidth = l.LineWidth;
                LimitLinePaint.PathEffect = l.DashPathEffect;

                SKPoint pt = Trasformer.PointValueToPixel(0f, l.Limit);

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
                    LimitLinePaint.Typeface = l.Typeface;
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
