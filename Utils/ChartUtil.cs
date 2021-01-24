using System;

namespace XF.ChartLibrary
{
#if __IOS__ || __TVOS
    using Color = UIKit.UIColor;
#elif __ANDROID__
    using Color = Android.Graphics.Color;
#elif NETSTANDARD
    using Color = SkiaSharp.SKColor;
#endif
    public static partial class ChartUtil
    {
        public static void InvalidateView(this Charts.IChartBase chart)
        {
#if __ANDROID__
        chart.Invalidate();
#elif __IOS__ || __TVOS__
        chart.SetNeedsDisplay();
#elif NETSTANDARD
            chart.InvalidateSurface();
#endif
        }

        public static Color FromRGB(byte r, byte g, byte b)
        {
#if __IOS__ || __TVOS__
            return Color.FromRGB(r/255f, g/255f, b/255f);
#else
            return new Color(r, g, b);
#endif
        }
    }
}
