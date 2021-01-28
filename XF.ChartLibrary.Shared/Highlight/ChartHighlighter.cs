using System;
using System.Collections.Generic;
using XF.ChartLibrary.Components;
using XF.ChartLibrary.Data;
using XF.ChartLibrary.Interfaces.DataSets;
#if __IOS__ || __TVOS__
using Point = CoreGraphics.CGPoint;
#elif __ANDROID__
using Point = Android.Graphics.PointF;
#elif NETSTANDARD
using Point = SkiaSharp.SKPoint;
#endif
namespace XF.ChartLibrary.Highlight
{
    public class ChartHighlighter<T> : IHighlighter where T : Interfaces.DataProvider.IBarLineScatterCandleBubbleDataProvider
    {
        protected readonly T Chart;

        protected readonly List<Highlight> HighlightBuffer = new List<Highlight>();

        public ChartHighlighter(T chart)
        {
            Chart = chart;
        }

        public Highlight GetHighlight(float x, float y)
        {
            var pos = GetValsForTouch(x, y);
            return GetHighlightForX((float)pos.X, x, y);
        }

        /// <summary>
        /// Returns the corresponding xPos for a given touch-position in pixels.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>a <see cref="Point"/> instance.</returns>
        protected Point GetValsForTouch(float x, float y)
        {
            // take any transformer to determine the x-axis value
            return Chart
                .GetTransformer(YAxisDependency.Left)
                .ValueByTouchPoint(x, y);
        }

        /// <summary>
        /// Returns the corresponding Highlight for a given xVal and x- and y-touch position in pixels.
        /// </summary>
        protected Highlight GetHighlightForX(float xVal, float x, float y)
        {

            IList<Highlight> closestValues = GetHighlightsAtXValue(xVal, x, y);

            if (closestValues.Count == 0)
            {
                return null;
            }

            float leftAxisMinDist = GetMinimumDistance(closestValues, y, YAxisDependency.Left);
            float rightAxisMinDist = GetMinimumDistance(closestValues, y, YAxisDependency.Right);

            var axis = leftAxisMinDist < rightAxisMinDist ? YAxisDependency.Left : YAxisDependency.Right;

            return GetClosestHighlightByPixel(closestValues, x, y, axis, Chart.MaxHighlightDistance);
        }

        /// <summary>
        /// Returns the minimum distance from a touch value (in pixels) to the
        /// closest value(in pixels) that is displayed in the chart.
        /// </summary>
        protected float GetMinimumDistance(IList<Highlight> closestValues, float pos, YAxisDependency axis)
        {

            float distance = float.MaxValue;

            for (int i = 0; i < closestValues.Count; i++)
            {

                Highlight high = closestValues[i];

                if (high.Axis == axis)
                {

                    float tempDistance = MathF.Abs(GetHighlightPos(high) - pos);
                    if (tempDistance < distance)
                    {
                        distance = tempDistance;
                    }
                }
            }

            return distance;
        }

        protected float GetHighlightPos(Highlight h)
        {
            return h.YPx;
        }

        /// <summary>
        ///  Returns a list of Highlight objects representing the entries closest to the given xVal.
        /// The returned list contains two objects per DataSet (closest rounding up, closest rounding down).
        /// </summary>
        /// <param name="xVal">the transformed x-value of the x-touch position</param>
        /// <param name="x">touch position</param>
        /// <param name="y">touch position</param>
        protected IList<Highlight> GetHighlightsAtXValue(float xVal, float x, float y)
        {

            HighlightBuffer.Clear();

            var data = ((Interfaces.DataProvider.IBarLineScatterCandleBubbleProvider)Chart).Data;

            if (data == null)
                return HighlightBuffer;
            var dataSets = data.DataSets;
            for (int i = 0, dataSetCount = dataSets.Count; i < dataSetCount; i++)
            {

                IDataSet dataSet = data[i];

                // don't include DataSets that cannot be highlighted
                if (!dataSet.IsHighlightEnabled)
                    continue;

                HighlightBuffer.AddRange(BuildHighlights(dataSet, i, xVal, DataSetRounding.Closest));
            }

            return HighlightBuffer;
        }

        /// <summary>
        /// An array of `Highlight` objects corresponding to the selected xValue and dataSetIndex.
        /// </summary>
        protected IList<Highlight> BuildHighlights(IDataSet set, int dataSetIndex, float xVal, DataSetRounding rounding)
        {

            IList<Highlight> highlights = new List<Highlight>();

            //noinspection unchecked
            IList<Entry> entries = set.EntriesForXValue(xVal);
            if (entries.Count == 0)
            {
                // Try to find closest x-value and take all entries for that x-value
                Entry closest = set.EntryForXValue(xVal, float.NaN, rounding);
                if (closest != null)
                {
                    //noinspection unchecked
                    entries = set.EntriesForXValue(closest.X);
                }
            }

            if (entries.Count == 0)
                return highlights;

            foreach (Entry e in entries)
            {
                var pixels = Chart.GetTransformer(
                        set.AxisDependency).PointValueToPixel(e.X, e.Y);

                highlights.Add(new Highlight(
                        e.X, e.Y,
                        (float)pixels.X, (float)pixels.Y,
                        dataSetIndex, set.AxisDependency));
            }

            return highlights;
        }

        /// <summary>
        /// Returns the Highlight of the DataSet that contains the closest value on the
        /// y-axis.
        /// </summary>
        /// <param name="closestValues">contains two Highlight objects per DataSet closest to the selected x-position(determined by
        ///                             rounding up an down)</param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="axis"> the closest axis</param>
        /// <param name="minSelectionDistance"></param>
        /// <returns></returns>
        public Highlight GetClosestHighlightByPixel(IList<Highlight> closestValues, float x, float y,
                                                    YAxisDependency axis, float minSelectionDistance)
        {
            Highlight closest = null;
            float distance = minSelectionDistance;

            for (int i = 0; i < closestValues.Count; i++)
            {

                Highlight high = closestValues[i];

                if (high.Axis == axis)
                {

                    float cDistance = GetDistance(x, y, high.XPx, high.YPx);

                    if (cDistance < distance)
                    {
                        closest = high;
                        distance = cDistance;
                    }
                }
            }

            return closest;
        }

        /// <summary>
        /// Calculates the distance between the two given points.
        /// </summary>
        protected float GetDistance(float x1, float y1, float x2, float y2)
        {
            //return Math.abs(y1 - y2);
            //return Math.abs(x1 - x2);
            return ChartUtil.Hypot(x1 - x2, y1 - y2);
        }

        protected Interfaces.IChartData<IBarLineScatterCandleBubbleDataSet> Data
        {
            get
            {
                return ((Interfaces.DataProvider.IBarLineScatterCandleBubbleProvider)Chart).Data;
            }
        }
    }

}