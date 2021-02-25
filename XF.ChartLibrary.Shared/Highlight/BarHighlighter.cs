using System;
using System.Collections.Generic;
using XF.ChartLibrary.Data;
using XF.ChartLibrary.Interfaces;
using XF.ChartLibrary.Interfaces.DataProvider;
using XF.ChartLibrary.Interfaces.DataSets;

namespace XF.ChartLibrary.Highlight
{
    public class BarHighlighter : ChartHighlighter<IBarDataProvider>
    {
        public BarHighlighter(IBarDataProvider chart) : base(chart)
        {
        }

        public override Highlight GetHighlight(float x, float y)
        {
            Highlight high = base.GetHighlight(x, y);

            if (high == null)
            {
                return null;
            }

            var pos = GetValsForTouch(x, y);

            var barData = Chart.BarData;

            var set = barData[high.DataSetIndex];
            if (set.IsStacked)
            {

                return GetStackedHighlight(high,
                        set,
                        (float)pos.X,
                        (float)pos.Y);
            }

            return high;
        }

        /// <summary>
        /// This method creates the Highlight object that also indicates which value of a stacked BarEntry has been
        /// selected.
        /// </summary>
        /// <param name="high">the Highlight to work with looking for stacked values</param>
        /// <param name="dataSet"></param>
        /// <param name="xVal"></param>
        /// <param name="yVal"></param>
        /// <returns></returns>
        public Highlight GetStackedHighlight(Highlight high, IDataSet<BarEntry> dataSet, float xVal, float yVal)
        {
            BarEntry entry = dataSet.EntryForXValue(xVal, yVal);

            if (entry == null)
                return null;

            // not stacked
            if (entry.YVals == null)
            {
                return high;
            }
            else
            {
                var ranges = entry.Ranges;

                if (ranges.Count > 0)
                {
                    int stackIndex = GetClosestStackIndex(ranges, yVal);

                    var pixels = Chart.GetTransformer(dataSet.AxisDependency).PointValueToPixel(high.X, ranges[stackIndex].To);

                    Highlight stackedHigh = new Highlight(
                            entry.X,
                            entry.Y,
                            (float)pixels.X,
                            (float)pixels.Y,
                            high.DataSetIndex,
                            stackIndex,
                            high.Axis
                    );

                    return stackedHigh;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the index of the closest value inside the values array / ranges(stacked barchart) to the value
        /// given as
        /// </summary>
        /// <param name="ranges"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected int GetClosestStackIndex(IList<Range> ranges, float value)
        {
            if (ranges == null || ranges.Count == 0)
                return 0;

            int stackIndex = 0;

            foreach (Range range in ranges)
            {
                if (range.Contains(value))
                    return stackIndex;
                else
                    stackIndex++;
            }

            int length = Math.Max(ranges.Count - 1, 0);

            return (value > ranges[length].To) ? length : 0;
        }

        protected override float GetDistance(float x1, float y1, float x2, float y2)
        {
            return Math.Abs(x1 - x2);
        }

        protected override IChartData Data => Chart.BarData;
    }
}
