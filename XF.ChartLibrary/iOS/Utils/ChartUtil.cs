using UIKit;

namespace XF.ChartLibrary.Utils
{
    public static partial class ChartUtil
    {
        public static ChartSize Measure(this UIFont self, string text)
        {
            var size = UIStringDrawing.StringSize(text, self);
            return new ChartSize((float)size.Width, (float)size.Height);
        }

        public static float MeasureWidth(this UIFont self, string text)
        {
            return (float)UIStringDrawing.StringSize(text, self).Width;
        }
    }
}
