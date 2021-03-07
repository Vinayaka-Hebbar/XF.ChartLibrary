using System.Collections.Generic;

#if NETSTANDARD || SKIASHARP
using DashPathEffect = SkiaSharp.SKPathEffect;
#elif __ANDROID__
using DashPathEffect = Android.Graphics.DashPathEffect;
#endif

namespace XF.ChartLibrary.Data
{
    public abstract partial class LineScatterCandleRadarDataSet<TEntry> : BarLineScatterCandleBubbleDataSet<TEntry>, Interfaces.DataSets.ILineScatterCandleRadarDataSet<TEntry> where TEntry : Entry
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

#if __ANDROID__ || NETSTANDARD || SKIASHARP
        public float HighlightLineWidth { get; }

        internal DashPathEffect dashPathEffectHighlight;
        public DashPathEffect DashPathEffectHighlight
        {
            get
            {
                return dashPathEffectHighlight;
            }set
            {
                dashPathEffectHighlight = value;
            }
        }
#endif

        protected LineScatterCandleRadarDataSet(IList<TEntry> yVals, string label) : base(yVals, label)
        {
        }
    }
}
