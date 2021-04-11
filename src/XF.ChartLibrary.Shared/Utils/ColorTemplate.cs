

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
        public static readonly Color DefaultColor = FromRGB(140, 234, 255);

        public static readonly Color HoleBlue = FromRGB(51, 181, 229);

        public static readonly Color[] DefaultColors = new Color[] { DefaultColor };

        public static readonly Color[] DefaultValueColors = new Color[] { Black };

        public static readonly Color Black = FromRGB(0, 0, 0);

        public static readonly Color White = FromRGB(255, 255, 255);

        public static readonly Color DefaultValueTextColor = FromRGB(140, 234, 255);

        public static readonly Color[] LibreryColors = {
            FromRGB(207, 248, 246), FromRGB(148, 212, 212), FromRGB(136, 180, 187),
            FromRGB(118, 174, 175), FromRGB(42, 109, 130)
        };

        public static readonly Color[] JoyfulColors = {
            FromRGB(217, 80, 138), FromRGB(254, 149, 7), FromRGB(254, 247, 120),
            FromRGB(106, 167, 134), FromRGB(53, 194, 209)
        };

        public static readonly Color[] PastelColors =
        {
            FromRGB(64, 89, 128),
            FromRGB(149, 165, 124),
            FromRGB(217, 184, 162),
            FromRGB(191, 134, 134),
            FromRGB(179, 48, 80)
        };

        public static readonly Color[] VordiplomColors =
        {
            FromRGB(192, 255, 140),
            FromRGB(255, 247, 140),
            FromRGB(255, 208, 140),
            FromRGB(140, 234, 255),
            FromRGB(255, 140, 157)
        };

        public static readonly Color[] MaterialColors = {
            FromRGB(46, 204, 113),
            FromRGB(241, 196, 15),
            FromRGB(231, 76, 60),
            FromRGB(52, 152, 219)
        };

        public static Color FromRGB(byte r, byte g, byte b)
        {
#if (__IOS__ || __TVOS__) && !SKIASHARP
            return Color.FromRGB(r/255f, g/255f, b/255f);
#else
            return new Color(r, g, b);
#endif
        }

        public static Color FromRGB(byte r, byte g, byte b, byte a)
        {
#if (__IOS__ || __TVOS__) && !SKIASHARP
            return Color.FromRGB(r/255f, g/255f, b/255f, a/ 255f);
#else
            return new Color(r, g, b, a);
#endif
        }
    }
}
