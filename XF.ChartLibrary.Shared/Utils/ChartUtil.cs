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
        public const double DegToRad = Math.PI / 180.0;
        public const float FDegToRad = MathF.PI / 180.0f;

        public const double AngToDeg = 180.0 / Math.PI;
        public const float FAngToDeg = 180.0f / MathF.PI;

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

        public static float Hypot(float x, float y) => MathF.Sqrt(x * x + y * y);


        /// <summary>
        ///  returns an angle between 0.f < 360.f (not less than zero, less than 360)
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static float GetNormalizedAngle(float angle)
        {
            while (angle < 0.0f)
                angle += 360.0f;

            return angle % 360.0f;
        }
    }
}
