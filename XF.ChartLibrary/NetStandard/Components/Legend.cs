using System;
using XF.ChartLibrary.Utils;

namespace XF.ChartLibrary.Components
{
    public partial class Legend
    {
        /// <summary>
        /// Calculates the dimensions of the Legend. This includes the maximum width
        /// and height of a single entry, as well as the total width and height of
        /// the Legend.
        public void CalculateDimensions(SkiaSharp.SKPaint paint, ViewPortHandler viewPortHandler)
        {
            var maxEntrySize = GetMaximumEntrySize(paint);
            float defaultFormSize = FormSize;
            float stackSpace = StackSpace;
            float formToTextSpace = FormToTextSpace;
            float xEntrySpace = XEntrySpace;
            float yEntrySpace = YEntrySpace;
            var wordWrapEnabled = WordWrapEnabled;
            var entries = Entries;
            int entryCount = entries.Count;

            TextWidthMax = maxEntrySize.Width;
            TextHeightMax = maxEntrySize.Height;

            switch (Orientation)
            {
                case Orientation.Vertical:
                    {

                        float maxWidth = 0f, maxHeight = 0f, width = 0f;
                        float labelLineHeight = paint.LineHeight();
                        bool wasStacked = false;

                        for (int i = 0; i < entryCount; i++)
                        {

                            LegendEntry e = entries[i];
                            var drawingForm = e.Form != Form.None;
                            float formSize = float.IsNaN(e.FormSize)
                                    ? defaultFormSize
                                    : e.FormSize;
                            var label = e.Label;
                            if (!wasStacked)
                                width = 0.0f;

                            if (drawingForm)
                            {
                                if (wasStacked)
                                    width += stackSpace;
                                width += formSize;
                            }

                            // grouped forms have null labels
                            if (label != null)
                            {
                                var size = paint.MeasureWidth(label);
                                // make a step to the left
                                if (drawingForm && !wasStacked)
                                    width += formToTextSpace;
                                else if (wasStacked)
                                {
                                    maxWidth = Math.Max(maxWidth, width);
                                    maxHeight += labelLineHeight + yEntrySpace;
                                    width = 0.0f;
                                    wasStacked = false;
                                }

                                width += size;

                                maxHeight += labelLineHeight + yEntrySpace;
                            }
                            else
                            {
                                wasStacked = true;
                                width += formSize;
                                if (i < entryCount - 1)
                                    width += stackSpace;
                            }

                            maxWidth = Math.Max(maxWidth, width);
                        }

                        NeededWidth = maxWidth;
                        NeededHeight = maxHeight;

                        break;
                    }
                case Orientation.Horizontal:
                    {

                        float labelLineHeight = paint.LineHeight();
                        float labelLineSpacing = paint.LineSpacing() + yEntrySpace;
                        float contentWidth = viewPortHandler.ContentWidth * MaxSizePercent;

                        // Start calculating layout
                        float maxLineWidth = 0.0f;
                        float currentLineWidth = 0.0f;
                        float requiredWidth = 0.0f;
                        int stackedStartIndex = -1;

                        CalculatedLabelBreakPoints.Clear();
                        CalculatedLabelSizes.Clear();
                        CalculatedLineSizes.Clear();

                        for (int i = 0; i < entryCount; i++)
                        {
                            LegendEntry e = entries[i];
                            var drawingForm = e.Form != Form.None;
                            float formSize = float.IsNaN(e.FormSize)
                                    ? defaultFormSize
                                    : e.FormSize;
                            var label = e.Label;

                            CalculatedLabelBreakPoints.Add(false);

                            if (stackedStartIndex == -1)
                            {
                                // we are not stacking, so required width is for this label
                                // only
                                requiredWidth = 0.0f;
                            }
                            else
                            {
                                // add the spacing appropriate for stacked labels/forms
                                requiredWidth += stackSpace;
                            }

                            // grouped forms have null labels
                            if (label != null)
                            {

                                CalculatedLabelSizes.Add(paint.Measure(label));
                                requiredWidth += drawingForm ? formToTextSpace + formSize : 0.0f;
                                requiredWidth += CalculatedLabelSizes[i].Width;
                            }
                            else
                            {

                                CalculatedLabelSizes.Add(new ChartSize(0,0));
                                requiredWidth += drawingForm ? formSize : 0.0f;

                                if (stackedStartIndex == -1)
                                {
                                    // mark this index as we might want to break here later
                                    stackedStartIndex = i;
                                }
                            }

                            if (label != null || i == entryCount - 1)
                            {

                                float requiredSpacing = currentLineWidth == 0.0f ? 0.0f : xEntrySpace;

                                if (!wordWrapEnabled // No word wrapping, it must fit.
                                                     // The line is empty, it must fit
                                        || currentLineWidth == 0.0f
                                        // It simply fits
                                        || (contentWidth - currentLineWidth >=
                                        requiredSpacing + requiredWidth))
                                {
                                    // Expand current line
                                    currentLineWidth += requiredSpacing + requiredWidth;
                                }
                                else
                                { // It doesn't fit, we need to wrap a line

                                    // Add current line size to array
                                    CalculatedLineSizes.Add(new ChartSize(currentLineWidth, labelLineHeight));
                                    maxLineWidth = Math.Max(maxLineWidth, currentLineWidth);

                                    // Start a new line
                                    CalculatedLabelBreakPoints.Insert(
                                            stackedStartIndex > -1 ? stackedStartIndex
                                                    : i, true);
                                    currentLineWidth = requiredWidth;
                                }

                                if (i == entryCount - 1)
                                {
                                    // Add last line size to array
                                    CalculatedLineSizes.Add(new ChartSize(currentLineWidth, labelLineHeight));
                                    maxLineWidth = Math.Max(maxLineWidth, currentLineWidth);
                                }
                            }

                            stackedStartIndex = label != null ? -1 : stackedStartIndex;
                        }

                        NeededWidth = maxLineWidth;
                        NeededHeight = labelLineHeight
                                * (float)(CalculatedLineSizes.Count)
                                + labelLineSpacing *
                                (float)(CalculatedLineSizes.Count == 0
                                        ? 0
                                        : (CalculatedLineSizes.Count - 1));

                        break;
                    }
            }

            NeededHeight += YOffset;
            NeededWidth += XOffset;
        }
    }
}
