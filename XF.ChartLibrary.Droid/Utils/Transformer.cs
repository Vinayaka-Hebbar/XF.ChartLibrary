using Android.Graphics;

namespace XF.ChartLibrary.Utils
{
    partial class Transformer
    {
        /// <summary>
        /// buffer for performance
        /// </summary>
        readonly float[] touchPointBuffer = new float[2];
        readonly float[] ptsBuffer = new float[2];

        /// <summary>
        /// Transform a rectangle with all matrices.
        /// </summary>
        /// <param name="r"></param>
        public void RectValueToPixel(RectF r)
        {
            MatrixValueToPx.MapRect(r);
            ViewPortHandler.touchMatrix.MapRect(r);
            MatrixOffset.MapRect(r);
        }

        /// <summary>
        /// Returns a recyclable MPPointD instance.
        /// </summary>
        /// <returns>the x and y values in the chart at the given touch point
        /// (encapsulated in a MPPointD). This method transforms pixel coordinates to
        /// coordinates / values in the chart.This is the opposite method to
        /// getPixelForValues(...).</returns>
        public PointF ValueByTouchPoint(float x, float y)
        {
            var result = new PointF(0, 0);
            ValueByTouchPoint(x, y, result);
            return result;
        }

        public void ValueByTouchPoint(float x, float y, PointF outputPoint)
        {
            touchPointBuffer[0] = x;
            touchPointBuffer[1] = y;
            PixelsToValue(touchPointBuffer);

            outputPoint.X = touchPointBuffer[0];
            outputPoint.Y = touchPointBuffer[1];
        }

        protected Matrix mPixelToValueMatrixBuffer = new Matrix();

        /// <summary>
        /// Transforms the given array of touch positions(pixels) (x, y, x, y, ...)
        /// into values on the chart.
        /// </summary>
        public void PixelsToValue(float[] pixels)
        {
            Matrix tmp = mPixelToValueMatrixBuffer;
            tmp.Reset();

            // invert all matrixes to convert back to the original value
            MatrixOffset.Invert(tmp);
            tmp.MapPoints(pixels);

            ViewPortHandler.touchMatrix.Invert(tmp);
            tmp.MapPoints(pixels);

            MatrixValueToPx.Invert(tmp);
            tmp.MapPoints(pixels);
        }

        public void PointValueToPixel(float[] points)
        {
            MatrixValueToPx.MapPoints(points);
            MatrixOffset.MapPoints(points);
            ViewPortHandler.touchMatrix.MapPoints(points);

        }

        public PointF PointValueToPixel(float x, float y)
        {
            ptsBuffer[0] = x;
            ptsBuffer[1] = y;
            PointValueToPixel(ptsBuffer);
            return new PointF(ptsBuffer[0], ptsBuffer[1]); 

        }

        /// <summary>
        /// Prepares the matrix that contains all offsets.
        /// </summary>
        public void PrepareMatrixOffset(bool inverted)
        {
            MatrixOffset.Reset();
            if (!inverted)
                MatrixOffset.PostTranslate(ViewPortHandler.OffsetLeft,
                        ViewPortHandler.ChartHeight - ViewPortHandler.OffsetBottom);
            else
            {
                MatrixOffset.SetTranslate(ViewPortHandler.OffsetLeft, -ViewPortHandler.OffsetTop);
                MatrixOffset.PostScale(1f, -1f);
            }
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
            MatrixValueToPx.Reset();
            MatrixValueToPx.PostTranslate(-xChartMin, -yChartMin);
            MatrixValueToPx.PostScale(scaleX, -scaleY);
        }
    }
}
