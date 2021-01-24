namespace XF.ChartLibrary.Utils
{
    public partial class ChartFont
    {
        public readonly UIKit.UIFont Value;

        public ChartFont(UIKit.UIFont font)
        {
            Value = font;
        }

        public float LineHeight => (float)Value.LineHeight;

        public static ChartFont OfSize(string name, float size)
        {
            return new ChartFont(UIKit.UIFont.FromName(name, size: (float)size));
        }

        public static ChartFont SystemFont(float size)
        {
            return new ChartFont(UIKit.UIFont.SystemFontOfSize((float)size));
        }

        public ChartSize Measure(string text)
        {
            var size = UIKit.UIStringDrawing.StringSize(text, Value);
            return new ChartSize((float)size.Width, (float)size.Height);
        }

        public float MeasureWidth(string text)
        {
            return (float)(UIKit.UIStringDrawing.StringSize(text, Value).Width);
        }
    }
}
