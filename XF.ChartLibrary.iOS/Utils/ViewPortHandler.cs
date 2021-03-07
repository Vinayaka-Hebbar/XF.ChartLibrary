using CoreGraphics;
using System;
using XF.ChartLibrary.Charts;

namespace XF.ChartLibrary.Utils
{
    public partial class ViewPortHandler
    {
        internal CGAffineTransform touchMatrix = CGAffineTransform.MakeIdentity();

        private CGRect contentRect = new CGRect();

        public CGRect ContentRect => contentRect;

        public void RestrainViewPort(float offsetLeft, float offsetTop, float offsetRight, float offsetBottom)
        {
            contentRect.Offset(offsetLeft, offsetTop);
            contentRect.Size = new CGSize(chartWidth - offsetRight, chartHeight - offsetBottom);
        }

        public CGAffineTransform Zoom(float scaleX, float scaleY, float x, float y)
        {
            var matrix = touchMatrix;
            matrix.Translate(x, y);
            matrix.Scale(scaleX, scaleY);
            matrix.Translate(-x, -y);
            return matrix;
        }


        /// <summary>
        ///  Resets all zooming and dragging and makes the chart fit exactly it's
        /// bounds.
        /// </summary>
        /// <returns></returns>
        public CGAffineTransform FitScreen()
        {
            minScaleX = 1.0f;
            minScaleY = 1.0f;

            return CGAffineTransform.MakeIdentity();
        }


        /// <summary>
        /// Centers the viewport around the specified position (x-index and y-value) in the chart.
        /// Centering the viewport outside the bounds of the chart is not possible.
        /// Makes most sense in combination with the setScaleMinima(...) method.
        /// </summary>
        public void CenterViewPort(CGPoint pt, IChartBase chart)
        {
            var translateX = pt.X - OffsetLeft;
            var translateY = pt.Y - OffsetTop;
            var matrix = touchMatrix * CGAffineTransform.MakeTranslation(-translateX, -translateY);
            Refresh(matrix, chart: chart, invalidate: true);
        }

        public CGAffineTransform Refresh(CGAffineTransform newMtrix, IChartBase chart, bool invalidate)
        {
            touchMatrix = LimitTransAndScale(newMtrix, contentRect);

            if (invalidate)
            {
                chart.SetNeedsDisplay();
            }

            return touchMatrix;
        }

        public CGAffineTransform LimitTransAndScale(CGAffineTransform matrix, CGRect content)
        {
            scaleX = MathF.Min(MathF.Max(minScaleX, (float)matrix.xx), maxScaleX);

            scaleY = MathF.Min(MathF.Max(minScaleY, (float)matrix.yy), maxScaleY);

            float width, height;
            if (content.IsEmpty)
            {
                width = 0;
                height = 0;
            }
            else
            {
                width = (float)content.Width;
                height = (float)content.Height;
            }
            float maxTransX = -width * (scaleX - 1f);
            transX = MathF.Min(MathF.Max((float)matrix.x0, maxTransX - transOffsetX), transOffsetX);

            float maxTransY = height * (scaleY - 1f);
            transY = MathF.Max(MathF.Min((float)matrix.y0, maxTransY + transOffsetY), -transOffsetY);
            matrix.x0 = transX;
            matrix.xx = scaleX;
            matrix.y0 = transY;
            matrix.yy = scaleY;

            return matrix;
        }
    }
}
