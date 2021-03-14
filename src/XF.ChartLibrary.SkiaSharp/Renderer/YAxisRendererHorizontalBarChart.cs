using SkiaSharp;
using System.Collections.Generic;
using XF.ChartLibrary.Components;

namespace XF.ChartLibrary.Renderer
{
    partial class YAxisRendererHorizontalBarChart
    {
        protected override void Initialize()
        {
            base.Initialize();
            LimitLinePaint.TextAlign = SKTextAlign.Left;
        }

        public override void RenderAxisLabels(SKCanvas c)
        {
            if (!YAxis.IsEnabled || !YAxis.IsDrawLabelsEnabled)
                return;

            SKPoint[] positions = GetTransformedPositions();

            AxisLabelPaint.Typeface = YAxis.Typeface;
            AxisLabelPaint.TextSize = YAxis.TextSize;
            AxisLabelPaint.Color = YAxis.TextColor;
            AxisLabelPaint.TextAlign = SKTextAlign.Center;

            float baseYOffset = 2.5f.DpToPixel();
            float textHeight = AxisLabelPaint.MeasureHeight("Q");

            YAxisDependency dependency = YAxis.AxisDependency;
            YAxis.YAxisLabelPosition labelPosition = YAxis.Position;

            float yPos;
            if (dependency == YAxisDependency.Left)
            {
                if (labelPosition == YAxis.YAxisLabelPosition.OutSideChart)
                {
                    yPos = ViewPortHandler.ContentTop - baseYOffset;
                }
                else
                {
                    yPos = ViewPortHandler.ContentTop - baseYOffset;
                }
            }
            else
            {
                if (labelPosition == YAxis.YAxisLabelPosition.OutSideChart)
                {
                    yPos = ViewPortHandler.ContentBottom + textHeight + baseYOffset;
                }
                else
                {
                    yPos = ViewPortHandler.ContentBottom + textHeight + baseYOffset;
                }
            }

            DrawYLabels(c, yPos, positions, YAxis.YOffset);
        }

        public override void RenderAxisLine(SKCanvas c)
        {
            if (!YAxis.IsEnabled || !YAxis.IsDrawAxisLineEnabled)
                return;

            AxisLinePaint.Color = YAxis.AxisLineColor;
            AxisLinePaint.StrokeWidth = YAxis.AxisLineWidth;

            if (YAxis.AxisDependency == YAxisDependency.Left)
            {
                c.DrawLine(ViewPortHandler.ContentLeft, ViewPortHandler.ContentTop, ViewPortHandler.ContentRight,
                        ViewPortHandler.ContentTop, AxisLinePaint);
            }
            else
            {
                c.DrawLine(ViewPortHandler.ContentLeft, ViewPortHandler.ContentBottom, ViewPortHandler.ContentRight,
                        ViewPortHandler.ContentBottom, AxisLinePaint);
            }
        }

        protected override void DrawYLabels(SKCanvas c, float fixedPosition, SKPoint[] positions, float offset)
        {
            AxisLabelPaint.Typeface = YAxis.Typeface;
            AxisLabelPaint.TextSize = YAxis.TextSize;
            AxisLabelPaint.Color = YAxis.TextColor;

            int from = YAxis.DrawBottomYLabelEntry ? 0 : 1;
            int to = YAxis.DrawTopYLabelEntry
                    ? YAxis.entryCount
                    : (YAxis.entryCount - 1);

            float xOffset = YAxis.XLabelOffset;

            // draw
            for (int i = from; i < to; i++)
            {
                c.DrawText(YAxis.GetFormattedLabel(i),
                        positions[i].X,
                        fixedPosition - offset + xOffset,
                        AxisLabelPaint);
            }
        }

        protected override SKPoint[] GetTransformedPositions()
        {
            if (TransformedPositionsBuffer.Length != YAxis.entryCount)
            {
                TransformedPositionsBuffer = new SKPoint[YAxis.entryCount];
            }
            SKPoint[] positions = TransformedPositionsBuffer;

            for (int i = 0; i < positions.Length; i++)
            {
                // only fill y values, x values are not needed for y-labels
                positions[i].X = YAxis.entries[i];
            }

            return Trasformer.PointValuesToPixel(positions);
        }

        public override SKRect GetGridClippingRect()
        {
            return ViewPortHandler.ContentRect.InsetHorizontally(Axis.GridLineWidth);
        }

        protected override SKPath LinePath(SKPath p, SKPoint position)
        {
            p.MoveTo(position.X, ViewPortHandler.ContentTop);
            p.LineTo(position.X, ViewPortHandler.ContentBottom);

            return p;
        }

        protected override void DrawZeroLine(SKCanvas c)
        {
            int clipRestoreCount = c.Save();
            c.ClipRect(ViewPortHandler.ContentRect.InsetHorizontally(YAxis.ZeroLineWidth));

            // draw zero line
            SKPoint pos = Trasformer.PixelsToValue(0f, 0f);

            ZeroLinePaint.Color = YAxis.ZeroLineColor;
            ZeroLinePaint.StrokeWidth = YAxis.ZeroLineWidth;

            SKPath zeroLinePath = DrawZeroLinePath;
            zeroLinePath.Reset();

            zeroLinePath.MoveTo((float)pos.X - 1, ViewPortHandler.ContentTop);
            zeroLinePath.LineTo((float)pos.X - 1, ViewPortHandler.ContentBottom);

            // draw a path because lines don't support dashing on lower android versions
            c.DrawPath(zeroLinePath, ZeroLinePaint);

            c.RestoreToCount(clipRestoreCount);
        }

        public override void RenderLimitLines(SKCanvas c)
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
                // in android dx is inverted (-ve)
                SKRect limitLineClippingRect = ViewPortHandler.ContentRect.InsetHorizontally(l.LineWidth);
                c.ClipRect(limitLineClippingRect);

                LimitLinePaint.Style = SKPaintStyle.Stroke;
                LimitLinePaint.Color = l.LineColor;
                LimitLinePaint.StrokeWidth = l.LineWidth;
                LimitLinePaint.PathEffect = l.DashPathEffect;

                SKPoint pt = Trasformer.PointValueToPixel(l.Limit, 0);

                limitLinePath.MoveTo(pt.X, ViewPortHandler.ContentTop);
                limitLinePath.LineTo(pt.X, ViewPortHandler.ContentBottom);

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

                    float xOffset = l.LineWidth + l.XOffset;
                    float yOffset = 2f.DpToPixel() + l.YOffset;

                    LimitLine.LimitLabelPosition position = l.LabelPosition;

                    if (position == LimitLine.LimitLabelPosition.RightTop)
                    {
                        float labelLineHeight = LimitLinePaint.MeasureHeight(label);
                        LimitLinePaint.TextAlign = SKTextAlign.Left;
                        c.DrawText(label,
                                pt.X + xOffset, ViewPortHandler.ContentTop + yOffset + labelLineHeight, LimitLinePaint);

                    }
                    else if (position == LimitLine.LimitLabelPosition.RightBottom)
                    {

                        LimitLinePaint.TextAlign = SKTextAlign.Left;
                        c.DrawText(label,
                                pt.X + xOffset,
                                ViewPortHandler.ContentBottom - yOffset, LimitLinePaint);

                    }
                    else if (position == LimitLine.LimitLabelPosition.LeftTop)
                    {
                        float labelLineHeight = LimitLinePaint.MeasureHeight(label);
                        LimitLinePaint.TextAlign = SKTextAlign.Right;
                        c.DrawText(label,
                                pt.X - xOffset,
                                ViewPortHandler.ContentTop + yOffset + labelLineHeight, LimitLinePaint);

                    }
                    else
                    {
                        LimitLinePaint.TextAlign = SKTextAlign.Right;
                        c.DrawText(label,
                                 pt.X - xOffset,
                               ViewPortHandler.ContentBottom - yOffset, LimitLinePaint);
                    }
                }

                c.RestoreToCount(clipRestoreCount);
            }
        }
    }
}
