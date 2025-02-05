﻿using SkiaSharp;

namespace XF.ChartLibrary.Utils
{
    public readonly struct GradientFill : IFill
    {
        public GradientFill(params SKColor[] gradientColors)
        {
            GradientColors = gradientColors;
            ColorPos = null;
        }

        public GradientFill(SKColor[] gradientColors, float[] colorPos)
        {
            GradientColors = gradientColors;
            ColorPos = colorPos;
        }

        public readonly SKColor[] GradientColors { get; }

        public readonly float[] ColorPos { get; }

        public void Draw(SKCanvas c, SKPath path, SKPaint paint, SKRect rect)
        {
            if (GradientColors == null)
                return;
            int save = c.Save();
            c.ClipPath(path);
            using (var gradient = SKShader.CreateLinearGradient(
                    SKPoint.Empty,
                    new SKPoint(rect.Width,
                    rect.Height),
                   GradientColors,
                    ColorPos,
                    SKShaderTileMode.Mirror))
            {

                var previousStyle = paint.Style;
                paint.Style = SKPaintStyle.Fill;
                paint.Shader = gradient;
                c.DrawPath(path, paint);
                paint.Style = previousStyle;
            }

            c.RestoreToCount(save);
        }
    }
}
