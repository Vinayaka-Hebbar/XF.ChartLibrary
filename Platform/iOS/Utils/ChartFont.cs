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

        public ChartSize Measure(ChartString text)
        {
            var size = UIKit.NSStringDrawing.GetSizeUsingAttributes(text.Value, new UIKit.UIStringAttributes { Font = Value });
            return new ChartSize((float)size.Width, (float)size.Height);
        }

        public float MeasureWidth(ChartString text)
        {
            return (float)(UIKit.NSStringDrawing.GetSizeUsingAttributes(text.Value, new UIKit.UIStringAttributes { Font = Value }).Width);
        }
    }
}
