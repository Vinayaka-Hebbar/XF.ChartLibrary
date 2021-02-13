using Android.Graphics;
using System;
using System.Drawing;
using XF.ChartLibrary.Charts;

namespace XF.ChartLibrary.Utils
{
    public partial class ViewPortHandler
    {
        internal Matrix touchMatrix = new Matrix();

        private RectangleF contentRect = new RectangleF();

        public void RestrainViewPort(float offsetLeft, float offsetTop, float offsetRight, float offsetBottom)
        {
            contentRect.Offset(offsetLeft, offsetTop);
            contentRect.Size = new SizeF(chartWidth - offsetRight, chartHeight - offsetBottom);
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

        protected Matrix CenterViewPortMatrixBuffer = new Matrix();


        /// <summary>
        ///  Centers the viewport around the specified position(x-index and y-value)
        /// in the chart.Centering the viewport outside the bounds of the chart is
        /// not possible. Makes most sense in combination with the
        /// setScaleMinima(...) method.
        /// </summary>
        /// <param name="transformedPts">the position to center view viewport to</param>
        /// <param name="view"></param>
        public void CenterViewPort(float[] transformedPts, IChartBase view)
        {
            Matrix save = CenterViewPortMatrixBuffer;
            save.Reset();
            save.Set(touchMatrix);

            float x = transformedPts[0] - OffsetLeft;
            float y = transformedPts[1] - OffsetTop;

            save.PostTranslate(-x, -y);

            Refresh(save, view, true);
        }

        protected readonly float[] MatrixBuffer = new float[9];

        public Matrix Refresh(Matrix newMtrix, IChartBase chart, bool invalidate)
        {
            touchMatrix.Set(newMtrix);

            LimitTransAndScale(touchMatrix, contentRect);

            if (invalidate)
                chart.Invalidate();
            newMtrix.Set(touchMatrix);
            return newMtrix;
        }

        public void LimitTransAndScale(Matrix matrix, RectangleF content)
        {
            matrix.GetValues(MatrixBuffer);

            float curTranX = MatrixBuffer[Matrix.MtransX];
            float cureScaleX = MatrixBuffer[Matrix.MscaleX];

            float curTranY = MatrixBuffer[Matrix.MtransY];
            float curScaleY = MatrixBuffer[Matrix.MscaleY];

            scaleX = MathF.Min(MathF.Max(minScaleX, cureScaleX), maxScaleX);

            scaleY = MathF.Min(MathF.Max(minScaleY, curScaleY), maxScaleY);

            float width, height;
            if (content.IsEmpty)
            {
                width = 0;
                height = 0;
            }
            else
            {
                width = content.Width;
                height = content.Height;
            }
            float maxTransX = -width * (scaleX - 1f);
            transX = MathF.Min(MathF.Max(curTranX, maxTransX - transOffsetX), transOffsetX);

            float maxTransY = height * (scaleY - 1f);
            transY = MathF.Max(MathF.Min(curTranY, maxTransY + transOffsetY), -transOffsetY);
            MatrixBuffer[Matrix.MtransX] = transX;
            MatrixBuffer[Matrix.MscaleX] = scaleX;
            MatrixBuffer[Matrix.MtransY] = transY;
            MatrixBuffer[Matrix.MscaleY] = scaleY;

            matrix.SetValues(MatrixBuffer);
        }
    }
}
