using System;
using XF.ChartLibrary.Components;
using XF.ChartLibrary.Utils;

namespace XF.ChartLibrary.Renderer
{
    public partial class AxisRenderer : ChartRenderer
    {
        public readonly Transformer Trasformer;

        public readonly AxisBase Axis;

        public AxisRenderer(ViewPortHandler viewPortHandler, AxisBase axis, Transformer trans) : base(viewPortHandler)
        {
            Axis = axis;
            Trasformer = trans;
        }

        /// <summary>
        ///  Computes the axis values.
        /// </summary>
        /// <param name="min">the minimum value in the data object for this axis</param>
        /// <param name="max">he maximum value in the data object for this axis</param>
        /// <param name="inverted"></param>
        public virtual void ComputeAxis(float min, float max, bool inverted)
        {
            // calculate the starting and entry point of the y-labels (depending on
            // zoom / contentrect bounds)
            if (ViewPortHandler.ContentWidth > 10 && !ViewPortHandler.IsFullyZoomedOutY)
            {

                var p1 = Trasformer.ValueByTouchPoint(ViewPortHandler.ContentLeft, ViewPortHandler.ContentTop);
                var p2 = Trasformer.ValueByTouchPoint(ViewPortHandler.ContentLeft, ViewPortHandler.ContentBottom);

                if (!inverted)
                {
                    min = (float)p2.Y;
                    max = (float)p1.Y;
                }
                else
                {

                    min = (float)p1.Y;
                    max = (float)p2.Y;
                }
            }

            ComputeAxisValues(min, max);
        }

        /// <summary>
        /// Sets up the axis values. Computes the desired number of labels between the two given extremes.
        /// </summary>
        protected virtual void ComputeAxisValues(float min, float max)
        {
            float yMin = min;
            float yMax = max;

            int labelCount = Axis.LabelCount;
            var range = Math.Abs(yMax - yMin);

            if (labelCount == 0 || range <= 0 || float.IsInfinity(range))
            {
                Axis.entries = Array.Empty<float>();
                Axis.centeredEntries = Array.Empty<float>();
                Axis.entryCount = 0;
                return;
            }

            // Find out how much spacing (in y value space) between axis values
            double rawInterval = range / labelCount;
            var interval = rawInterval.RoundToNextSignificant();

            // If granularity is enabled, then do not allow the interval to go below specified granularity.
            // This is used to avoid repeated values when rounding values for display.
            if (Axis.GranularityEnabled)
                interval = interval < Axis.Granularity ? Axis.Granularity : interval;

            // Normalize interval
            var intervalMagnitude = Math.Pow(10, (int)MathF.Log10(interval)).RoundToNextSignificant();
            int intervalSigDigit = (int)(interval / intervalMagnitude);
            if (intervalSigDigit > 5)
            {
                // Use one order of magnitude higher, to avoid intervals like 0.9 or 90
                // if it's 0.0 after floor(), we use the old value
                interval = MathF.Floor(10.0f * intervalMagnitude) == 0.0
                        ? interval
                        : MathF.Floor(10.0f * intervalMagnitude);

            }

            int n = Axis.IsCenterAxisLabelsEnabled ? 1 : 0;

            // force label count
            if (Axis.IsForceLabelsEnabled)
            {

                interval = (float)range / (float)(labelCount - 1);
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
                    v += interval;
                }

                n = labelCount;

                // no forced count
            }
            else
            {

                var first = interval == 0.0 ? 0.0 : Math.Ceiling(yMin / interval) * interval;
                if (Axis.IsCenterAxisLabelsEnabled)
                {
                    first -= interval;
                }

                var last = interval == 0.0 ? 0.0 : (Math.Floor(yMax / interval) * interval).NextDouble();

                double f;
                int i;

                if (interval != 0.0 && last != first)
                {
                    for (f = first; f <= last; f += interval)
                    {
                        ++n;
                    }
                }
                else if (last == first && n == 0)
                {
                    n = 1;
                }

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
                Axis.Decimals = (int)Math.Ceiling(-Math.Log10(interval));
            }
            else
            {
                Axis.Decimals = 0;
            }

            if (Axis.IsCenterAxisLabelsEnabled)
            {

                if (Axis.centeredEntries.Length < n)
                {
                    Axis.centeredEntries = new float[n];
                }

                float offset = (float)interval / 2f;

                for (int i = 0; i < n; i++)
                {
                    Axis.centeredEntries[i] = Axis.entries[i] + offset;
                }
            }
        }
    }
}
