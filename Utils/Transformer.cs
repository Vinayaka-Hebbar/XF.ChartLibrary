using System;
using System.Collections.Generic;
using System.Text;
using XF.ChartLibrary.Data;
#if __IOS__ || __TVOS__
using Point = CoreGraphics.CGPoint;
using Matrix = CoreGraphics.CGAffineTransform;
#elif __ANDROID__
using Point = Android.Graphics.PointF;
using Matrix = Android.Graphics.Matrix;
#elif NETSTANDARD
using Point = SkiaSharp.SKPoint;
using Matrix = SkiaSharp.SKMatrix;
#endif
namespace XF.ChartLibrary.Utils
{
    /**
 * Transformer class that contains all matrices and is responsible for
 * transforming values into pixels on the screen and backwards.
 *
 * @author Philipp Jahoda
 */
    public class Transformer
    {

        /**
         * matrix to map the values to the screen pixels
         */
        protected Matrix mMatrixValueToPx = MatrixUtil.CreateIdentity();

        /**
         * matrix for handling the different offsets of the chart
         */
        protected Matrix mMatrixOffset = MatrixUtil.CreateIdentity();

        protected ViewPortHandler mViewPortHandler;

        public Transformer(ViewPortHandler viewPortHandler)
        {
            this.mViewPortHandler = viewPortHandler;
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

            float scaleX = (float)((mViewPortHandler.ContentWidth) / deltaX);
            float scaleY = (float)((mViewPortHandler.ContentHeight) / deltaY);

            if (float.IsInfinity(scaleX))
            {
                scaleX = 0;
            }
            if (float.IsInfinity(scaleY))
            {
                scaleY = 0;
            }

            // setup all matrices
            mMatrixValueToPx = mMatrixValueToPx.PostTranslateScale(scaleX ,scaleY, - xChartMin, -yChartMin);
        }

        /**
         * Prepares the matrix that contains all offsets.
         *
         * @param inverted
         */
        public void PrepareMatrixOffset(bool inverted)
        {
            if (!inverted)
                mMatrixOffset = MatrixUtil.PostTranslate(mMatrixOffset, mViewPortHandler.OffsetLeft,
                        mViewPortHandler.ChartHeight - mViewPortHandler.OffsetBottom);
            else
            {
                mMatrixOffset = mMatrixOffset.PostTranslateScale(1.0f, 1.0f, mViewPortHandler.OffsetLeft, -mViewPortHandler.OffsetTop);
            }
        }
    }
}
