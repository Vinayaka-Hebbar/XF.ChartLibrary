using SkiaSharp;
using XF.ChartLibrary.Utils;

namespace XF.ChartLibrary
{
    public  static partial class ChartUtil
    {
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
    }
}
