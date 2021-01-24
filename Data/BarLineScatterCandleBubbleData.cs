using System.Collections.Generic;

namespace XF.ChartLibrary.Data
{
    public class BarLineScatterCandleBubbleData<TDataSet, TEntry> : ChartData<TDataSet, TEntry> where TDataSet : Interfaces.DataSets.IBarLineScatterCandleBubbleDataSet<TEntry>
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
