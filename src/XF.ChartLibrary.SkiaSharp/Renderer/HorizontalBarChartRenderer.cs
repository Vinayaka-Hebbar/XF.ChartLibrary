using SkiaSharp;
using System;
using XF.ChartLibrary.Data;
using XF.ChartLibrary.Interfaces.DataProvider;
using XF.ChartLibrary.Interfaces.DataSets;
using XF.ChartLibrary.Utils;

namespace XF.ChartLibrary.Renderer
{
    public partial class HorizontalBarChartRenderer
    {
        protected override void Initialize()
        {
            base.Initialize();
            ValuePaint.TextAlign = SKTextAlign.Left;
        }

        void PrepareBuffer(IBarDataSet dataSet, int index)
        {
            if (Chart is IBarDataProvider dataProvider && (Chart.BarData is BarData barData))
            {
                var barWidthHalf = barData.BarWidth / 2f;
                var bufferIndex = 0;
                var containsStacks = dataSet.IsStacked;
                var isInverted = dataProvider.IsInverted(axis: dataSet.AxisDependency);
                var phaseY = Animator.PhaseY;
                var size = (float)Math.Ceiling(dataSet.EntryCount * Animator.PhaseX);
                IDataSet<BarEntry> data = dataSet;
                for (int i = 0; i < size; i++)
                {
                    BarEntry e = data[i];
                    if (e == null)
                        continue;

                    var x = e.X;

                    var y = e.Y;
                    var vals = e.YVals;
                    float left, right;
                    var bottom = x - barWidthHalf;
                    var top = x + barWidthHalf;
                    if (containsStacks && vals != null)
                    {
                        var posY = 0.0f;
                        var negY = -e.NegativeSum;

                        // fill the stack
                        foreach (var value in vals)
                        {
                            float yStart;
                            if (value == 0.0 && (posY == 0.0 || negY == 0.0))
                            {
                                // Take care of the situation of a 0.0 value, which overlaps a non-zero bar
                                y = value;
                                yStart = y;
                            }
                            else if (value >= 0.0f)
                            {
                                y = posY;
                                yStart = posY + value;
                                posY = yStart;
                            }
                            else
                            {
                                y = negY;
                                yStart = negY + Math.Abs(value);
                                negY += Math.Abs(value);
                            }

                            right = isInverted
                                ? (y <= yStart ? y : yStart)
                                : (y >= yStart ? y : yStart);
                            left = isInverted
                                ? (y >= yStart ? y : yStart)
                                : (y <= yStart ? y : yStart);

                            // multiply the height of the rect with the phase
                            right *= phaseY;
                            left *= phaseY;

                            BarBuffer[index][bufferIndex++] = new SKPoint(left, top);
                            BarBuffer[index][bufferIndex++] = new SKPoint(right, bottom);
                        }
                    }
                    else
                    {
                        if (isInverted)
                        {
                            left = y >= 0 ? y : 0;
                            right = y <= 0 ? y : 0;
                        }
                        else
                        {
                            right = y >= 0 ? y : 0;
                            left = y <= 0 ? y : 0;
                        }

                        // multiply the height of the rect with the phase
                        if (right > 0)
                            right *= phaseY;
                        else
                            left *= phaseY;
                        BarBuffer[index][bufferIndex++] = new SKPoint(left, top);
                        BarBuffer[index][bufferIndex++] = new SKPoint(right, bottom);
                    }
                }
            }
        }

        protected override SKRect PrepareBarHighlight(SKRect rect, Transformer trans)
        {
            return trans.RectToPixelPhaseHorizontal(rect, Animator.PhaseY);
        }

        protected override void DrawDataSet(SKCanvas c, IBarDataSet barDataSet, int index)
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
                    BarShadowRectBuffer.Top = x - barWidthHalf;
                    BarShadowRectBuffer.Bottom = x + barWidthHalf;

                    BarShadowRectBuffer = trans.RectValueToPixel(BarShadowRectBuffer);

                    if (!ViewPortHandler.IsInBoundsLeft(BarShadowRectBuffer.Bottom))
                        continue;

                    if (!ViewPortHandler.IsInBoundsRight(BarShadowRectBuffer.Top))
                        break;

                    BarShadowRectBuffer.Left = ViewPortHandler.ContentLeft;
                    BarShadowRectBuffer.Left = ViewPortHandler.ContentRight;

                    c.DrawRect(BarShadowRectBuffer, ShadowPaint);
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

                if (!ViewPortHandler.IsInBoundsLeft(pts[j + 1].Y))
                    continue;

                if (!ViewPortHandler.IsInBoundsRight(pts[j].Y))
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
                                    isInverted ? FillDirection.Left : FillDirection.Right);
                }
                else
                {
                    DrawRect(c, pts[j].X, pts[j].Y, pts[j + 1].X,
                            pts[j + 1].Y, RenderPaint);
                }

                if (drawBorder)
                {
                    DrawRect(c, pts[j].X, pts[j].Y, pts[j + 1].X,
                            pts[j + 1].Y, BarBorderPaint);
                }
            }
        }

        public override void DrawValues(SKCanvas c)
        {
            // if values are drawn
            if (IsDrawingValuesAllowed(Chart))
            {
                BarData data = Chart.BarData;
                var dataSets = data.DataSets;

                float valueOffsetPlus = 5f.DpToPixel();
                var drawValueAboveBar = Chart.IsDrawValueAboveBar;

                for (int i = 0; i < data.DataSetCount; i++)
                {

                    IBarDataSet barDataSet = dataSets[i];

                    if (!ShouldDrawValues(barDataSet))
                        continue;

                    // apply the text-styling defined by the DataSet
                    ApplyValueTextStyle(barDataSet);

                    IDataSet<BarEntry> dataSet = barDataSet;

                    var isInverted = Chart.IsInverted(barDataSet.AxisDependency);

                    // calculate the correct offset depending on the draw position of
                    // the value
                    float halfTextHeight = ValuePaint.MeasureHeight("10") / 2f;
                    var formatter = dataSet.ValueFormatter;

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

                            float y = (buffer[j].Y + buffer[j + 1].Y) / 2f;

                            if (!ViewPortHandler.IsInBoundsTop(buffer[j].Y))
                                break;

                            if (!ViewPortHandler.IsInBoundsX(buffer[j].X)
                                    || !ViewPortHandler.IsInBoundsBottom(buffer[j].Y))
                                continue;

                            BarEntry entry = dataSet[j / 2];
                            var val = entry.Y;
                            string formattedValue = formatter.GetFormattedValue(entry.Y, entry, i, ViewPortHandler);
                            float valueTextWidth = ValuePaint.MeasureWidth(formattedValue);
                            float posOffset = drawValueAboveBar ? valueOffsetPlus : -(valueTextWidth + valueOffsetPlus);
                            float negOffset = (drawValueAboveBar ? -(valueTextWidth + valueOffsetPlus) : valueOffsetPlus) - (buffer[j + 1].X - buffer[j].X);
                            if (isInverted)
                            {
                                posOffset = -posOffset - valueTextWidth;
                                negOffset = -negOffset - valueTextWidth;
                            }
                            if (barDataSet.IsDrawValuesEnabled)
                            {
                                DrawValue(c, formattedValue, buffer[j + 1].X + val > 0 ? posOffset : negOffset,
                                       y + halfTextHeight,
                                        barDataSet.ValueTextColorAt(j / 2));
                            }

                            if (entry.Icon != null && barDataSet.IsDrawIconsEnabled)
                            {
                                float px = buffer[j + 1].X + val > 0 ? posOffset : negOffset;
                                float py = y;

                                px += iconsOffset.X;
                                py += iconsOffset.Y;

                                entry.Icon.Draw(
                                    c,
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

                            var color = barDataSet.ValueTextColorAt(index);

                            var vals = entry.YVals;
                            // we still draw stacked bars, but there is one
                            // non-stacked
                            // in between
                            if (vals == null)
                            {

                                if (!ViewPortHandler.IsInBoundsTop(buffer[bufferIndex].Y))
                                    break;

                                if (!ViewPortHandler.IsInBoundsX(buffer[bufferIndex].X)
                                        || !ViewPortHandler.IsInBoundsBottom(buffer[bufferIndex].Y))
                                    continue;

                                var val = entry.Y;
                                var formattedValue = formatter.GetFormattedValue(val, entry, i, ViewPortHandler);
                                var valueTextWidth = ValuePaint.MeasureWidth(formattedValue);
                                float posOffset = drawValueAboveBar ? valueOffsetPlus : -(valueTextWidth + valueOffsetPlus);
                                float negOffset = drawValueAboveBar ? -(valueTextWidth + valueOffsetPlus) : valueOffsetPlus;

                                if (isInverted)
                                {
                                    posOffset = -posOffset - valueTextWidth;
                                    negOffset = -negOffset - valueTextWidth;
                                }

                                if (barDataSet.IsDrawValuesEnabled)
                                {
                                    DrawValue(c, formattedValue, buffer[bufferIndex + 1].X + entry.Y >= 0 ? posOffset : negOffset, buffer[bufferIndex].Y + halfTextHeight,
                                            color);
                                }

                                if (entry.Icon != null && barDataSet.IsDrawIconsEnabled)
                                {
                                    float px = buffer[bufferIndex + 1].X + entry.Y >= 0 ? posOffset : negOffset;
                                    float py = buffer[bufferIndex].Y;

                                    px += iconsOffset.X;
                                    py += iconsOffset.Y;

                                    entry.Icon.Draw(
                                     c,
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

                                    transformed[idx].X = y * phaseY;
                                }

                                transformed = trans.PointValuesToPixel(transformed);

                                for (int k = 0; k < transformed.Length; k++)
                                {
                                    float val = vals[k / 2];
                                    var formattedValue = formatter.GetFormattedValue(val, entry, i, ViewPortHandler);
                                    var valueTextWidth = ValuePaint.MeasureWidth(formattedValue);
                                    float posOffset = drawValueAboveBar ? valueOffsetPlus : -(valueTextWidth + valueOffsetPlus);
                                    float negOffset = drawValueAboveBar ? -(valueTextWidth + valueOffsetPlus) : valueOffsetPlus;

                                    if (isInverted)
                                    {
                                        posOffset = -posOffset - valueTextWidth;
                                        negOffset = -negOffset - valueTextWidth;
                                    }
                                    var drawBelow =
                                            (val == 0.0f && negY == 0.0f && posY > 0.0f) ||
                                                    val < 0.0f;
                                    float x = transformed[k].X
                                            + (drawBelow ? negOffset : posOffset);
                                    var y = buffer[bufferIndex].Y + buffer[bufferIndex + 1].Y;

                                    if (!ViewPortHandler.IsInBoundsTop(y))
                                        break;

                                    if (!ViewPortHandler.IsInBoundsX(x)
                                            || !ViewPortHandler.IsInBoundsBottom(y))
                                        continue;

                                    if (barDataSet.IsDrawValuesEnabled)
                                    {
                                        DrawValue(c, formattedValue, x, y + halfTextHeight,
                                                color);
                                    }

                                    if (entry.Icon != null && barDataSet.IsDrawIconsEnabled)
                                    {
                                        entry.Icon.Draw(c,
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

        protected void DrawValue(SKCanvas c, string value, float x, float y, SKColor color)
        {
            ValuePaint.Color = color;
            c.DrawText(value, x, y, ValuePaint);
        }

        protected override void SetHighlightDrawPos(Highlight.Highlight high, SKRect rect)
        {
            high.SetDraw(rect.MidY, rect.Top);
        }

        public override bool IsDrawingValuesAllowed(IChartDataProvider dataProvider)
        {
            return Chart.BarData.EntryCount < Chart.MaxVisibleCount
                * ViewPortHandler.ScaleY;
        }
    }
}
