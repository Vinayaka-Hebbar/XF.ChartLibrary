using XF.ChartLibrary.Data;
using XF.ChartLibrary.Utils;
#if __IOS__ || __TVOS
    using Color = UIKit.UIColor;
#elif __ANDROID__
    using Color = Android.Graphics.Color;
#elif NETSTANDARD
using Color = SkiaSharp.SKColor;
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
#if __IOS__ || __TVOS__
        float FillAlpha { get; } 
#else
        byte FillAlpha { get; }
#endif
        float LineWidth { get; }
    }
}