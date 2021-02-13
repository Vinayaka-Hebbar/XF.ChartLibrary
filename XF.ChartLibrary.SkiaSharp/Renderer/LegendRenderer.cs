using SkiaSharp;
using System;
using System.Collections.Generic;
using XF.ChartLibrary.Components;
using XF.ChartLibrary.Interfaces;
using XF.ChartLibrary.Interfaces.DataSets;

namespace XF.ChartLibrary.Renderer
{
    partial class LegendRenderer
    {
        /// <summary>
        /// paint for the legend labels
        /// </summary>
        protected SKPaint legendLabelPaint;

        /// <summary>
        /// paint used for the legend forms
        /// </summary>
        protected SKPaint legendFormPaint;


        protected readonly List<LegendEntry> computedEntries = new List<LegendEntry>(16);

        protected override void Initialize()
        {
            legendLabelPaint = new SKPaint
            {
                IsAntialias = true,
                TextSize = 9f.DpToPixel(),
                TextAlign = SKTextAlign.Left
            };

            legendFormPaint = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Fill
            };
        }

        /// <summary>
        /// Prepares the legend and calculates all needed forms, labels and colors.
        /// </summary>
        /// <param name="data"></param>
        public void ComputeLegend(IChartData data)
        {
            if (!Legend.IsLegendCustom)
            {
                computedEntries.Clear();

                // loop for building up the colors and labels used in the legend
                for (int i = 0; i < data.DataSetCount; i++)
                {

                    IDataSet dataSet = data[i];
                    if (dataSet == null)
                        continue;

                    IList<SKColor> clrs = dataSet.Colors;
                    int entryCount = dataSet.EntryCount;

                    // if we have a barchart with stacked bars
                    if (dataSet is IBarDataSet bds && bds.IsStacked)
                    {
                        IList<string> sLabels = bds.StackLabels;

                        int minEntries = Math.Min(clrs.Count, bds.StackSize);

                        for (int j = 0; j < minEntries; j++)
                        {
                            string label;
                            if (sLabels.Count > 0)
                            {
                                int labelIndex = j % minEntries;
                                label = labelIndex < sLabels.Count ? sLabels[labelIndex] : null;
                            }
                            else
                            {
                                label = null;
                            }

                            computedEntries.Add(new LegendEntry(
                                    label,
                                    dataSet.Form,
                                    dataSet.FormSize,
                                    dataSet.FormLineWidth,
                                    dataSet.FormLineDashEffect,
                                    clrs[j]
                            ));
                        }

                        if (bds.Label != null)
                        {
                            // add the legend description label
                            computedEntries.Add(new LegendEntry(
                                    dataSet.Label,
                                    Form.None,
                                    float.NaN,
                                    float.NaN,
                                    null,
                                    SKColors.Empty
                            ));
                        }

                    }
                    else if (dataSet is IPieDataSet pds)
                    {

                        for (int j = 0; j < clrs.Count && j < entryCount; j++)
                        {

                            computedEntries.Add(new LegendEntry(
                                    pds[j].Label,
                                    dataSet.Form,
                                    dataSet.FormSize,
                                    dataSet.FormLineWidth,
                                    dataSet.FormLineDashEffect,
                                    clrs[j]
                            ));
                        }

                        if (pds.Label != null)
                        {
                            // add the legend description label
                            computedEntries.Add(new LegendEntry(
                                    dataSet.Label,
                                    Form.None,
                                    float.NaN,
                                    float.NaN,
                                    null,
                                    SKColors.Empty
                            ));
                        }

                    }
                    else if (dataSet is ICandleDataSet candle && candle.DecreasingColor !=
                          SKColors.Empty)
                    {

                        SKColor decreasingColor = candle.DecreasingColor;
                        SKColor increasingColor = candle.IncreasingColor;

                        computedEntries.Add(new LegendEntry(
                                null,
                                dataSet.Form,
                                dataSet.FormSize,
                                dataSet.FormLineWidth,
                                dataSet.FormLineDashEffect,
                                decreasingColor
                        ));

                        computedEntries.Add(new LegendEntry(
                                dataSet.Label,
                                dataSet.Form,
                                dataSet.FormSize,
                                dataSet.FormLineWidth,
                                dataSet.FormLineDashEffect,
                                increasingColor
                        ));

                    }
                    else
                    {
                        // all others
                        for (int j = 0; j < clrs.Count && j < entryCount; j++)
                        {
                            string label;

                            // if multiple colors are set for a DataSet, group them
                            if (j < clrs.Count - 1 && j < entryCount - 1)
                            {
                                label = null;
                            }
                            else
                            { // add label to the last entry
                                label = data[i].Label;
                            }

                            computedEntries.Add(new LegendEntry(
                                    label,
                                    dataSet.Form,
                                    dataSet.FormSize,
                                    dataSet.FormLineWidth,
                                    dataSet.FormLineDashEffect,
                                    clrs[j]
                            ));
                        }
                    }
                }

                if (Legend.ExtraEntries != null)
                {
                    computedEntries.AddRange(Legend.ExtraEntries);
                }

                Legend.Entries = computedEntries;
            }

            SKTypeface tf = Legend.Typeface;

            if (tf != null)
                legendLabelPaint.Typeface = tf;

            legendLabelPaint.TextSize = Legend.TextSize;
            legendLabelPaint.Color = Legend.TextColor;

            // calculate all dimensions of the mLegend
            Legend.CalculateDimensions(legendLabelPaint, ViewPortHandler);
        }

        public void RenderLegend(SKCanvas c)
        {
            if (!Legend.IsEnabled)
                return;

            SKTypeface tf = Legend.Typeface;

            if (tf != null)
                legendLabelPaint.Typeface = tf;

            legendLabelPaint.TextSize = Legend.TextSize;
            legendLabelPaint.Color = Legend.TextColor;

            float labelLineHeight = legendLabelPaint.LineHeight();
            float labelLineSpacing = legendLabelPaint.LineSpacing()
                    + Legend.YEntrySpace.DpToPixel();
            float formYOffset = labelLineHeight - legendLabelPaint.MeasureHeight("ABC") / 2.0f;

            IList<LegendEntry> entries = Legend.Entries;

            float formToTextSpace = Legend.FormToTextSpace.DpToPixel();
            float xEntrySpace = Legend.XEntrySpace.DpToPixel();
            Orientation orientation = Legend.Orientation;
            HorizontalAlignment horizontalAlignment = Legend.HorizontalAlignment;
            VerticalAlignment verticalAlignment = Legend.VerticalAlignment;
            Direction direction = Legend.Direction;
            float defaultFormSize = Legend.FormSize.DpToPixel();

            // space between the entries
            float stackSpace = Legend.StackSpace.DpToPixel();

            float yoffset = Legend.YOffset;
            float xoffset = Legend.XOffset;
            float originPosX = 0.0f;

            switch (horizontalAlignment)
            {
                case HorizontalAlignment.Left:

                    if (orientation == Orientation.Vertical)
                        originPosX = xoffset;
                    else
                        originPosX = ViewPortHandler.ContentLeft + xoffset;

                    if (direction == Direction.RightToLeft)
                        originPosX += Legend.NeededWidth;

                    break;

                case HorizontalAlignment.Right:

                    if (orientation == Orientation.Vertical)
                        originPosX = ViewPortHandler.ChartWidth - xoffset;
                    else
                        originPosX = ViewPortHandler.ContentRight - xoffset;

                    if (direction == Direction.LeftToRight)
                        originPosX -= Legend.NeededWidth;

                    break;

                case HorizontalAlignment.Center:

                    if (orientation == Orientation.Vertical)
                        originPosX = ViewPortHandler.ChartWidth / 2.0f;
                    else
                        originPosX = ViewPortHandler.ContentLeft
                                + ViewPortHandler.ContentWidth / 2.0f;

                    originPosX += direction == Direction.LeftToRight
                            ? +xoffset
                            : -xoffset;

                    // Horizontally layed out legends do the center offset on a line basis,
                    // So here we offset the vertical ones only.
                    if (orientation == Orientation.Vertical)
                    {
                        originPosX += (direction == Direction.LeftToRight
                                ? -Legend.NeededWidth / 2.0f + xoffset
                                : Legend.NeededWidth / 2.0f - xoffset);
                    }

                    break;
            }

            switch (orientation)
            {
                case Orientation.Horizontal:
                    {

                        IList<Utils.ChartSize> calculatedLineSizes = Legend.CalculatedLineSizes;
                        IList<Utils.ChartSize> calculatedLabelSizes = Legend.CalculatedLabelSizes;
                        IList<bool> calculatedLabelBreakPoints = Legend.CalculatedLabelBreakPoints;

                        float posX = originPosX;
                        float posY = 0.0f;

                        switch (verticalAlignment)
                        {
                            case VerticalAlignment.Top:
                                posY = yoffset;
                                break;

                            case VerticalAlignment.Bottom:
                                posY = ViewPortHandler.ChartHeight - yoffset - Legend.NeededHeight;
                                break;

                            case VerticalAlignment.Center:
                                posY = (ViewPortHandler.ChartHeight - Legend.NeededHeight) / 2.0f + yoffset;
                                break;
                        }

                        int lineIndex = 0;

                        for (int i = 0, count = entries.Count; i < count; i++)
                        {

                            LegendEntry e = entries[i];
                            bool drawingForm = e.Form != Form.None;
                            float formSize = float.IsNaN(e.FormSize) ? defaultFormSize : e.FormSize.DpToPixel();

                            if (i < calculatedLabelBreakPoints.Count && calculatedLabelBreakPoints[i])
                            {
                                posX = originPosX;
                                posY += labelLineHeight + labelLineSpacing;
                            }

                            if (posX == originPosX &&
                                    horizontalAlignment == HorizontalAlignment.Center &&
                                    lineIndex < calculatedLineSizes.Count)
                            {
                                posX += (direction == Direction.RightToLeft
                                        ? calculatedLineSizes[lineIndex].Width
                                        : -calculatedLineSizes[lineIndex].Width) / 2.0f;
                                lineIndex++;
                            }

                            bool isStacked = e.Label == null; // grouped forms have null labels

                            if (drawingForm)
                            {
                                if (direction == Direction.RightToLeft)
                                    posX -= formSize;

                                DrawForm(c, posX, posY + formYOffset, e, Legend);

                                if (direction == Direction.LeftToRight)
                                    posX += formSize;
                            }

                            if (!isStacked)
                            {
                                if (drawingForm)
                                    posX += direction == Direction.RightToLeft ? -formToTextSpace :
                                            formToTextSpace;

                                if (direction == Direction.RightToLeft)
                                    posX -= calculatedLabelSizes[i].Width;

                                DrawLabel(c, posX, posY + labelLineHeight, e.Label);

                                if (direction == Direction.LeftToRight)
                                    posX += calculatedLabelSizes[i].Width;

                                posX += direction == Direction.RightToLeft ? -xEntrySpace : xEntrySpace;
                            }
                            else
                                posX += direction == Direction.RightToLeft ? -stackSpace : stackSpace;
                        }

                        break;
                    }

                case Orientation.Vertical:
                    {
                        // contains the stacked legend size in pixels
                        float stack = 0f;
                        bool wasStacked = false;
                        float posY = 0.0f;

                        switch (verticalAlignment)
                        {
                            case VerticalAlignment.Top:
                                posY = (horizontalAlignment == HorizontalAlignment.Center
                                        ? 0.0f
                                        : ViewPortHandler.ContentTop);
                                posY += yoffset;
                                break;

                            case VerticalAlignment.Bottom:
                                posY = (horizontalAlignment == HorizontalAlignment.Center
                                        ? ViewPortHandler.ChartHeight
                                        : ViewPortHandler.ContentBottom);
                                posY -= Legend.NeededHeight + yoffset;
                                break;

                            case VerticalAlignment.Center:
                                posY = ViewPortHandler.ChartHeight / 2.0f
                                        - Legend.NeededHeight / 2.0f
                                        + Legend.YOffset;
                                break;
                        }

                        for (int i = 0; i < entries.Count; i++)
                        {

                            LegendEntry e = entries[i];
                            bool drawingForm = e.Form != Form.None;
                            float formSize = float.IsNaN(e.FormSize) ? defaultFormSize : e.FormSize.DpToPixel();

                            float posX = originPosX;

                            if (drawingForm)
                            {
                                if (direction == Direction.LeftToRight)
                                    posX += stack;
                                else
                                    posX -= formSize - stack;

                                DrawForm(c, posX, posY + formYOffset, e, Legend);

                                if (direction == Direction.LeftToRight)
                                    posX += formSize;
                            }

                            if (e.Label != null)
                            {

                                if (drawingForm && !wasStacked)
                                    posX += direction == Direction.LeftToRight ? formToTextSpace
                                            : -formToTextSpace;
                                else if (wasStacked)
                                    posX = originPosX;

                                if (direction == Direction.RightToLeft)
                                    posX -= legendLabelPaint.MeasureWidth(e.Label);

                                if (!wasStacked)
                                {
                                    DrawLabel(c, posX, posY + labelLineHeight, e.Label);
                                }
                                else
                                {
                                    posY += labelLineHeight + labelLineSpacing;
                                    DrawLabel(c, posX, posY + labelLineHeight, e.Label);
                                }

                                // make a step down
                                posY += labelLineHeight + labelLineSpacing;
                                stack = 0f;
                            }
                            else
                            {
                                stack += formSize + stackSpace;
                                wasStacked = true;
                            }
                        }

                        break;

                    }
            }
        }


        private readonly SKPath LineFormPath = new SKPath();

        /// <summary>
        /// Draws the Legend-form at the given position with the color at the given
        /// index.
        /// </summary>
        /// <param name="c">canvas to draw with</param>
        /// <param name="x">position</param>
        /// <param name="y">position</param>
        /// <param name="entry">the entry to render</param>
        /// <param name="legend">the legend context</param>
        protected void DrawForm(
                SKCanvas c,
                float x, float y,
                LegendEntry entry,
                Legend legend)
        {

            if (entry.FormColor == SKColors.Empty)
                return;

            int restoreCount = c.Save();

            Form form = entry.Form;
            if (form == Form.Default)
                form = legend.Form;

            legendFormPaint.Color = entry.FormColor;

            float formSize = (float.IsNaN(entry.FormSize)
                            ? legend.FormSize
                            : entry.FormSize).DpToPixel();
            float half = formSize / 2f;

            switch (form)
            {
                case Form.None:
                case Form.Empty:
                    // Do not draw, but keep space for the form
                    break;

                case Form.Default:
                case Form.Circle:
                    legendFormPaint.Style = SKPaintStyle.Fill;
                    c.DrawCircle(x + half, y, half, legendFormPaint);
                    break;

                case Form.Square:
                    legendFormPaint.Style = SKPaintStyle.Fill;
                    c.DrawRect(x, y - half, x + formSize, y + half, legendFormPaint);
                    break;

                case Form.Line:
                    {
                        float formLineWidth = (float.IsNaN(entry.FormLineWidth)
                                        ? legend.FormLineWidth
                                        : entry.FormLineWidth).DpToPixel();
                        SKPathEffect formLineDashEffect = entry.FormLineDashEffect ?? legend.FormLineDashEffect;
                        legendFormPaint.Style = SKPaintStyle.Stroke;
                        legendFormPaint.StrokeWidth = formLineWidth;
                        legendFormPaint.PathEffect = formLineDashEffect;

                        LineFormPath.Reset();
                        LineFormPath.MoveTo(x, y);
                        LineFormPath.LineTo(x + formSize, y);
                        c.DrawPath(LineFormPath, legendFormPaint);
                    }
                    break;
            }

            c.RestoreToCount(restoreCount);
        }

        /// <summary>
        ///  Draws the provided label at the given position.
        /// </summary>
        /// <param name="c">canvas to draw with</param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="label">the label to draw</param>
        protected void DrawLabel(SKCanvas c, float x, float y, string label)
        {
            c.DrawText(label, x, y, legendLabelPaint);
        }
    }
}
