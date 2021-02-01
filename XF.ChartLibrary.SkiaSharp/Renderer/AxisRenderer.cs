using SkiaSharp;

namespace XF.ChartLibrary.Renderer
{
    partial class AxisRenderer
    {
        protected internal SKPaint AxisLabelPaint;

        protected SKPaint GridPaint;

        protected SKPaint AxisLinePaint;

        protected SKPaint LimitLinePaint;

        protected override void Initialize()
        {
            if (ViewPortHandler != null)
            {
                AxisLabelPaint = new SKPaint()
                {
                    IsAntialias = true
                };

                GridPaint = new SKPaint
                {
                    Color = SKColors.Gray.WithAlpha(90),
                    StrokeWidth = 1f,
                    Style = SKPaintStyle.Stroke
                };

                AxisLinePaint = new SKPaint
                {
                    Color = SKColors.Black,
                    StrokeWidth = 1f,
                    Style = SKPaintStyle.Stroke
                };

                LimitLinePaint = new SKPaint
                {
                    IsAntialias = true,
                    Style = SKPaintStyle.Stroke
                };
            }
        }
    }
}
