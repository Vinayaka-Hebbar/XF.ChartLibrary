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

    }
}
