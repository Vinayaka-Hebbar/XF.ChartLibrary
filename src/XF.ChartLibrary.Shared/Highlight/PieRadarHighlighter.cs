using XF.ChartLibrary.Charts;

namespace XF.ChartLibrary.Highlight
{
    public abstract class PieRadarHighlighter<T> : IHighlighter where T : IPieRadarChartBase
    {
        protected T Chart;

        protected PieRadarHighlighter(T chart)
        {
            Chart = chart;
        }

        public Highlight GetHighlight(float x, float y)
        {
            float touchDistanceToCenter = Chart.DistanceToCenter(x, y);

            // check if a slice was touched
            if (touchDistanceToCenter > Chart.Radius)
            {

                // if no slice was touched, highlight nothing
                return null;

            }
            else
            {

                float angle = Chart.GetAngleForPoint(x, y);

                if (Chart is PieChart)
                {
                    angle /= Chart.Animator.PhaseY;
                }

                int index = Chart.GetIndexForAngle(angle);

                // check if the index could be found
                if (index < 0 || index >= Chart.Data.GetMaxEntryCountSet().EntryCount)
                {
                    return null;

                }
                else
                {
                    return GetClosestHighlight(index, x, y);
                }
            }
        }

        /// <summary>
        /// Returns the closest Highlight object of the given objects based on the touch position inside the chart.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        protected abstract Highlight GetClosestHighlight(int index, float x, float y);
    }
}
