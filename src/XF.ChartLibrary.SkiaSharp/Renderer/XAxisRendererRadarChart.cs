using SkiaSharp;

namespace XF.ChartLibrary.Renderer
{
    partial class XAxisRendererRadarChart
    {
        static readonly SKPoint LabelAnchor = new SKPoint(0.5f, 0.25f);

        public override void RenderAxisLabels(SKCanvas c)
        {
            if (!XAxis.IsEnabled || !XAxis.IsDrawLabelsEnabled)
                return;

            float labelRotationAngleDegrees = XAxis.LabelRotationAngle;
            var drawLabelAnchor = LabelAnchor;

            AxisLabelPaint.Typeface = XAxis.Typeface;
            AxisLabelPaint.TextSize = XAxis.TextSize;
            AxisLabelPaint.Color = XAxis.TextColor;

            float sliceangle = Chart.SliceAngle;

            // calculate the factor that is needed for transforming the value to
            // pixels
            float factor = Chart.Factor;

            var center = Chart.CenterOffsets;
            int entryCount = Chart.Data.GetMaxEntryCountSet().EntryCount;
            for (int i = 0; i < entryCount; i++)
            {

                var label = XAxis.ValueFormatter.GetFormattedValue(i, XAxis);

                float angle = (sliceangle * i + Chart.RotationAngle) % 360f;

                SKPoint pOut = ChartUtil.GetPosition(center, Chart.YRange * factor
                        + XAxis.LabelRotatedWidth / 2f, angle);

                DrawLabel(c, label, pOut.X, pOut.Y - XAxis.LabelRotatedHeight / 2.0f,
                        drawLabelAnchor, labelRotationAngleDegrees);
            }
        }

        /// <summary>
        ///  XAxis LimitLines on RadarChart not yet supported.
        /// </summary>
        public override void RenderLimitLines(SKCanvas c)
        {
        }
    }
}
