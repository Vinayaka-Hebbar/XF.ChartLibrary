namespace XF.ChartLibrary.Utils
{
    public partial class ChartFont
    {
        public readonly SkiaSharp.SKPaint Value;

        public ChartFont(SkiaSharp.SKFont font)
        {
            Value = new SkiaSharp.SKPaint(font);
        }

        public float LineHeight
        {
            get
            {
                Value.GetFontMetrics(out SkiaSharp.SKFontMetrics matrics);
                return matrics.Descent - matrics.Ascent;
            }
        }

        public float LineSpacing
        {
            get
            {
                Value.GetFontMetrics(out SkiaSharp.SKFontMetrics matrics);
                return matrics.Descent - matrics.Top + matrics.Bottom;
            }
        }

        public static ChartFont OfSize(string name, float size)
        {
            return new ChartFont(new SkiaSharp.SKFont
            {
                Size = (float)size,
                Typeface = SkiaSharp.SKTypeface.FromFamilyName(name)

            });
        }

        public static ChartFont SystemFont(float size)
        {
            return new ChartFont(new SkiaSharp.SKFont
            {
                Size = (float)size

            });
        }
    }
}
