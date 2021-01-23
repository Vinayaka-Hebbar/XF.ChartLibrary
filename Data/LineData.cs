using System.Collections.Generic;

namespace XF.ChartLibrary.Data
{
    public class LineData : BarLineScatterCandleBubbleData<LineDataSet, Entry>
    {
        public LineData()
        {
        }

        public LineData(IList<LineDataSet> sets) : base(sets)
        {
        }
    }
}
