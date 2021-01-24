using XF.ChartLibrary.Data;

#if __ANDROID__
using DashPathEffect = Android.Graphics.DashPathEffect;
#elif NETSTANDARD
using DashPathEffect = SkiaSharp.SKPathEffect;
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
#if __ANDROID__ || NETSTANDARD
        float HighlightLineWidth { get; }
        DashPathEffect DashPathEffectHighlight { get; }
#endif
    }
}