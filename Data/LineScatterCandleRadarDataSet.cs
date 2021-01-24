using System.Collections.Generic;


#if __ANDROID__
using DashPathEffect = Android.Graphics.DashPathEffect;
#elif NETSTANDARD
using DashPathEffect = SkiaSharp.SKPathEffect;
#endif

namespace XF.ChartLibrary.Data
{
    public abstract class LineScatterCandleRadarDataSet<TEntry> : BarLineScatterCandleBubbleDataSet<TEntry>, Interfaces.DataSets.ILineScatterCandleRadarDataSet<TEntry> where TEntry : Entry
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
        public bool DrawHorizontalHighlightIndicatorEnabled
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

#if __ANDROID__ || NETSTANDARD
        public float HighlightLineWidth { get; }
        public DashPathEffect DashPathEffectHighlight { get; }
#endif

        protected LineScatterCandleRadarDataSet(IList<TEntry> yVals, string label) : base(yVals, label)
        {
        }
    }
}
