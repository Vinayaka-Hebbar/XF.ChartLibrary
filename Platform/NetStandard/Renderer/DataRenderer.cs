using SkiaSharp;
using XF.ChartLibrary.Formatter;

namespace XF.ChartLibrary.Renderer
{
    public partial class DataRenderer
    {
        /// <summary>
        /// paint object for drawing values (text representing values of chart
        /// entries)
        /// </summary>
        protected SKPaint ValuePaint;

        /// <summary>
        /// main paint object used for rendering
        /// </summary>
        protected SKPaint RenderPaint;

        /// <summary>
        /// paint used for highlighting values
        /// </summary>
        protected SKPaint HighlightPaint;

        protected virtual void Initialize()
        {
            RenderPaint = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Fill
            };
            ValuePaint = new SKPaint
            {
                IsAntialias = true,
                Color = new SKColor(63, 63, 63),
                TextAlign = SKTextAlign.Center,
                TextSize = 9f
            };
            HighlightPaint = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 2f,
                Color = new SKColor(255, 187, 115)
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
        public void DrawValue(SKCanvas c, IValueFormatter formatter, float value, Data.Entry entry, int dataSetIndex, float x, float y, SKColor color)
        {
            ValuePaint.Color = (color);
            c.DrawText(formatter.GetFormattedValue(value, entry, dataSetIndex, ViewPortHandler), x, y, ValuePaint);
        }

    }
}
