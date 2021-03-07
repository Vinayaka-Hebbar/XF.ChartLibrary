using SkiaSharp;

namespace XF.ChartLibrary.Components
{
    public enum StrokeStyle
    {
        Line,
        Dotted,
        Dashed,
    }

    public class Path : IDrawable
    {
        private float strokeSize;
        private SKStrokeCap cap;
        private readonly SKPath source;
        private StrokeStyle style;
        private SKStrokeJoin join;
        private SKColor fill;
        private SKColor stroke;

        public Path(SKPath path, SKStrokeCap cap, SKStrokeJoin join)
        {
            source = path;
            this.cap = cap;
            this.join = join;
        }

        public StrokeStyle Style
        {
            get => style;
            set => style = value;
        }

        public float StrokeSize
        {
            get => strokeSize;
            set => strokeSize = value;
        }

        public SKPath Source { get => source; }

        public SKStrokeCap Cap
        {
            get => cap;
            set => cap = value;
        }

        public SKStrokeJoin Join
        {
            get => join;
            set => join = value;
        }

        public SKColor Fill
        {
            get => fill;
            set => fill = value;
        }

        public SKColor Stroke
        {
            get => stroke;
            set => stroke = value;
        }

        public void Draw(SKCanvas canvas, float x, float y)
        {
            var path = new SKPath(Source);
            path.Offset(x, y);
            using var paint = new SKPaint
            {
                IsAntialias = true,
                Color = fill,
                Style = SKPaintStyle.Fill,
            };
            if (fill != SKColor.Empty)
            {
                canvas.DrawPath(path, paint);
            }
            if (stroke != SKColor.Empty)
            {
                float size = strokeSize.DpToPixel();
                paint.Color = stroke;
                paint.Style = SKPaintStyle.Stroke;
                paint.StrokeCap = cap;
                paint.StrokeWidth = size;
                paint.StrokeJoin = join;
                if (style == StrokeStyle.Dotted)
                {
                    paint.PathEffect = SKPathEffect.CreateDash(new[] { 0, size * 2, 0, size * 2 }, 0);
                }
                else if (style == StrokeStyle.Dashed)
                {
                    paint.PathEffect = SKPathEffect.CreateDash(new[] { size * 6, size * 2 }, 0);
                }
                canvas.DrawPath(path, paint);
            }
        }
    }
}
