using System;
using System.Collections.Generic;

#if __IOS__ || __TVOS
using Color = UIKit.UIColor;
#elif __ANDROID__
    using Color = Android.Graphics.Color;
#elif NETSTANDARD
using Color = SkiaSharp.SKColor;
#endif

namespace XF.ChartLibrary.Data
{
    /// <summary>
    /// Baseclass of all DataSets for Bar-, Line-, Scatter- and CandleStickChart.
    /// </summary>
    public abstract class BarLineScatterCandleBubbleDataSet<TEntry>
        : DataSet<TEntry>, IBarLineScatterCandleBubbleDataSet<TEntry> where TEntry : Entry
    {

        /// <summary>
        /// Sets the color that is used for drawing the highlight indicators.
        /// </summary>
        public Color HighLightColor { get; set; } = ChartUtil.FromRGB(255, 187, 115);

        public BarLineScatterCandleBubbleDataSet(IList<TEntry> yVals, String label) : base(yVals, label)
        {
        }

    }
}
