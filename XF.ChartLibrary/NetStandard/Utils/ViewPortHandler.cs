using SkiaSharp;

namespace XF.ChartLibrary.Utils
{
    public partial class ViewPortHandler
    {
        protected internal SKMatrix TouchMatrix = SKMatrix.CreateIdentity();

        private SKRect contentRect = new SKRect();

        public SKMatrix MatrixTouch => TouchMatrix;

        public SKRect ContentRect => contentRect;

        public void RestrainViewPort(float offsetLeft, float offsetTop, float offsetRight, float offsetBottom)
        {
            contentRect.Offset(offsetLeft, offsetTop);
            contentRect.Size = new SKSize(chartWidth - offsetRight, chartHeight
                - offsetBottom);
        }

        public SKMatrix Zoom(float scaleX, float scaleY, float x, float y)
        {
            return SKMatrix.CreateScaleTranslation(scaleX, scaleY, x, y);
        }

        /**
         * Resets all zooming and dragging and makes the chart fit exactly it's
         * bounds.
         */
        public SKMatrix FitScreen()
        {
            minScaleX = 1.0f;
            minScaleY = 1.0f;

            return SKMatrix.CreateIdentity();
        }

    }
}
