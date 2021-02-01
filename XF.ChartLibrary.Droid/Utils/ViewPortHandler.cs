using Android.Graphics;

namespace XF.ChartLibrary.Utils
{
    public partial class ViewPortHandler
    {
        internal Matrix touchMatrix = new Matrix();

        private System.Drawing.RectangleF contentRect = new System.Drawing.RectangleF();

        public void RestrainViewPort(float offsetLeft, float offsetTop, float offsetRight, float offsetBottom)
        {
            contentRect.Offset(offsetLeft, offsetTop);
            contentRect.Size = new System.Drawing.SizeF(chartWidth - offsetRight, chartHeight - offsetBottom);
        }

        public Matrix Zoom(float scaleX, float scaleY, float x, float y)
        {
            var matrix = new Matrix();
            matrix.Reset();
            matrix.Set(touchMatrix);
            matrix.PostScale(scaleX, scaleY, x, y);
            return matrix;
        }

        /**
         * Zooms out to original size.
         * @param outputMatrix
         */
        public void ResetZoom(Matrix outputMatrix)
        {
            outputMatrix.Reset();
            outputMatrix.Set(touchMatrix);
            outputMatrix.PostScale(1.0f, 1.0f, 0.0f, 0.0f);
        }

        /// <summary>
        /// Resets all zooming and dragging and makes the chart fit exactly it's
        /// bounds.
        /// </summary>
        /// <returns></returns>
        public Matrix FitScreen()
        {
            Matrix save = new Matrix();
            FitScreen(save);
            return save;
        }


        protected float[] valsBufferForFitScreen = new float[9];
        /**
         * Resets all zooming and dragging and makes the chart fit exactly it's
         * bounds.  Output Matrix is available for those who wish to cache the object.
         */
        public void FitScreen(Matrix outputMatrix)
        {
            minScaleX = 1f;
            minScaleY = 1f;

            outputMatrix.Set(touchMatrix);

            float[] vals = valsBufferForFitScreen;
            for (int i = 0; i < 9; i++)
            {
                vals[i] = 0;
            }

            outputMatrix.GetValues(vals);

            // reset all translations and scaling
            vals[Matrix.MtransX] = 0f;
            vals[Matrix.MtransY] = 0f;
            vals[Matrix.MscaleX] = 1f;
            vals[Matrix.MscaleY] = 1f;

            outputMatrix.SetValues(vals);
        }
    }
}
