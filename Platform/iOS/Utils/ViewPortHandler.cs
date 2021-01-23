using CoreGraphics;

namespace XF.ChartLibrary.Utils
{
    public partial class ViewPortHandler
    {
        private CGAffineTransform _touchMatrix = CGAffineTransform.MakeIdentity();

        private CGRect contentRect = new CGRect();

        public void RestrainViewPort(float offsetLeft, float offsetTop, float offsetRight, float offsetBottom)
        {
            contentRect.Offset(offsetLeft, offsetTop);
            contentRect.Size = new CGSize(chartWidth - offsetRight, chartHeight
                - offsetBottom);
        }

        public CGAffineTransform Zoom(float scaleX, float scaleY, float x, float y)
        {
            var matrix = _touchMatrix;
            matrix.Translate(x, y);
            matrix.Scale(scaleX, scaleY);
            matrix.Translate(-x, -y);
            return matrix;
        }


        /**
         * Resets all zooming and dragging and makes the chart fit exactly it's
         * bounds.
         */
        public CGAffineTransform FitScreen()
        {
            minScaleX = 1.0f;
            minScaleY = 1.0f;

            return CGAffineTransform.MakeIdentity();
        }
    }
}
