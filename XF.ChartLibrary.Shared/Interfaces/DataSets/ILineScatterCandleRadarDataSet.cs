using XF.ChartLibrary.Data;

#if NETSTANDARD || SKIASHARP
using DashPathEffect = SkiaSharp.SKPathEffect;
#elif __ANDROID__
using DashPathEffect = Android.Graphics.DashPathEffect;
#endif

namespace XF.ChartLibrary.Interfaces.DataSets
{
    public interface ILineScatterCandleRadarDataSet<TEntry> : IBarLineScatterCandleBubbleDataSet<TEntry> where TEntry : Entry
    {
    }

    public interface ILineScatterCandleRadarDataSet : IBarLineScatterCandleBubbleDataSet
    {
        bool DrawHorizontalHighlightIndicatorEnabled { get; }
        bool DrawVerticalHighlightIndicatorEnabled { get; }
#if __ANDROID__ || NETSTANDARD || SKIASHARP
        float HighlightLineWidth { get; }
        DashPathEffect DashPathEffectHighlight { get; }
#endif
    }
}