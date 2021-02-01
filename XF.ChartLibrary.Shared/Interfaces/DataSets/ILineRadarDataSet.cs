using XF.ChartLibrary.Data;
using XF.ChartLibrary.Utils;

#if NETSTANDARD || SKIASHARP
using Color = SkiaSharp.SKColor;
#elif __IOS__ || __TVOS
    using Color = UIKit.UIColor;
#elif __ANDROID__
    using Color = Android.Graphics.Color;
#endif

namespace XF.ChartLibrary.Interfaces.DataSets
{
    public interface ILineRadarDataSet<TEntry> : ILineScatterCandleRadarDataSet<TEntry>, ILineRadarDataSet where TEntry : Entry
    { 
    }

    public interface ILineRadarDataSet : ILineScatterCandleRadarDataSet
    {
        bool DrawFilled { get; }
        IFill Fill { get; }
        Color FillColor { get; }
#if (__IOS__ || __TVOS__) && !SKIASHARP
        float FillAlpha { get; } 
#else
        byte FillAlpha { get; }
#endif
        float LineWidth { get; }
    }
}