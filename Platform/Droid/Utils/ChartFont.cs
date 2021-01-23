using Android.Graphics;

namespace XF.ChartLibrary.Utils
{
    public partial class ChartFont
    {
        public readonly Paint Value;

        public ChartFont(Paint paint)
        {
            Value = paint;
        }

        public float LineHeight
        {
            get
            {
                var matrics = Value.GetFontMetrics();
                return matrics.Descent - matrics.Ascent;
            }
        }

        public float LineSpacing
        {
            get
            {
                var matrics = Value.GetFontMetrics();
                return matrics.Descent - matrics.Top + matrics.Bottom;
            }
        }

        public static ChartFont OfSize(string name, float size)
        {
            Paint paint = new Paint
            {
                TextSize = (float)size,
            };
            paint.SetTypeface(Typeface.Create(name, TypefaceStyle.Normal));
            return new ChartFont(paint);
        }

        public static ChartFont SystemFont(double size)
        {
            return new ChartFont(new Paint
            {
                TextSize = (float)size

            });
        }

        static Rect calcTextHeightRect = new Rect();

        public ChartSize Measure(ChartString text)
        {
            var r = calcTextHeightRect;
            Value.GetTextBounds(text.Value, 0, text.Length, r);
            return new ChartSize(r.Width(), r.Height());
        }

        public float MeasureWidth(ChartString text)
        {
            var r = calcTextHeightRect;
            Value.GetTextBounds(text.Value, 0, text.Length, r);
            return r.Width();
        }
    }
}
