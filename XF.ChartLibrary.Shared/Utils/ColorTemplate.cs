

#if NETSTANDARD || SKIASHARP
using Color = SkiaSharp.SKColor;
#elif __IOS__ || __TVOS
using Color = UIKit.UIColor;
#elif __ANDROID__
using Color = Android.Graphics.Color;
#endif

namespace XF.ChartLibrary.Utils
{
    public static class ColorTemplate
    {
        public static readonly Color DefaultColor = ChartUtil.FromRGB(140, 234, 255);

        public static readonly Color Black = ChartUtil.FromRGB(0, 0, 0);

        public static readonly Color DefaultValueTextColor = ChartUtil.FromRGB(140, 234, 255);
    }
}
