using XF.ChartLibrary.Utils;
#if __IOS__ || __TVOS
    using Color = UIKit.UIColor;
#elif __ANDROID__
    using Color = Android.Graphics.Color;
#elif NETSTANDARD
using Color = SkiaSharp.SKColor;
#endif
namespace XF.ChartLibrary.Data
{
    public interface ILineRadarDataSet<TEntry> : ILineScatterCandleRadarDataSet<TEntry> where TEntry : Entry
    {
        bool DrawFilled { get; }
        IFill Fill { get; }
        Color FillCOlor { get; }
        float LineWidth { get; }
    }
}