using System.Collections.Generic;
using XF.ChartLibrary.Interfaces.DataSets;

namespace XF.ChartLibrary.Data
{
    public class BarData : BarLineScatterCandleBubbleData<IBarDataSet, BarEntry>
    {
        /// <summary>
        /// the width of the bars on the x-axis, in values (not pixels)
        /// </summary>
        private float barWidth = 0.85f;

        public BarData()
        {

        }

        public BarData(IList<IBarDataSet> dataSets) : base(dataSets)
        {

        }

        /// <summary>
        /// the width each bar should have on the x-axis (in values, not pixels).
        /// Default 0.85f
        /// </summary>
        public float BarWidth
        {
            get => barWidth;
            set => barWidth = value;
        }

        /// <summary>
        /// Groups all BarDataSet objects this data object holds together by modifying the x-value of their entries.
        /// Previously set x-values of entries will be overwritten.Leaves space between bars and groups as specified
        /// by the parameters.
        /// Do not forget to call notifyDataSetChanged() on your BarChart object after calling this method.
        /// </summary>
        /// <param name="fromX">the starting point on the x-axis where the grouping should begin</param>
        /// <param name="groupSpace">the space between groups of bars in values (not pixels) e.g. 0.8f for bar width 1f</param>
        /// <param name="barSpace">the space between individual bars in values (not pixels) e.g. 0.1f for bar width 1f</param>
        public void GroupBars(float fromX, float groupSpace, float barSpace)
        {
            int setCount = dataSets.Count;
            if (setCount <= 1)
            {
                throw new System.InvalidOperationException("BarData needs to hold at least 2 BarDataSets to allow grouping.");
            }

            IBarDataSet max = GetMaxEntryCountSet();
            int maxEntryCount = max.EntryCount;

            float groupSpaceWidthHalf = groupSpace / 2f;
            float barSpaceHalf = barSpace / 2f;
            float barWidthHalf = barWidth / 2f;

            float interval = GetGroupWidth(groupSpace, barSpace);

            for (int i = 0; i < maxEntryCount; i++)
            {

                float start = fromX;
                fromX += groupSpaceWidthHalf;

                foreach (IBarDataSet set in dataSets)
                {

                    fromX += barSpaceHalf;
                    fromX += barWidthHalf;

                    if (i < set.EntryCount)
                    {
                        BarEntry entry = ((IDataSet<BarEntry>)set)[i];
                        if (entry != null)
                        {
                            entry.X = fromX;
                        }
                    }

                    fromX += barWidthHalf;
                    fromX += barSpaceHalf;
                }

                fromX += groupSpaceWidthHalf;
                float end = fromX;
                float innerInterval = end - start;
                float diff = interval - innerInterval;

                // correct rounding errors
                if (diff > 0 || diff < 0)
                {
                    fromX += diff;
                }
            }

            NotifyDataChanged();
        }

        /// <summary>
        /// In case of grouped bars, this method returns the space an individual group of bar needs on the x-axis.
        /// </summary>
        /// <param name="groupSpace"></param>
        /// <param name="barSpace"></param>
        /// <returns></returns>
        public float GetGroupWidth(float groupSpace, float barSpace)
        {
            return dataSets.Count * (barWidth + barSpace) + groupSpace;
        }
    }
}
