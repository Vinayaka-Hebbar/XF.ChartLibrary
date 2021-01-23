using System.Collections.Generic;

namespace XF.ChartLibrary.Data
{
    public abstract class LineScatterCandleRadarDataSet<TEntry> : BarLineScatterCandleBubbleDataSet<TEntry>, ILineScatterCandleRadarDataSet<TEntry> where TEntry : Entry
    {
        private bool drawVerticalHighlightIndicator = true;
        private bool drawHorizontalHighlightIndicator = true;

        /// <summary>
        /// Enables / disables the horizontal highlight-indicator. If disabled, the indicator is not drawn.
        /// </summary>
        public bool DrawVerticalHighlightIndicatorEnabled
        {
            get => drawVerticalHighlightIndicator;
            set => drawVerticalHighlightIndicator = value;
        }

        /// <summary>
        /// Enables / disables the vertical highlight-indicator. If disabled, the indicator is not drawn.
        /// </summary>
        public bool DrawHorizontalHighlightIndicator
        {
            get => drawHorizontalHighlightIndicator;
            set => drawHorizontalHighlightIndicator = value;
        }

        /// <summary>
        /// Enables / disables both vertical and horizontal highlight-indicators
        /// </summary>
        public bool DrawHighlightIndicators
        {
            set
            {
                drawVerticalHighlightIndicator = value;
                drawHorizontalHighlightIndicator = value;
            }
        }

        protected LineScatterCandleRadarDataSet(IList<TEntry> yVals, string label) : base(yVals, label)
        {
        }
    }
}
