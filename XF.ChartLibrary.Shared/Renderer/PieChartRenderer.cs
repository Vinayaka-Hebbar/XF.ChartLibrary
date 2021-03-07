using System;
using XF.ChartLibrary.Charts;
using XF.ChartLibrary.Interfaces.DataSets;
using XF.ChartLibrary.Utils;
#if NETSTANDARD || SKIASHARP
using Point = SkiaSharp.SKPoint;
#elif __IOS__ || __TVOS__
using Point = CoreGraphics.CGPoint;
#elif __ANDROID__
using Point = Android.Graphics.PointF;
#endif

namespace XF.ChartLibrary.Renderer
{
    public partial class PieChartRenderer : DataRenderer
    {
        protected PieChart Chart;

        public PieChartRenderer(PieChart chart, Animation.Animator animator, ViewPortHandler viewPortHandler) : base(animator, viewPortHandler)
        {
            Chart = chart;
        }

        protected float CalculateMinimumRadiusForSpacedSlice(
            Point center,
            float radius,
            float angle,
            float arcStartPointX,
            float arcStartPointY,
            float startAngle,
            float sweepAngle)
        {
            float angleMiddle = startAngle + sweepAngle / 2.0f;

            // Other point of the arc
            float arcEndPointX = center.X + radius * (float)Math.Cos((startAngle + sweepAngle) * ChartUtil.FDegToRad);
            float arcEndPointY = center.Y + radius * (float)Math.Sin((startAngle + sweepAngle) * ChartUtil.FDegToRad);

            // Middle point on the arc
            float arcMidPointX = center.X + radius * (float)Math.Cos(angleMiddle * ChartUtil.FDegToRad);
            float arcMidPointY = center.Y + radius * (float)Math.Sin(angleMiddle * ChartUtil.FDegToRad);

            // This is the base of the contained triangle
            double basePointsDistance = Math.Sqrt(
                    Math.Pow(arcEndPointX - arcStartPointX, 2) +
                            Math.Pow(arcEndPointY - arcStartPointY, 2));

            // After reducing space from both sides of the "slice",
            //   the angle of the contained triangle should stay the same.
            // So let's find out the height of that triangle.
            float containedTriangleHeight = (float)(basePointsDistance / 2.0 *
                    Math.Tan((180.0 - angle) / 2.0 * ChartUtil.DegToRad));

            // Now we subtract that from the radius
            float spacedRadius = radius - containedTriangleHeight;

            // And now subtract the height of the arc that's between the triangle and the outer circle
            spacedRadius -= MathF.Sqrt(
                    MathF.Pow(arcMidPointX - (arcEndPointX + arcStartPointX) / 2.0f, 2) +
                            MathF.Pow(arcMidPointY - (arcEndPointY + arcStartPointY) / 2.0f, 2));

            return spacedRadius;
        }

        /// <summary>
        /// Calculates the sliceSpace to use based on visible values and their size compared to the set sliceSpace.
        /// </summary>
        /// <param name="dataSet"></param>
        /// <returns></returns>
        protected float GetSliceSpace(IPieDataSet dataSet)
        {
            if (!dataSet.IsAutomaticallyDisableSliceSpacing)
                return dataSet.SliceSpace;

            float spaceSizeRatio = dataSet.SliceSpace / ViewPortHandler.SmallestContentExtension;
            float minValueRatio = dataSet.YMin / Chart.Data.YValueSum * 2;

            return spaceSizeRatio > minValueRatio ? 0f : dataSet.SliceSpace;
        }

        public override void InitBuffers()
        {
            //  TODO Auto-generated method stub
        }

    }
}