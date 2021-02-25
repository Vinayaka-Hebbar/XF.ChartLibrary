using System;
using System.Collections.Generic;

#if NETSTANDARD || SKIASHARP
using Color = SkiaSharp.SKColor;
#elif __IOS__ || __TVOS
using Color = UIKit.UIColor;
#elif __ANDROID__
    using Color = Android.Graphics.Color;
#endif

namespace XF.ChartLibrary.Data
{
    /// <summary>
    /// Baseclass of all DataSets for Bar-, Line-, Scatter- and CandleStickChart.
    /// </summary>
    public abstract class BarLineScatterCandleBubbleDataSet<TEntry>
        : DataSet<TEntry>, Interfaces.DataSets.IBarLineScatterCandleBubbleDataSet<TEntry> where TEntry : Entry
    {
        internal Color highLightColor = ChartUtil.FromRGB(255, 187, 115);

        /// <summary>
        /// Sets the color that is used for drawing the highlight indicators.
        /// </summary>
        public Color HighLightColor
        {
            get => highLightColor;
            set => highLightColor = value;
        }

        public BarLineScatterCandleBubbleDataSet(IList<TEntry> yVals, String label) : base(yVals, label)
        {
        }

    }
}
