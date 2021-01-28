using SkiaSharp;
using System;
using XF.ChartLibrary.Utils;

namespace XF.ChartLibrary
{
    public static partial class ChartUtil
    {
        public const double DEG2RAD = Math.PI / 180.0;
        public const float FDEG2RAD = MathF.PI / 180.0f;
        public static float LineHeight(this SKPaint self)
        {
            self.GetFontMetrics(out SKFontMetrics matrics);
            return matrics.Descent - matrics.Ascent;
        }

        public static float LineSpacing(this SKPaint self)
        {
            self.GetFontMetrics(out SKFontMetrics matrics);
            return matrics.Descent - matrics.Top + matrics.Bottom;
        }
        public static ChartSize Measure(this SKPaint self, string text)
        {
            SKRect rect = SKRect.Empty;
            self.MeasureText(text, ref rect);
            return new ChartSize(rect.Width, rect.Height);
        }

        public static float MeasureWidth(this SKPaint self, string text)
        {
            SKRect rect = SKRect.Empty;
            self.MeasureText(text, ref rect);
            return rect.Width;
        }

        public static float MeasureHeight(this SKPaint self, string text)
        {
            SKRect rect = SKRect.Empty;
            self.MeasureText(text, ref rect);
            return rect.Height;
        }

        /**
     * Returns a recyclable FSize instance.
     * Represents size of a rotated rectangle by degrees.
     *
     * @param rectangleWidth
     * @param rectangleHeight
     * @param degrees
     * @return A Recyclable FSize instance
     */
        public static ChartSize GetSizeOfRotatedRectangleByDegrees(float rectangleWidth, float
                rectangleHeight, float degrees)
        {
            return GetSizeOfRotatedRectangleByRadians(rectangleWidth, rectangleHeight, degrees * FDEG2RAD);
        }

        /**
     * Returns a recyclable FSize instance.
     * Represents size of a rotated rectangle by radians.
     *
     * @param rectangleWidth
     * @param rectangleHeight
     * @param radians
     * @return A Recyclable FSize instance
     */
        public static ChartSize GetSizeOfRotatedRectangleByRadians(float rectangleWidth, float
                rectangleHeight, float radians)
        {
            return new ChartSize(
                    MathF.Abs(rectangleWidth * (float)MathF.Cos(radians)) + MathF.Abs(rectangleHeight *
                            (float)MathF.Sin(radians)),
                    MathF.Abs(rectangleWidth * (float)MathF.Sin(radians)) + MathF.Abs(rectangleHeight *
                            (float)MathF.Cos(radians))
            );
        }
    }
}
