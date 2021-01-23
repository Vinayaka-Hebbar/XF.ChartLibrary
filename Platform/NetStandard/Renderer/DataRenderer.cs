using SkiaSharp;
using XF.ChartLibrary.Formatter;

namespace XF.ChartLibrary.Renderer
{
    public partial class DataRenderer
    {
        protected SKPaint ValuePaint;

        partial void Initialize()
        {
            ValuePaint = new SKPaint
            {
                IsAntialias = true,
                Color = new SKColor(63, 63, 63),
                TextAlign = SKTextAlign.Center,
                TextSize = 9f
            };
        }

        /// <summary>
        /// Draws the value of the given entry by using the provided IValueFormatter.
        /// </summary>
        /// <param name="c">canvas</param>
        /// <param name="formatter">formatter for custom value-formatting</param>
        /// <param name="value">the value to be drawn</param>
        /// <param name="entry">the entry the value belongs to</param>
        /// <param name="dataSetIndex">the index of the DataSet the drawn Entry belongs to</param>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        /// <param name="color"></param>
        public void DrawValue(SKCanvas c, IValueFormatter formatter, double value, Data.Entry entry, int dataSetIndex, float x, float y, SKColor color)
        {
            ValuePaint.Color = (color);
            c.DrawText(formatter.GetFormattedValue(value, entry, dataSetIndex, ViewPortHandler), x, y, ValuePaint);
        }

    }
}
