using System.Collections.Generic;
using XF.ChartLibrary.Interfaces;
using XF.ChartLibrary.Interfaces.DataSets;

namespace XF.ChartLibrary.Data
{
    public class BarLineScatterCandleBubbleData<TDataSet, TEntry> : ChartData<TDataSet, TEntry>, IChartData<TDataSet> where TDataSet : IBarLineScatterCandleBubbleDataSet<TEntry>
        where TEntry : Entry
    {
        public BarLineScatterCandleBubbleData()
        {
        }

        public BarLineScatterCandleBubbleData(IList<TDataSet> sets) : base(sets)
        {
        }
    }
}
