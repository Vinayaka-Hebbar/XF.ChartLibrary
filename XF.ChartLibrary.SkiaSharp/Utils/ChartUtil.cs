using SkiaSharp;
using System;
using XF.ChartLibrary.Utils;

namespace XF.ChartLibrary
{
    public static partial class ChartUtil
    {
        public const double DEG2RAD = Math.PI / 180.0;

        public const float FDEG2RAD = MathF.PI / 180.0f;

        public static SKPoint DpToPixel(this SKPoint self)
        {
            self.X *= density;
            self.Y *= density;
            return self;
        }

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

        public static SKRect InsetVertically(this SKRect self, float value)
        {
            self.Top -= value;
            self.Bottom += value;
            return self;
        }

        public static SKRect Inset(this SKRect self, SKRect value)
        {
            self.Left -= value.Left;
            self.Right += value.Right;
            self.Top -= value.Top;
            self.Bottom += value.Bottom;
            return self;
        }

        public static SKRect InsetHorizontally(this SKRect self, float value)
        {
            self.Left -= value;
            self.Right += value;
            return self;
        }

        /// <summary>
        /// Represents size of a rotated rectangle by degrees.
        /// </summary>
        /// <returns>A Size Represents size of a rotated rectangle by degrees. </returns>
        public static ChartSize GetSizeOfRotatedRectangleByDegrees(float rectangleWidth, float
                rectangleHeight, float degrees)
        {
            return GetSizeOfRotatedRectangleByRadians(rectangleWidth, rectangleHeight, degrees * FDEG2RAD);
        }

        /// <summary>
        /// Represents size of a rotated rectangle by degrees.
        /// </summary>
        /// <returns>A Size Represents size of a rotated rectangle by degrees. </returns>
        public static ChartSize GetSizeOfRotatedRectangleByRadians(float rectangleWidth, float
                rectangleHeight, float radians)
        {
            return new ChartSize(
                    Math.Abs(rectangleWidth * (float)Math.Cos(radians)) + Math.Abs(rectangleHeight *
                            (float)Math.Sin(radians)),
                    Math.Abs(rectangleWidth * (float)Math.Sin(radians)) + Math.Abs(rectangleHeight *
                            (float)Math.Cos(radians))
            );
        }
    }
}
