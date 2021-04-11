using SkiaSharp;
using System;
using XF.ChartLibrary.Components;
using XF.ChartLibrary.Utils;

namespace XF.ChartLibrary.Renderer
{
    partial class YAxisRendererRadarChart
    {
        protected override void ComputeAxisValues(float min, float max)
        {
            float yMin = min;
            float yMax = max;

            int labelCount = Axis.LabelCount;
            double range = Math.Abs(yMax - yMin);

            if (labelCount == 0 || range <= 0 || double.IsInfinity(range))
            {
                Axis.entries = Array.Empty<float>();
                Axis.centeredEntries = Array.Empty<float>();
                Axis.entryCount = 0;
                return;
            }

            // Find out how much spacing (in y value space) between axis values
            double rawInterval = range / labelCount;
            double interval = rawInterval.RoundToNextSignificant();

            // If granularity is enabled, then do not allow the interval to go below specified granularity.
            // This is used to avoid repeated values when rounding values for display.
            if (Axis.GranularityEnabled)
                interval = interval < Axis.Granularity ? Axis.Granularity : interval;

            // Normalize interval
            double intervalMagnitude = (Math.Pow(10, (int)Math.Log10(interval))).RoundToNextSignificant();
            int intervalSigDigit = (int)(interval / intervalMagnitude);
            if (intervalSigDigit > 5)
            {
                // Use one order of magnitude higher, to avoid intervals like 0.9 or 90
                // if it's 0.0 after floor(), we use the old value
                interval = Math.Floor(10.0 * intervalMagnitude) == 0.0
                        ? interval
                        : Math.Floor(10.0 * intervalMagnitude);
            }

            var centeringEnabled = Axis.IsCenterAxisLabelsEnabled;
            int n = centeringEnabled ? 1 : 0;

            // force label count
            if (Axis.IsForceLabelsEnabled)
            {

                float step = (float)range / (float)(labelCount - 1);
                Axis.entryCount = labelCount;

                if (Axis.entries.Count < labelCount)
                {
                    // Ensure stops contains at least numStops elements.
                    Axis.entries = new float[labelCount];
                }

                float v = min;

                for (int i = 0; i < labelCount; i++)
                {
                    Axis.entries[i] = v;
                    v += step;
                }

                n = labelCount;

                // no forced count
            }
            else
            {

                double first = interval == 0.0 ? 0.0 : Math.Ceiling(yMin / interval) * interval;
                if (centeringEnabled)
                {
                    first -= interval;
                }

                double last = interval == 0.0 ? 0.0 : NumberExtensions.NextDouble(Math.Floor(yMax / interval) * interval);

                double f;
                int i;

                if (interval != 0.0)
                {
                    for (f = first; f <= last; f += interval)
                    {
                        ++n;
                    }
                }

                n++;

                Axis.entryCount = n;

                if (Axis.entries.Count < n)
                {
                    // Ensure stops contains at least numStops elements.
                    Axis.entries = new float[n];
                }

                for (f = first, i = 0; i < n; f += interval, ++i)
                {

                    if (f == 0.0) // Fix for negative zero case (Where value == -0.0, and 0.0 == -0.0)
                        f = 0.0;

                    Axis.entries[i] = (float)f;
                }
            }

            // set decimals
            if (interval < 1)
            {
                Axis.decimals = (int)Math.Ceiling(-Math.Log10(interval));
            }
            else
            {
                Axis.decimals = 0;
            }

            if (centeringEnabled)
            {

                if (Axis.centeredEntries.Length < n)
                {
                    Axis.centeredEntries = new float[n];
                }

                float offset = (Axis.entries[1] - Axis.entries[0]) / 2f;

                for (int i = 0; i < n; i++)
                {
                    Axis.centeredEntries[i] = Axis.entries[i] + offset;
                }
            }
            Axis.axisMinimum = Axis.entries[0];
            Axis.axisMaximum = Axis.entries[n - 1];
            Axis.axisRange = Math.Abs(Axis.axisMaximum - Axis.axisMinimum);
        }

        public override void RenderAxisLabels(SKCanvas c)
        {
            if (!YAxis.IsEnabled || !YAxis.IsDrawLabelsEnabled)
                return;

            AxisLabelPaint.Typeface = YAxis.Typeface;
            AxisLabelPaint.TextSize = YAxis.TextSize;
            AxisLabelPaint.Color = YAxis.TextColor;

            // calculate the factor that is needed for transforming the value to
            // pixels
            float factor = Chart.Factor;
            var center = Chart.CenterOffsets;

            int from = YAxis.DrawBottomYLabelEntry ? 0 : 1;
            int to = YAxis.DrawTopYLabelEntry
                    ? YAxis.EntryCount
                    : (YAxis.EntryCount - 1);

            float xOffset = YAxis.XLabelOffset;

            for (int j = from; j < to; j++)
            {

                float r = (YAxis.entries[j] - YAxis.axisMinimum) * factor;

                var pOut = ChartUtil.GetPosition(center, r, Chart.RotationAngle);

                var label = YAxis.GetFormattedLabel(j);

                c.DrawText(label, pOut.X + xOffset, pOut.Y, AxisLabelPaint);
            }
        }

        private readonly SKPath RenderLimitLinesPathBuffer = new SKPath();

        public override void RenderLimitLines(SKCanvas c)
        {
            var limitLines = YAxis.LimitLines;

            if (limitLines == null)
                return;

            float sliceangle = Chart.SliceAngle;

            // calculate the factor that is needed for transforming the value to
            // pixels
            float factor = Chart.Factor;

            var center = Chart.CenterOffsets;
            for (int i = 0; i < limitLines.Count; i++)
            {

                LimitLine l = limitLines[i];

                if (!l.IsEnabled)
                    continue;

                LimitLinePaint.Color = l.LineColor;
                LimitLinePaint.PathEffect = l.DashPathEffect;
                LimitLinePaint.StrokeWidth = l.LineWidth;

                float r = (l.Limit - Chart.YChartMin) * factor;

                var limitPath = RenderLimitLinesPathBuffer;
                limitPath.Reset();


                int entryCount = Chart.Data.GetMaxEntryCountSet().EntryCount;
                for (int j = 0; j < entryCount; j++)
                {

                    var pOut = ChartUtil.GetPosition(center, r, sliceangle * j + Chart.RotationAngle);

                    if (j == 0)
                        limitPath.MoveTo(pOut.X, pOut.Y);
                    else
                        limitPath.LineTo(pOut.X, pOut.Y);
                }
                limitPath.Close();

                c.DrawPath(limitPath, LimitLinePaint);
            }
        }
    }
}
