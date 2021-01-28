using Android.Graphics;

namespace XF.ChartLibrary.Utils
{
    partial class Transformer
    {
        /// <summary>
        /// buffer for performance
        /// </summary>
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
            ptsBuffer[0] = x;
            ptsBuffer[1] = y;
            PixelsToValue(ptsBuffer);

            outputPoint.X = ptsBuffer[0];
            outputPoint.Y = ptsBuffer[1];
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

    }
}
