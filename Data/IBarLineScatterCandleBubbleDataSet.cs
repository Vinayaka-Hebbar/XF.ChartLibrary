#if __IOS__ || __TVOS
using Color = UIKit.UIColor;
#elif __ANDROID__
    using Color = Android.Graphics.Color;
#elif NETSTANDARD
using Color = SkiaSharp.SKColor;
#endif
namespace XF.ChartLibrary.Data
{
    public interface IBarLineScatterCandleBubbleDataSet<TEntry> : IDataSet<TEntry> where TEntry : Entry
    {
        Color HighLightColor { get; }
    }
}