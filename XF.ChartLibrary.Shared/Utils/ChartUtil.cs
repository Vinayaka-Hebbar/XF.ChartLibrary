using System;

namespace XF.ChartLibrary
{

#if NETSTANDARD || SKIASHARP
    using Color = SkiaSharp.SKColor;
#elif __IOS__ || __TVOS
    using Color = UIKit.UIColor;
#elif __ANDROID__
    using Color = Android.Graphics.Color;
#endif
    public static partial class ChartUtil
    {
        public static void InvalidateView(this Charts.IChartBase chart)
        {

#if SKIASHARP && !NATIVE
          chart.InvalidateSurface();
#elif __ANDROID__
        chart.Invalidate();
#elif __IOS__ || __TVOS__
        chart.SetNeedsDisplay();
#elif WPF
            chart.InvalidateVisual();
#endif
        }

        public static Color FromRGB(byte r, byte g, byte b)
        {
#if (__IOS__ || __TVOS__) && !SKIASHARP
            return Color.FromRGB(r/255f, g/255f, b/255f);
#else
            return new Color(r, g, b);
#endif
        }

        public static float Hypot(float x, float y) => MathF.Sqrt(x * x + y * y);
    }
}
