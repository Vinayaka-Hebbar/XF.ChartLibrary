using SkiaSharp;
using System;
using XF.ChartLibrary.Charts;

namespace XF.ChartLibrary.Utils
{
    public partial class ViewPortHandler
    {
        internal SKMatrix touchMatrix = SKMatrix.CreateIdentity();

        private SKRect contentRect = new SKRect();

        public SKMatrix TouchMatrix => touchMatrix;

        public SKRect ContentRect => contentRect;

        public void RestrainViewPort(float offsetLeft, float offsetTop, float offsetRight, float offsetBottom)
        {
            contentRect.Left = offsetLeft;
            contentRect.Top = offsetTop;
            contentRect.Right = chartWidth - offsetRight;
            contentRect.Bottom = chartHeight - offsetBottom;
        }

        public SKMatrix Zoom(float scaleX, float scaleY, float x, float y)
        {
            return touchMatrix.PostConcat(SKMatrix.CreateScale(scaleX, scaleY, x, y));
        }

        public SKMatrix Zoom(float scaleX, float scaleY)
        {
            return SKMatrix.Concat(touchMatrix, SKMatrix.CreateScale(scaleX, scaleY));
        }

        /// <summary>
        /// Resets all zooming and dragging and makes the chart fit exactly it's
        /// bounds.
        /// </summary>
        /// <returns></returns>
        public SKMatrix FitScreen()
        {
            minScaleX = 1.0f;
            minScaleY = 1.0f;

            return SKMatrix.CreateIdentity();
        }


        /// <summary>
        ///  Centers the viewport around the specified position(x-index and y-value)
        /// in the chart.Centering the viewport outside the bounds of the chart is
        /// not possible. Makes most sense in combination with the
        /// setScaleMinima(...) method.
        /// </summary>
        /// <param name="transformedPts">the position to center view viewport to</param>
        /// <param name="view"></param>
        public void CenterViewPort(SKPoint transformedPts, IChartBase view)
        {
            float x = transformedPts.X - OffsetLeft;
            float y = transformedPts.Y - OffsetTop;

            Refresh(touchMatrix.PostConcat(SKMatrix.CreateTranslation(-x, -y)), view, true);
        }

        public SKMatrix Refresh(SKMatrix newMtrix, IChartBase chart, bool invalidate)
        {
            touchMatrix = LimitTransAndScale(newMtrix, contentRect);

            if (invalidate)
            {
#if WPF
                chart.InvalidateVisual();
#else
                chart.InvalidateSurface();
#endif
            }

            return touchMatrix;
        }

        public SKMatrix LimitTransAndScale(SKMatrix matrix, SKRect content)
        {
            scaleX = Math.Min(Math.Max(minScaleX, matrix.ScaleX), maxScaleX);

            scaleY = Math.Min(Math.Max(minScaleY, matrix.ScaleY), maxScaleY);

            var width = content.Width;
            var height = content.Height;
            float maxTransX = -width * (scaleX - 1f);
            transX = Math.Min(Math.Max(matrix.TransX, maxTransX - transOffsetX), transOffsetX);

            float maxTransY = height * (scaleY - 1f);
            transY = Math.Max(Math.Min(matrix.TransY, maxTransY + transOffsetY), -transOffsetY);
            matrix.TransX = transX;
            matrix.ScaleX = scaleX;
            matrix.TransY = transY;
            matrix.ScaleY = scaleY;

            return matrix;
        }
    }
}
