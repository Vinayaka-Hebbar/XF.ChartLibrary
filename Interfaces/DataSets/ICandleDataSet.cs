namespace XF.ChartLibrary.Interfaces.DataSets
{
#if __IOS__ || __TVOS
using Color = UIKit.UIColor;
#elif __ANDROID__
    using Color = Android.Graphics.Color;
#elif NETSTANDARD
    using Color = SkiaSharp.SKColor;
#endif

    public interface ICandleDataSet : ILineScatterCandleRadarDataSet<Data.CandleEntry>
    {
        /// <summary>
        /// Returns the increasing color (for open &lt; close).
        /// </summary>
        Color IncreasingColor { get; }

        /// <summary>
        /// Returns the decreasing color (for open &gt; close).
        /// </summary>
        Color DecreasingColor { get; }
    }
}
