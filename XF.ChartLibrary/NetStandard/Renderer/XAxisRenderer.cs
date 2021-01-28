using SkiaSharp;
using System;
using XF.ChartLibrary.Components;

namespace XF.ChartLibrary.Renderer
{
    partial class XAxisRenderer
    {
        protected override void Initialize()
        {
            base.Initialize();
            AxisLabelPaint.Color = SKColors.Black;
            AxisLabelPaint.TextAlign = SKTextAlign.Center;
            AxisLabelPaint.TextSize = 10f;
        }

        protected void SetupGridPaint()
        {
            GridPaint.Color = XAxis.GridColor;
            GridPaint.StrokeWidth = XAxis.GridLineWidth;
            GridPaint.PathEffect = XAxis.GridDashedLine;
        }

        partial void ComputeSize()
        {
            var longest = XAxis.GetLongestLabel();

            AxisLabelPaint.Typeface = XAxis.Typeface;
            AxisLabelPaint.TextSize = XAxis.TextSize;

            var labelSize = AxisLabelPaint.Measure(longest);

            float labelWidth = labelSize.Width;
            float labelHeight = AxisLabelPaint.MeasureHeight("Q");

            var labelRotatedSize = ChartUtil.GetSizeOfRotatedRectangleByDegrees(
                    labelWidth,
                    labelHeight,
                    XAxis.LabelRotationAngle);


            XAxis.LabelWidth = (int)Math.Round(labelWidth);
            XAxis.LabelHeight = (int)Math.Round(labelHeight);
            XAxis.LabelRotatedWidth = (int)MathF.Round(labelRotatedSize.Width);
            XAxis.LabelRotatedHeight = (int)MathF.Round(labelRotatedSize.Height);
        }

        public void RenderAxisLabels(SKCanvas c)
        {

            if (!XAxis.IsEnabled || !XAxis.IsDrawLabelsEnabled)
                return;

            float yoffset = XAxis.YOffset;

            AxisLabelPaint.Typeface = XAxis.Typeface;
            AxisLabelPaint.TextSize = XAxis.TextSize;
            AxisLabelPaint.Color = XAxis.TextColor;

            switch (XAxis.Position)
            {
                case XAxis.XAxisPosition.Top:
                    DrawLabels(c, ViewPortHandler.ContentTop - yoffset, new SKPoint(0.5f, 1.0f));

                    break;
                case XAxis.XAxisPosition.TopInside:
                    DrawLabels(c, ViewPortHandler.ContentTop + yoffset + XAxis.LabelRotatedHeight, new SKPoint(0.5f, 1.0f));

                    break;
                case XAxis.XAxisPosition.Bottom:
                    DrawLabels(c, ViewPortHandler.ContentBottom + yoffset, new SKPoint(0.5f, 0.0f));

                    break;
                case XAxis.XAxisPosition.BottomInside:
                    DrawLabels(c, ViewPortHandler.ContentBottom - yoffset - XAxis.LabelRotatedHeight, new SKPoint(0.5f, 0.0f));

                    break;
                default:
                    // BOTH SIDED
                    DrawLabels(c, ViewPortHandler.ContentTop - yoffset, new SKPoint(0.5f, 1.0f));
                    DrawLabels(c, ViewPortHandler.ContentBottom + yoffset, new SKPoint(0.5f, 0.0f));
                    break;
            }
        }

        public void RenderAxisLine(SKCanvas c)
        {
            if (!XAxis.IsDrawAxisLineEnabled || !XAxis.IsEnabled)
                return;

            AxisLinePaint.Color = XAxis.AxisLineColor;
            AxisLinePaint.StrokeWidth = XAxis.AxisLineWidth;
            AxisLinePaint.PathEffect = XAxis.AxisLineDashedLine;

            switch (XAxis.Position)
            {
                case XAxis.XAxisPosition.Top:
                case XAxis.XAxisPosition.TopInside:
                    c.DrawLine(ViewPortHandler.ContentLeft, ViewPortHandler.ContentTop, ViewPortHandler.ContentRight, ViewPortHandler.ContentTop, AxisLinePaint);
                    break;
                case XAxis.XAxisPosition.Bottom:
                case XAxis.XAxisPosition.BottomInside:
                    c.DrawLine(ViewPortHandler.ContentLeft, ViewPortHandler.ContentBottom, ViewPortHandler.ContentRight, ViewPortHandler.ContentBottom, AxisLinePaint);
                    break;
                case XAxis.XAxisPosition.BothSided:
                    c.DrawLine(ViewPortHandler.ContentLeft, ViewPortHandler.ContentTop, ViewPortHandler.ContentRight, ViewPortHandler.ContentTop, AxisLinePaint);
                    c.DrawLine(ViewPortHandler.ContentLeft, ViewPortHandler.ContentBottom, ViewPortHandler.ContentRight, ViewPortHandler.ContentBottom, AxisLinePaint);
                    break;
            }
        }

        /// <summary>
        /// draws the x-labels on the specified y-position
        /// </summary>
        protected void DrawLabels(SKCanvas c, float pos, SKPoint anchor)
        {

            float labelRotationAngleDegrees = XAxis.LabelRotationAngle;
            var centeringEnabled = XAxis.IsCenterAxisLabelsEnabled;

            SKPoint[] positions = new SKPoint[XAxis.entryCount];

            for (int i = 0; i < positions.Length; i++)
            {
                // only fill x values
                if (centeringEnabled)
                {
                    positions[i].X = XAxis.centeredEntries[i / 2];
                }
                else
                {
                    positions[i].X = XAxis.entries[i / 2];
                }
            }

            positions = Trasformer.PointValuesToPixel(positions);

            for (int i = 0; i < positions.Length; i += 2)
            {

                float x = positions[i].X;

                if (ViewPortHandler.IsInBoundsX(x))
                {

                    var label = XAxis.ValueFormatter.GetFormattedValue(XAxis.entries[i / 2], XAxis);

                    if (XAxis.AvoidFirstLastClipping)
                    {

                        // avoid clipping of the last
                        if (i / 2 == XAxis.entryCount - 1 && XAxis.entryCount > 1)
                        {
                            float width = AxisLabelPaint.MeasureWidth(label);

                            if (width > ViewPortHandler.OffsetRight * 2
                                    && x + width > ViewPortHandler.ChartWidth)
                                x -= width / 2;

                            // avoid clipping of the first
                        }
                        else if (i == 0)
                        {

                            float width = AxisLabelPaint.MeasureWidth(label);
                            x += width / 2;
                        }
                    }

                    DrawLabel(c, label, x, pos, anchor, labelRotationAngleDegrees);
                }
            }
        }

        protected void DrawLabel(SKCanvas c, string formattedLabel, float x, float y, SKPoint anchor, float angleDegrees)
        {
            DrawXAxisValue(c, formattedLabel, x, y, AxisLabelPaint, anchor, angleDegrees);
        }

        public static void DrawXAxisValue(SKCanvas c, string text, float x, float y, SKPaint paint, SKPoint anchor, float angleDegrees)
        {
            float drawOffsetX = 0.0f;
            float drawOffsetY = 0.0f;

            SKRect bounds = SKRect.Empty;
            float lineHeight = paint.GetFontMetrics(out SKFontMetrics fontMatrics);
            paint.MeasureText(text, ref bounds);

            // Android sometimes has pre-padding
            drawOffsetX -= bounds.Left;

            // Android does not snap the bounds to line boundaries,
            //  and draws from bottom to top.
            // And we want to normalize it.
            drawOffsetY += -fontMatrics.Ascent;

            // To have a consistent point of reference, we always draw left-aligned
            var originalTextAlign = paint.TextAlign;
            paint.TextAlign = SKTextAlign.Left;

            if (angleDegrees != 0.0f)
            {

                // Move the text drawing rect in a way that it always rotates around its center
                drawOffsetX -= bounds.Width * 0.5f;
                drawOffsetY -= lineHeight * 0.5f;

                float translateX = x;
                float translateY = y;

                // Move the "outer" rect relative to the anchor, assuming its centered
                if (anchor.X != 0.5f || anchor.Y != 0.5f)
                {
                    var rotatedSize = ChartUtil.GetSizeOfRotatedRectangleByDegrees(
                            bounds.Width,
                            lineHeight,
                            angleDegrees);

                    translateX -= rotatedSize.Width * (anchor.X - 0.5f);
                    translateY -= rotatedSize.Height * (anchor.Y - 0.5f);
                }

                c.Save();
                c.Translate(translateX, translateY);
                c.RotateDegrees(angleDegrees);

                c.DrawText(text, drawOffsetX, drawOffsetY, paint);

                c.Restore();
            }
            else
            {
                if (anchor.X != 0.0f || anchor.Y != 0.0f)
                {

                    drawOffsetX -= bounds.Width * anchor.X;
                    drawOffsetY -= lineHeight * anchor.Y;
                }

                drawOffsetX += x;
                drawOffsetY += y;

                c.DrawText(text, drawOffsetX, drawOffsetY, paint);
            }

            paint.TextAlign = originalTextAlign;
        }

        protected SKPath mRenderGridLinesPath = new SKPath();
        protected SKPoint[] mRenderGridLinesBuffer = new SKPoint[1];

        public void RenderGridLines(SKCanvas c)
        {
            if (!this.XAxis.IsDrawGridLinesEnabled || !this.XAxis.IsEnabled)
                return;

            int clipRestoreCount = c.Save();
            c.ClipRect(GetGridClippingRect());

            if (mRenderGridLinesBuffer.Length != Axis.entryCount)
            {
                mRenderGridLinesBuffer = new SKPoint[XAxis.entryCount];
            }
            var positions = mRenderGridLinesBuffer;

            for (int i = 0; i < Axis.entries.Count; i++)
            {
                float entry = (float)Axis.entries[i];
                positions[i] = new SKPoint(entry, entry);
            }

            positions = Trasformer.PointValuesToPixel(positions);

            SetupGridPaint();

            var gridLinePath = mRenderGridLinesPath;
            gridLinePath.Reset();

            foreach (SKPoint pos in positions)
            {
                DrawGridLine(c, pos, gridLinePath);
            }

            c.RestoreToCount(clipRestoreCount);
        }


        public SKRect GetGridClippingRect()
        {
            var rect = ViewPortHandler.ContentRect;
            rect.Offset(-Axis.GridLineWidth, 0.0f);
            return rect;
        }

        /// <summary>
        /// Draws the grid line at the specified position using the provided path.
        /// </summary>
        protected void DrawGridLine(SKCanvas c, SKPoint pos, SKPath gridLinePath)
        {
            gridLinePath.MoveTo(pos.X, ViewPortHandler.ContentBottom);
            gridLinePath.LineTo(pos.X, ViewPortHandler.ContentTop);

            // draw a path because lines don't support dashing on lower android versions
            c.DrawPath(gridLinePath, GridPaint);

            gridLinePath.Reset();
        }

        /// <summary>
        ///  Draws the LimitLines associated with this axis to the screen.
        /// </summary>
        /// <param name="c"></param>
        public void RenderLimitLines(SKCanvas c)
        {
            var limitLines = XAxis.LimitLines;

            if (limitLines == null || limitLines.Count <= 0)
                return;

            for (int i = 0; i < limitLines.Count; i++)
            {

                LimitLine l = limitLines[i];

                if (!l.IsEnabled)
                    continue;

                int clipRestoreCount = c.Save();
                var rect = ViewPortHandler.ContentRect;
                rect.Offset(-l.LineWidth, 0.0f);
                c.ClipRect(rect);

                var position = Trasformer.PointValueToPixel(l.Limit, 0.0f);

                RenderLimitLineLine(c, l, position);
                RenderLimitLineLabel(c, l, position, 2.0f + l.YOffset);

                c.RestoreToCount(clipRestoreCount);
            }
        }

        private readonly SKPoint[] LimitLineSegmentsBuffer = new SKPoint[2];
        private readonly SKPath LimitLinePath = new SKPath();

        public void RenderLimitLineLine(SKCanvas c, LimitLine limitLine, SKPoint position)
        {
            LimitLineSegmentsBuffer[0] = new SKPoint(position.X, ViewPortHandler.ContentTop);
            LimitLineSegmentsBuffer[1] = new SKPoint(position.Y, ViewPortHandler.ContentBottom);

            LimitLinePath.Reset();
            LimitLinePath.MoveTo(LimitLineSegmentsBuffer[0]);
            LimitLinePath.LineTo(LimitLineSegmentsBuffer[1]);

            LimitLinePaint.Style = SKPaintStyle.Stroke;
            LimitLinePaint.Color = limitLine.LineColor;
            LimitLinePaint.StrokeWidth = limitLine.LineWidth;
            LimitLinePaint.PathEffect = limitLine.DashPathEffect;

            c.DrawPath(LimitLinePath, LimitLinePaint);
        }

        public void RenderLimitLineLabel(SKCanvas c, LimitLine limitLine, SKPoint position, float yOffset)
        {
            string label = limitLine.Label;

            // if drawing the limit-value label is enabled
            if (string.IsNullOrEmpty(label) == false)
            {

                LimitLinePaint.Style = limitLine.TextStyle;
                LimitLinePaint.PathEffect = null;
                LimitLinePaint.Color = limitLine.TextColor;
                LimitLinePaint.StrokeWidth = 0.5f;
                LimitLinePaint.TextSize = limitLine.TextSize;

                float xOffset = limitLine.LineWidth + limitLine.XOffset;

                LimitLine.LimitLabelPosition labelPosition = limitLine.LabelPosition;

                if (labelPosition == LimitLine.LimitLabelPosition.RightTop)
                {
                    float labelLineHeight = LimitLinePaint.MeasureHeight(label);
                    LimitLinePaint.TextAlign = SKTextAlign.Left;
                    c.DrawText(label, position.X + xOffset, ViewPortHandler.ContentTop + yOffset + labelLineHeight,
                            LimitLinePaint);
                }
                else if (labelPosition == LimitLine.LimitLabelPosition.RightBottom)
                {
                    LimitLinePaint.TextAlign = SKTextAlign.Left;
                    c.DrawText(label, position.X + xOffset, ViewPortHandler.ContentBottom - yOffset, LimitLinePaint);
                }
                else if (labelPosition == LimitLine.LimitLabelPosition.LeftTop)
                {

                    LimitLinePaint.TextAlign = SKTextAlign.Right;
                    float labelLineHeight = LimitLinePaint.MeasureHeight(label);
                    c.DrawText(label, position.X - xOffset, ViewPortHandler.ContentTop + yOffset + labelLineHeight,
                            LimitLinePaint);
                }
                else
                {
                    LimitLinePaint.TextAlign = SKTextAlign.Right;
                    c.DrawText(label, position.X - xOffset, ViewPortHandler.ContentBottom - yOffset, LimitLinePaint);
                }
            }
        }
    }
}