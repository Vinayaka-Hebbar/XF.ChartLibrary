using CoreGraphics;

namespace XF.ChartLibrary.Utils
{
    public partial class Transformer
    {
        /// - Returns: The x and y values in the chart at the given touch point
        /// (x/y). This method transforms pixel coordinates to
        /// coordinates / values in the chart.
        public CGPoint ValueByTouchPoint(float x, float y)
        {
            return PixelToValueMatrix.TransformPoint(new CGPoint(x: x, y: y));
        }

        public CGAffineTransform ValueToPixelMatrix
        {
            get
            {
                return MatrixValueToPx * ViewPortHandler.touchMatrix * MatrixOffset;
            }
        }

        public CGAffineTransform PixelToValueMatrix
        {
            get
            {
                return ValueToPixelMatrix.Invert();
            }
        }

        public CGPoint PointValueToPixel(float x, float y)
        {
            return ValueToPixelMatrix.TransformPoint(new CGPoint(x, y));
        }
    }
}