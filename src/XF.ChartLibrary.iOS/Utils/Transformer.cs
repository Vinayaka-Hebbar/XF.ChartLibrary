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

        /// <summary>
        /// Transform a rectangle with all matrices.
        /// </summary>
        /// <param name="r"></param>
        public CGRect RectValueToPixel(CGRect r)
        {
            return ValueToPixelMatrix.TransformRect(r);
        }

        /// <summary>
        ///  Prepares the matrix that transforms values to pixels. Calculates the
        ///scale factors from the charts size and offsets.
        /// </summary>
        public void PrepareMatrixValuePx(float xChartMin, float deltaX, float deltaY, float yChartMin)
        {
            float scaleX = (float)((ViewPortHandler.ContentWidth) / deltaX);
            float scaleY = (float)((ViewPortHandler.ContentHeight) / deltaY);

            if (float.IsInfinity(scaleX))
            {
                scaleX = 0;
            }
            if (float.IsInfinity(scaleY))
            {
                scaleY = 0;
            }

            // setup all matrices
            MatrixValueToPx = CGAffineTransform.MakeScale(scaleX, -scaleY).TranslatedBy(-xChartMin, -yChartMin);
        }

        /// <summary>
        /// Prepares the matrix that contains all offsets.
        /// </summary>
        public void PrepareMatrixOffset(bool inverted)
        {
            if (!inverted)
                MatrixOffset = CGAffineTransform.MakeTranslation(ViewPortHandler.OffsetLeft,
                        ViewPortHandler.ChartHeight - ViewPortHandler.OffsetBottom);
            else
            {
                MatrixOffset = CGAffineTransform.MakeScale(1f, -1f).TranslatedBy(ViewPortHandler.OffsetLeft, -ViewPortHandler.OffsetTop);
            }
        }
    }
}