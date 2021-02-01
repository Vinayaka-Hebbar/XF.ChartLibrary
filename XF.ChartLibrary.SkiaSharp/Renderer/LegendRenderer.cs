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
        protected SKPaint mLegendLabelPaint;

        /// <summary>
        /// paint used for the legend forms
        /// </summary>
        protected SKPaint mLegendFormPaint;


        protected readonly List<LegendEntry> computedEntries = new List<LegendEntry>(16);

        protected override void Initialize()
        {
            mLegendLabelPaint = new SKPaint
            {
                IsAntialias = true,
                TextSize = 9f,
                TextAlign = SKTextAlign.Left
            };

            mLegendFormPaint = new SKPaint
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

                    var dataSet = data[i];
                    if (dataSet == null)
                        continue;

                    var clrs = dataSet.Colors;
                    int entryCount = dataSet.EntryCount;

                    // if we have a barchart with stacked bars
                    if (dataSet is IBarDataSet bds && bds.IsStacked)
                    {
                        var sLabels = bds.StackLabels;

                        int minEntries = Math.Min(clrs.Count, bds.StackSize);

                        for (int j = 0; j < minEntries; j++)
                        {
                            String label;
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
                    else if (dataSet is IPieDataSet pds) {

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

                } else if (dataSet is ICandleDataSet candle && candle.DecreasingColor !=
                        SKColors.Empty) {

                    var decreasingColor = candle.DecreasingColor;
                    var increasingColor = candle.IncreasingColor;

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

                } else
                { // all others

                    for (int j = 0; j < clrs.Count && j < entryCount; j++)
                    {

                        String label;

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

            Legend.ExtraEntries = computedEntries;
        }

        var tf = Legend.Typeface;

        if (tf != null)
            mLegendLabelPaint.Typeface = tf;

        mLegendLabelPaint.TextSize = Legend.TextSize;
        mLegendLabelPaint.Color = Legend.TextColor;

        // calculate all dimensions of the mLegend
        Legend.CalculateDimensions(mLegendLabelPaint, ViewPortHandler);
    }


    public void RenderLegend(SKCanvas c)
    {
        if (!Legend.IsEnabled)
            return;

        var tf = Legend.Typeface;

        if (tf != null)
            mLegendLabelPaint.Typeface = tf;

        mLegendLabelPaint.TextSize = Legend.TextSize;
        mLegendLabelPaint.Color = Legend.TextColor;

        float labelLineHeight = mLegendLabelPaint.LineHeight();
        float labelLineSpacing = mLegendLabelPaint.LineSpacing()
                + Legend.YEntrySpace;
        float formYOffset = labelLineHeight - mLegendLabelPaint.MeasureHeight("ABC") / 2.0f;

        var entries = Legend.Entries;

        float formToTextSpace = Legend.FormToTextSpace;
        float xEntrySpace =  Legend.XEntrySpace;
        var orientation = Legend.Orientation;
        var horizontalAlignment = Legend.HorizontalAlignment;
        var verticalAlignment = Legend.VerticalAlignment;
        var direction = Legend.Direction;
        float defaultFormSize = Legend.FormSize;

        // space between the entries
        float stackSpace = (Legend.StackSpace);

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

                    var calculatedLineSizes = Legend.CalculatedLineSizes;
                    var calculatedLabelSizes = Legend.CalculatedLabelSizes;
                    var calculatedLabelBreakPoints = Legend.CalculatedLabelBreakPoints;

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
                        float formSize = float.IsNaN(e.FormSize) ? defaultFormSize : e.FormSize;

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
                        float formSize = float.IsNaN(e.FormSize) ? defaultFormSize : e.FormSize;

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
                                posX -= mLegendLabelPaint.MeasureWidth(e.Label);

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

        var form = entry.Form;
        if (form == Form.Default)
            form = legend.Form;

        mLegendFormPaint.Color = (entry.FormColor);

        float formSize = (
                float.IsNaN(entry.FormSize)
                        ? legend.FormSize
                        : entry.FormSize);
        float half = formSize / 2f;

        switch (form)
        {
            case Form.None:
            case Form.Empty:
                // Do not draw, but keep space for the form
                break;

            case Form.Default:
            case Form.Circle:
                mLegendFormPaint.Style = SKPaintStyle.Fill;
                c.DrawCircle(x + half, y, half, mLegendFormPaint);
                break;

            case Form.Square:
                mLegendFormPaint.Style = SKPaintStyle.Fill;
                c.DrawRect(x, y - half, x + formSize, y + half, mLegendFormPaint);
                break;

            case Form.Line:
                {
                    float formLineWidth =
                            float.IsNaN(entry.FormLineWidth)
                                    ? legend.FormLineWidth
                                    : entry.FormLineWidth;
                    var formLineDashEffect = entry.FormLineDashEffect ?? legend.FormLineDashEffect;
                    mLegendFormPaint.Style = SKPaintStyle.Stroke;
                    mLegendFormPaint.StrokeWidth = formLineWidth;
                    mLegendFormPaint.PathEffect = formLineDashEffect;

                    LineFormPath.Reset();
                    LineFormPath.MoveTo(x, y);
                    LineFormPath.LineTo(x + formSize, y);
                    c.DrawPath(LineFormPath, mLegendFormPaint);
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
    protected void DrawLabel(SKCanvas c, float x, float y, String label)
    {
        c.DrawText(label, x, y, mLegendLabelPaint);
    }
}
}
