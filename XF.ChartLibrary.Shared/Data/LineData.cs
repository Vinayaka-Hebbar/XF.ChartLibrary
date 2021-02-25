using System.Collections.Generic;
using XF.ChartLibrary.Interfaces.DataSets;

namespace XF.ChartLibrary.Data
{
    public class LineData : BarLineScatterCandleBubbleData<ILineDataSet, Entry>, Interfaces.IChartData<ILineDataSet>
    {
        public LineData()
        {
        }

        public LineData(IList<ILineDataSet> sets) : base(sets)
        {
        }
    }
}
