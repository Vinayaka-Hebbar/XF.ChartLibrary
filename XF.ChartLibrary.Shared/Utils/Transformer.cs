#if __IOS__ || __TVOS__
using Point = CoreGraphics.CGPoint;
using Matrix = CoreGraphics.CGAffineTransform;
#elif __ANDROID__
using Point = Android.Graphics.PointF;
using Matrix = Android.Graphics.Matrix;
#elif NETSTANDARD
using Matrix = SkiaSharp.SKMatrix;
using Point = SkiaSharp.SKPoint;
#endif
namespace XF.ChartLibrary.Utils
{
    /**
 * Transformer class that contains all matrices and is responsible for
 * transforming values into pixels on the screen and backwards.
 *
 * @author Philipp Jahoda
 */
    public partial class Transformer
    {

        /**
         * matrix to map the values to the screen pixels
         */
        protected Matrix MatrixValueToPx = MatrixUtil.CreateIdentity();

        /**
         * matrix for handling the different offsets of the chart
         */
        protected Matrix MatrixOffset = MatrixUtil.CreateIdentity();

        protected ViewPortHandler ViewPortHandler;

        public Transformer(ViewPortHandler viewPortHandler)
        {
            this.ViewPortHandler = viewPortHandler;
        }

        /**
         * Prepares the matrix that transforms values to pixels. Calculates the
         * scale factors from the charts size and offsets.
         *
         * @param xChartMin
         * @param deltaX
         * @param deltaY
         * @param yChartMin
         */
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
            MatrixValueToPx = MatrixValueToPx.PostTranslateScale(scaleX ,scaleY, - xChartMin, -yChartMin);
        }

        /**
         * Prepares the matrix that contains all offsets.
         *
         * @param inverted
         */
        public void PrepareMatrixOffset(bool inverted)
        {
            if (!inverted)
                MatrixOffset = MatrixUtil.PostTranslate(MatrixOffset, ViewPortHandler.OffsetLeft,
                        ViewPortHandler.ChartHeight - ViewPortHandler.OffsetBottom);
            else
            {
                MatrixOffset = MatrixOffset.PostTranslateScale(1.0f, 1.0f, ViewPortHandler.OffsetLeft, -ViewPortHandler.OffsetTop);
            }
        }
    }
}
