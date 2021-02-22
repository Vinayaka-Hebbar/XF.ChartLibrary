

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

        public static readonly Color[] DefaultColors = new Color[] { DefaultColor };

        public static readonly Color[] DefaultValueColors = new Color[] { Black };

        public static readonly Color Black = ChartUtil.FromRGB(0, 0, 0);

        public static readonly Color DefaultValueTextColor = ChartUtil.FromRGB(140, 234, 255);

        public static readonly Color[] LibreryColors = {
            ChartUtil.FromRGB(207, 248, 246), ChartUtil.FromRGB(148, 212, 212), ChartUtil.FromRGB(136, 180, 187),
            ChartUtil.FromRGB(118, 174, 175), ChartUtil.FromRGB(42, 109, 130)
        };

        public static readonly Color[] JoyfulColors = {
            ChartUtil.FromRGB(217, 80, 138), ChartUtil.FromRGB(254, 149, 7), ChartUtil.FromRGB(254, 247, 120),
            ChartUtil.FromRGB(106, 167, 134), ChartUtil.FromRGB(53, 194, 209)
        };
    }
}
