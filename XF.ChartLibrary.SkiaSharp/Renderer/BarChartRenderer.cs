using SkiaSharp;
using System;
using System.Collections.Generic;
using XF.ChartLibrary.Data;
using XF.ChartLibrary.Interfaces.DataSets;
using XF.ChartLibrary.Utils;

namespace XF.ChartLibrary.Renderer
{
    partial class BarChartRenderer
    {
        protected SKPaint ShadowPaint;
        protected SKPaint BarBorderPaint;

        public override void DrawData(SKCanvas c)
        {
            var barData = Chart.BarData;
            for (int i = 0; i < barData.DataSetCount; i++)
            {

                IBarDataSet set = barData[i];

                if (set.IsVisible)
                {
                    DrawDataSet(c, set, i);
                }
            }
        }

        private SKRect barShadowRectBuffer = SKRect.Empty;

        protected void DrawDataSet(SKCanvas c, IBarDataSet barDataSet, int index)
        {
            Transformer trans = Chart.GetTransformer(barDataSet.AxisDependency);

            BarBorderPaint.Color = barDataSet.BarBorderColor;
            BarBorderPaint.StrokeWidth = barDataSet.BarBorderWidth.DpToPixel();

            var drawBorder = barDataSet.BarBorderWidth > 0.0f;

            float phaseX = Animator.PhaseX;

            // draw the bar shadow before the values
            if (Chart.IsDrawBarShadow)
            {
                ShadowPaint.Color = barDataSet.BarShadowColor;
                IDataSet<BarEntry> dataSet = barDataSet;
                BarData barData = Chart.BarData;

                float barWidth = barData.BarWidth;
                float barWidthHalf = barWidth / 2.0f;
                float x;

                for (int i = 0, count = Math.Min((int)Math.Ceiling(barDataSet.EntryCount * phaseX), barDataSet.EntryCount);
                    i < count;
                    i++)
                {

                    BarEntry e = dataSet[i];

                    x = e.X;
                    barShadowRectBuffer.Left = x - barWidthHalf;
                    barShadowRectBuffer.Right = x + barWidthHalf;

                    trans.RectValueToPixel(barShadowRectBuffer);

                    if (!ViewPortHandler.IsInBoundsLeft(barShadowRectBuffer.Right))
                        continue;

                    if (!ViewPortHandler.IsInBoundsRight(barShadowRectBuffer.Left))
                        break;

                    barShadowRectBuffer.Top = ViewPortHandler.ContentTop;
                    barShadowRectBuffer.Bottom = ViewPortHandler.ContentBottom;

                    c.DrawRect(barShadowRectBuffer, ShadowPaint);
                }
            }

            PrepareBuffer(dataSet: barDataSet, index: index);
            SKPoint[] pts;
            BarBuffer[index] = pts = trans.PointValuesToPixel(BarBuffer[index]);
            // initialize the buffer
            bool isCustomFill = barDataSet.Fills != null && barDataSet.Fills.Count != 0;
            bool isSingleColor = barDataSet.Colors.Count == 1;
            bool isInverted = Chart.IsInverted(barDataSet.AxisDependency);

            if (isSingleColor)
            {
                RenderPaint.Color = barDataSet.Color;
            }

            for (int j = 0, pos = 0; j < pts.Length; j += 2, pos++)
            {

                if (!ViewPortHandler.IsInBoundsLeft(pts[j + 1].X))
                    continue;

                if (!ViewPortHandler.IsInBoundsRight(pts[j].X))
                    break;

                if (!isSingleColor)
                {
                    // Set the color for the currently drawn value. If the index
                    // is out of bounds, reuse colors.
                    RenderPaint.Color = barDataSet.ColorAt(pos);
                }

                if (isCustomFill)
                {
                    barDataSet.GetFill(pos)
                            .Draw(
                                    c, RenderPaint,
                                    pts[j].X,
                                    pts[j].Y,
                                    pts[j + 1].X,
                                    pts[j + 1].Y,
                                    isInverted ? FillDirection.Down : FillDirection.Up);
                }
                else
                {
                    c.DrawRect(pts[j].X, pts[j].Y, pts[j + 1].X,
                            pts[j + 1].Y, RenderPaint);
                }

                if (drawBorder)
                {
                    c.DrawRect(pts[j].X, pts[j].Y, pts[j + 1].X,
                            pts[j + 1].Y, BarBorderPaint);
                }
            }
        }

        protected override void Initialize()
        {
            base.Initialize();
            HighlightPaint.Style = SKPaintStyle.Fill;
            // set alpha after color
            HighlightPaint.Color = new SKColor(0, 0, 0).WithAlpha(120);

            ShadowPaint = new SKPaint()
            {
                Style = SKPaintStyle.Fill,
                IsAntialias = true
            };

            BarBorderPaint = new SKPaint()
            {
                Style = SKPaintStyle.Stroke,
                IsAntialias = true
            };
        }

        public override void DrawValues(SKCanvas c)
        {
            // if values are drawn
            if (IsDrawingValuesAllowed(Chart))
            {
                BarData data = Chart.BarData;
                var dataSets = data.DataSets;

                float valueOffsetPlus = 4.5f.DpToPixel();
                var drawValueAboveBar = Chart.IsDrawValueAboveBar;

                for (int i = 0; i < data.DataSetCount; i++)
                {

                    IBarDataSet barDataSet = dataSets[i];

                    if (!ShouldDrawValues(barDataSet))
                        continue;

                    IDataSet<BarEntry> dataSet = barDataSet;
                    // apply the text-styling defined by the DataSet
                    ApplyValueTextStyle(barDataSet);

                    var isInverted = Chart.IsInverted(barDataSet.AxisDependency);

                    // calculate the correct offset depending on the draw position of
                    // the value
                    float valueTextHeight = ValuePaint.MeasureHeight("8");
                    float posOffset = (drawValueAboveBar ? -valueOffsetPlus : valueTextHeight + valueOffsetPlus);
                    float negOffset = (drawValueAboveBar ? valueTextHeight + valueOffsetPlus : -valueOffsetPlus);

                    if (isInverted)
                    {
                        posOffset = -posOffset - valueTextHeight;
                        negOffset = -negOffset - valueTextHeight;
                    }

                    // get the buffer
                    var buffer = BarBuffer[i];

                    float phaseY = Animator.PhaseY;

                    var iconsOffset = barDataSet.IconsOffset;
                    iconsOffset.X = iconsOffset.X.DpToPixel();
                    iconsOffset.Y = iconsOffset.Y.DpToPixel();

                    // if only single values are drawn (sum)
                    if (!barDataSet.IsStacked)
                    {
                        for (int j = 0; j < buffer.Length * Animator.PhaseX; j += 2)
                        {

                            float x = (buffer[j].X + buffer[j + 1].X) / 2f;

                            if (!ViewPortHandler.IsInBoundsRight(x))
                                break;

                            if (!ViewPortHandler.IsInBoundsY(buffer[j].Y)
                                    || !ViewPortHandler.IsInBoundsLeft(x))
                                continue;

                            BarEntry entry = dataSet[j / 2];
                            float val = entry.Y;

                            if (barDataSet.IsDrawValuesEnabled)
                            {
                                DrawValue(c, barDataSet.ValueFormatter, val, entry, i, x,
                                        val >= 0 ?
                                                (buffer[j].Y + posOffset) :
                                                (buffer[j + 1].Y + negOffset),
                                        barDataSet.ValueTextColorAt(j / 2));
                            }

                            if (entry.Icon != null && barDataSet.IsDrawIconsEnabled)
                            {
                                float px = x;
                                float py = val >= 0 ?
                                        (buffer[j].Y + posOffset) :
                                        (buffer[j + 1].Y + negOffset);

                                px += iconsOffset.X;
                                py += iconsOffset.Y;

                                c.DrawImage(
                                    entry.Icon,
                                    (int)px,
                                    (int)py);
                            }
                        }

                        // if we have stacks
                    }
                    else
                    {

                        Transformer trans = Chart.GetTransformer(barDataSet.AxisDependency);

                        int bufferIndex = 0;
                        int index = 0;
                        while (index < barDataSet.EntryCount * Animator.PhaseX)
                        {

                            BarEntry entry = dataSet[index];

                            var vals = entry.YVals;
                            float x = (buffer[bufferIndex].X + buffer[bufferIndex + 1].X) / 2f;

                            var color = barDataSet.ValueTextColorAt(index);

                            // we still draw stacked bars, but there is one
                            // non-stacked
                            // in between
                            if (vals == null)
                            {

                                if (!ViewPortHandler.IsInBoundsRight(x))
                                    break;

                                if (!ViewPortHandler.IsInBoundsY(buffer[bufferIndex].Y)
                                        || !ViewPortHandler.IsInBoundsLeft(x))
                                    continue;

                                if (barDataSet.IsDrawValuesEnabled)
                                {
                                    DrawValue(c, barDataSet.ValueFormatter, entry.Y, entry, i, x,
                                            buffer[bufferIndex].Y +
                                                    (entry.Y >= 0 ? posOffset : negOffset),
                                            color);
                                }

                                if (entry.Icon != null && barDataSet.IsDrawIconsEnabled)
                                {
                                    float px = x;
                                    float py = buffer[bufferIndex].Y +
                                            (entry.Y >= 0 ? posOffset : negOffset);

                                    px += iconsOffset.X;
                                    py += iconsOffset.Y;

                                    c.DrawImage(
                                     entry.Icon,
                                     (int)px,
                                     (int)py);
                                }

                                // draw stack values
                            }
                            else
                            {

                                SKPoint[] transformed = new SKPoint[vals.Count];

                                float posY = 0f;
                                float negY = -entry.NegativeSum;

                                for (int idx = 0; idx < transformed.Length; idx++)
                                {
                                    float value = vals[idx];
                                    float y;

                                    if (value == 0.0f && (posY == 0.0f || negY == 0.0f))
                                    {
                                        // Take care of the situation of a 0.0 value, which overlaps a non-zero bar
                                        y = value;
                                    }
                                    else if (value >= 0.0f)
                                    {
                                        posY += value;
                                        y = posY;
                                    }
                                    else
                                    {
                                        y = negY;
                                        negY -= value;
                                    }

                                    transformed[idx].Y = y * phaseY;
                                }

                                trans.PointValuesToPixel(transformed);

                                for (int k = 0; k < transformed.Length; k++)
                                {

                                    float val = vals[k / 2];
                                    var drawBelow =
                                            (val == 0.0f && negY == 0.0f && posY > 0.0f) ||
                                                    val < 0.0f;
                                    float y = transformed[k].Y
                                            + (drawBelow ? negOffset : posOffset);

                                    if (!ViewPortHandler.IsInBoundsRight(x))
                                        break;

                                    if (!ViewPortHandler.IsInBoundsY(y)
                                            || !ViewPortHandler.IsInBoundsLeft(x))
                                        continue;

                                    if (barDataSet.IsDrawValuesEnabled)
                                    {
                                        DrawValue(c,
                                                barDataSet.ValueFormatter,
                                                vals[k / 2],
                                                entry,
                                                i,
                                                x,
                                                y,
                                                color);
                                    }

                                    if (entry.Icon != null && barDataSet.IsDrawIconsEnabled)
                                    {
                                        c.DrawImage(entry.Icon,
                                                (int)(x + iconsOffset.X),
                                                (int)(y + iconsOffset.Y));
                                    }
                                }
                            }

                            bufferIndex = vals == null ? bufferIndex + 2 : bufferIndex + 2 * vals.Count;
                            index++;
                        }
                    }
                }
            }
        }

        public override void DrawHighlighted(SKCanvas c, IList<Highlight.Highlight> indices)
        {
            var barData = Chart.BarData;

            foreach (var high in indices)
            {

                var set = barData[high.DataSetIndex];

                if (set == null || !set.IsHighlightEnabled)
                    continue;

                var e = ((IDataSet<BarEntry>)set).EntryForXValue(high.X, high.Y);

                if (!IsInBoundsX(e, set))
                    continue;

                var trans = Chart.GetTransformer(set.AxisDependency);

                HighlightPaint.Color = set.HighLightColor.WithAlpha(set.HighLightAlpha);

                var isStack = high.StackIndex >= 0 && e.IsStacked;

                float y1;
                float y2;

                if (isStack)
                {

                    if (Chart.IsHighlightFullBar)
                    {

                        y1 = e.PositiveSum;
                        y2 = -e.NegativeSum;

                    }
                    else
                    {

                        var range = e.Ranges[high.StackIndex];

                        y1 = range.From;
                        y2 = range.To;
                    }

                }
                else
                {
                    y1 = e.Y;
                    y2 = 0.0f;
                }

                float barWidthHalf = barData.BarWidth / 2f;
                var x = e.X;

                SKRect rect = trans.RectToPixelPhase(new SKRect(x - barWidthHalf, y1, x + barWidthHalf, y2), Animator.PhaseY);
                high.SetDraw(rect.MidX, rect.Top);

                c.DrawRect(rect, HighlightPaint);
            }
        }

    }
}
