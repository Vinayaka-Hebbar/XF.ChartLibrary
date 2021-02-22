using SkiaSharp;

namespace XF.ChartLibrary.Utils
{
    public readonly struct GradientFill : IFill
    {
        public GradientFill(params SKColor[] gradientColors)
        {
            GradientColors = gradientColors;
            ColorPos = null;
        }

        public readonly SKColor[] GradientColors { get; }

        public readonly float[] ColorPos { get; }

        public void Draw(SKCanvas c, SKPath path, SKPaint paint, SKRect rect)
        {
            if (GradientColors == null)
                return;

            using (var gradient = SKShader.CreateLinearGradient(
                    SKPoint.Empty,
                    new SKPoint(rect.Width,
                    rect.Height),
                   GradientColors,
                    ColorPos,
                    SKShaderTileMode.Mirror))
                paint.Shader = gradient;

            c.DrawPath(path, paint);
        }
    }
}
