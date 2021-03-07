using System;
using System.Collections.Generic;
using XF.ChartLibrary.Utils;

#if NETSTANDARD || SKIASHARP
using Color = SkiaSharp.SKColor;
using Alpha = System.Byte;
#elif __IOS__ || __TVOS
    using Color = UIKit.UIColor;
    using Alpha = System.Single;
#elif __ANDROID__
    using Color = Android.Graphics.Color;
    using Alpha = System.Byte;
#endif

namespace XF.ChartLibrary.Data
{
    public class BarDataSet : BarLineScatterCandleBubbleDataSet<BarEntry>, Interfaces.DataSets.IBarDataSet
    {
        public const Alpha DefaultHighlightAlpha =
#if (__IOS__ || __TVOS) && !SKIASHARP
            120/255f;
#else
            120;
#endif

        /// <summary>
        /// the maximum number of bars that are stacked upon each other, this value
        /// is calculated from the Entries that are added to the DataSet
        /// </summary>
        private int stackSize = 1;

        /// <summary>
        /// the color used for drawing the bar shadows
        /// </summary>
        private Color barShadowColor = ColorTemplate.FromRGB(215, 215, 215);

        private float barBorderWidth = 0.0f;

        private Color barBorderColor = ColorTemplate.Black;

        /// <summary>
        /// the alpha value used to draw the highlight indicator bar
        /// </summary>
        private Alpha highLightAlpha = DefaultHighlightAlpha;

        /// <summary>
        /// the overall entry count, including counting each stack-value individually
        /// </summary>
        private int entryCountStacks = 0;

        /// <summary>
        /// array of labels used to describe the different values of the stacked bars
        /// </summary>
        private IList<string> stackLabels;

        public BarDataSet(IList<BarEntry> yVals, string label) : base(yVals, label)
        {
            highLightColor = ColorTemplate.FromRGB(0, 0, 0);

            CalcStackSize(yVals);
            CalcEntryCountIncludingStacks(yVals);
            stackLabels = Array.Empty<string>();
        }

#if __ANDROID__ || SKIASHARP
        protected IList<IRectFill> fills = null;
        public IList<IRectFill> Fills
        {
            get => fills;
            set => fills = value;
        }

        public IRectFill GetFill(int index)
        {
            return fills[index % fills.Count];
        }
#endif

        public int StackSize
        {
            get => stackSize;
        }

        public bool IsStacked
        {
            get => stackSize > 1;
        }

        /// <summary>
        /// returns the overall entry count, including counting each stack-value
        /// individually
        /// </summary>
        public int EntryCountStacks
        {
            get => entryCountStacks;
        }

        /// <summary>
        /// Sets the color used for drawing the bar-shadows. The bar shadows is a
        /// surface behind the bar that indicates the maximum value.
        /// </summary>
        public Color BarShadowColor
        {
            get => barShadowColor;
            set => barShadowColor = value;
        }

        /// <summary>
        /// the width used for drawing borders around the bars.
        ///If borderWidth == 0, no border will be drawn.
        /// </summary>
        public float BarBorderWidth
        {
            get => barBorderWidth;
            set => barBorderWidth = value;
        }

        /// <summary>
        /// the color drawing borders around the bars.
        /// </summary>
        public Color BarBorderColor
        {
            get => barBorderColor;
            set => barBorderColor = value;
        }

        /**
         * Set the alpha value (transparency) that is used for drawing the highlight
         * indicator bar. min = 0 (fully transparent), max = 255 (fully opaque)
         *
         * @param alpha
         */
        public Alpha HighLightAlpha
        {
            get => highLightAlpha;
            set => highLightAlpha = value;
        }

        /// <summary>
        /// labels for different values of bar-stacks, in case there are one.
        /// </summary>
        public IList<string> StackLabels
        {
            get => stackLabels;
            set => stackLabels = value;
        }

        /// <summary>
        /// Calculates the total number of entries this DataSet represents, including
        /// stacks.All values belonging to a stack are calculated separately.
        /// </summary>
        /// <param name="yVals"></param>
        private void CalcEntryCountIncludingStacks(IList<BarEntry> yVals)
        {

            entryCountStacks = 0;

            for (int i = 0; i < yVals.Count; i++)
            {
                var vals = yVals[i].YVals;
                if (vals == null)
                    entryCountStacks++;
                else
                    entryCountStacks += vals.Count;
            }
        }

        /// <summary>
        /// calculates the maximum stacksize that occurs in the Entries array of this
        /// DataSet
        /// </summary>
        /// <param name="yVals"></param>
        private void CalcStackSize(IList<BarEntry> yVals)
        {
            for (int i = 0; i < yVals.Count; i++)
            {
                var vals = yVals[i].YVals;
                if (vals != null && vals.Count > stackSize)
                    stackSize = vals.Count;
            }
        }

        protected override void CalcMinMax(BarEntry e)
        {
            if (e != null && !float.IsNaN(e.Y))
            {
                if (e.YVals == null)
                {
                    if (e.Y < yMin)
                        yMin = e.Y;

                    if (e.Y > yMax)
                        yMax = e.Y;
                }
                else
                {

                    if (-e.NegativeSum < yMin)
                        yMin = -e.NegativeSum;

                    if (e.PositiveSum > yMax)
                        yMax = e.PositiveSum;
                }

                CalcMinMaxX(e);
            }
        }
    }
}
