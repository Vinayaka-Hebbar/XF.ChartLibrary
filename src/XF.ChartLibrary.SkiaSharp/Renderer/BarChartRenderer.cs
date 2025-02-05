﻿using SkiaSharp;
using System;
using System.Collections.Generic;
using XF.ChartLibrary.Data;
using XF.ChartLibrary.Interfaces.DataProvider;
using XF.ChartLibrary.Interfaces.DataSets;
using XF.ChartLibrary.Utils;

namespace XF.ChartLibrary.Renderer
{
    partial class BarChartRenderer
    {
        protected SKPaint ShadowPaint;
        protected SKPaint BarBorderPaint;

        protected SKPoint[][] BarBuffer;

        #region Buffer
        public override void InitBuffers()
        {
            var barData = Chart.BarData;
            BarBuffer = new SKPoint[barData.DataSetCount][];

            for (int i = 0; i < BarBuffer.Length; i++)
            {
                var set = barData[i];
                BarBuffer[i] = new SKPoint[set.EntryCount * 2 * (set.IsStacked ? set.StackSize : 1)];
            }
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
                    var left = x - barWidthHalf;
                    var right = x + barWidthHalf;

                    var y = e.Y;
                    var vals = e.YVals;
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

                            var top = isInverted
                                ? (y <= yStart ? y : yStart)
                                : (y >= yStart ? y : yStart);
                            var bottom = isInverted
                                ? (y >= yStart ? y : yStart)
                                : (y <= yStart ? y : yStart);

                            // multiply the height of the rect with the phase
                            top *= phaseY;
                            bottom *= phaseY;

                            BarBuffer[index][bufferIndex++] = new SKPoint(left, top);
                            BarBuffer[index][bufferIndex++] = new SKPoint(right, bottom);
                        }
                    }
                    else
                    {
                        float bottom, top;

                        if (isInverted)
                        {
                            bottom = y >= 0 ? y : 0;
                            top = y <= 0 ? y : 0;
                        }
                        else
                        {
                            top = y >= 0 ? y : 0;
                            bottom = y <= 0 ? y : 0;
                        }

                        // multiply the height of the rect with the phase
                        if (top > 0)
                            top *= phaseY;
                        else
                            bottom *= phaseY;
                        BarBuffer[index][bufferIndex++] = new SKPoint(left, top);
                        BarBuffer[index][bufferIndex++] = new SKPoint(right, bottom);
                    }
                }
            }
        }
        #endregion

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

        protected SKRect BarShadowRectBuffer = SKRect.Empty;

        protected virtual void DrawDataSet(SKCanvas c, IBarDataSet barDataSet, int index)
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
                    BarShadowRectBuffer.Left = x - barWidthHalf;
                    BarShadowRectBuffer.Right = x + barWidthHalf;

                    BarShadowRectBuffer = trans.RectValueToPixel(BarShadowRectBuffer);

                    if (!ViewPortHandler.IsInBoundsLeft(BarShadowRectBuffer.Right))
                        continue;

                    if (!ViewPortHandler.IsInBoundsRight(BarShadowRectBuffer.Left))
                        break;

                    BarShadowRectBuffer.Top = ViewPortHandler.ContentTop;
                    BarShadowRectBuffer.Bottom = ViewPortHandler.ContentBottom;

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

        protected void DrawRect(SKCanvas c, float left, float top, float right, float bottom, SKPaint paint)
        {
            c.DrawRect(left, top, (right - left),
                                        (bottom - top), paint);
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

                                    transformed[idx].Y = y * phaseY;
                                }

                                transformed = trans.PointValuesToPixel(transformed);

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

                SKRect rect = PrepareBarHighlight(new SKRect(x - barWidthHalf, y1, x + barWidthHalf, y2), trans);
                SetHighlightDrawPos(high, rect);

                c.DrawRect(rect, HighlightPaint);
            }
        }

        protected virtual SKRect PrepareBarHighlight(SKRect rect, Transformer trans)
        {
            return trans.RectToPixelPhase(rect, Animator.PhaseY);
        }

        /// <summary>
        /// Sets the drawing position of the highlight object based on the riven bar-rect.
        /// </summary>
        /// <param name="high"></param>
        /// <param name="rect"></param>
        protected virtual void SetHighlightDrawPos(Highlight.Highlight high, SKRect rect)
        {
            high.SetDraw(rect.MidX, rect.Top);
        }

        public override void DrawExtras(SKCanvas c)
        {
        }
    }
}
