using System;
using XF.ChartLibrary.Animation;
using XF.ChartLibrary.Data;
using XF.ChartLibrary.Interfaces.DataProvider;
using XF.ChartLibrary.Interfaces.DataSets;
using XF.ChartLibrary.Utils;

namespace XF.ChartLibrary.Renderer
{
    public abstract class BarLineScatterCandleBubbleRenderer : DataRenderer
    {
        /**
         * buffer for storing the current minimum and maximum visible x
         */
        protected readonly Bounds XBounds = new Bounds();

        public BarLineScatterCandleBubbleRenderer(Animator animator, ViewPortHandler viewPortHandler) : base(animator, viewPortHandler)
        {
        }

        /// <summary>
        ///  Returns true if the DataSet values should be drawn, false if not.
        /// </summary>
        protected bool ShouldDrawValues(IDataSet set)
        {
            return set.IsVisible && (set.IsDrawValuesEnabled || set.IsDrawIconsEnabled);
        }

        /// <summary>
        /// Checks if the provided entry object is in bounds for drawing considering the current animation phase.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="set"></param>
        /// <returns></returns>
        protected bool IsInBoundsX(Entry e, IBarLineScatterCandleBubbleDataSet set)
        {

            if (e == null)
                return false;

            float entryIndex = set.EntryIndex(e);

            return e != null && entryIndex < set.EntryCount * Animator.PhaseX;
        }

        public class Bounds
        {
            public int Min;

            public int Max;

            public int Range;

            internal Bounds()
            {

            }

            public Bounds(IBarLineScatterCandleBubbleDataProvider chart, IBarLineScatterCandleBubbleDataSet dataSet, Animator animator)
            {
                Set(chart, dataSet, animator);
            }

            public void Set(IBarLineScatterCandleBubbleDataProvider chart, IBarLineScatterCandleBubbleDataSet dataSet, Animator animator)
            {
                var phaseX = Math.Max(0.0, Math.Min(1.0, animator.PhaseX));

                var low = chart.LowestVisibleX;
                var high = chart.HighestVisibleX;

                var entryFrom = dataSet.EntryForXValue(low, float.NaN, DataSetRounding.Down);
                Entry entryTo = dataSet.EntryForXValue(high, float.NaN, DataSetRounding.Up);

                Min = entryFrom == null ? 0 : dataSet.EntryIndex(entryFrom);
                Max = entryTo == null ? 0 : dataSet.EntryIndex(entryTo);
                Range = (int)((Max - Min) * phaseX);
            }
        }
    }


}
