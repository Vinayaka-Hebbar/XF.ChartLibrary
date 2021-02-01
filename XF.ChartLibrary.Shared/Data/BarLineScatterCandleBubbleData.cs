using System.Collections.Generic;
using XF.ChartLibrary.Interfaces;
using XF.ChartLibrary.Interfaces.DataSets;

namespace XF.ChartLibrary.Data
{
    public class BarLineScatterCandleBubbleData<TDataSet, TEntry> : ChartData<TDataSet, TEntry>, IChartData<IBarLineScatterCandleBubbleDataSet> where TDataSet : IBarLineScatterCandleBubbleDataSet<TEntry>
        where TEntry : Entry
    {
        public BarLineScatterCandleBubbleData()
        {
        }

        public BarLineScatterCandleBubbleData(IList<TDataSet> sets) : base(sets)
        {
        }

        IBarLineScatterCandleBubbleDataSet IChartData<IBarLineScatterCandleBubbleDataSet>.this[int index] => base[index];

        IList<IBarLineScatterCandleBubbleDataSet> IChartData<IBarLineScatterCandleBubbleDataSet>.DataSets => (IList<IBarLineScatterCandleBubbleDataSet>)DataSets;

        bool IChartData<IBarLineScatterCandleBubbleDataSet>.Contains(IBarLineScatterCandleBubbleDataSet dataSet)
        {
            return DataSets.Contains((TDataSet)dataSet);
        }

        IBarLineScatterCandleBubbleDataSet IChartData<IBarLineScatterCandleBubbleDataSet>.GetMaxEntryCountSet()
        {
            return GetMaxEntryCountSet();
        }
    }
}
