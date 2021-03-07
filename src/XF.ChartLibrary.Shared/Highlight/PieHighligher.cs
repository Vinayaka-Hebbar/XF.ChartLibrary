using XF.ChartLibrary.Charts;
using XF.ChartLibrary.Interfaces.DataSets;

namespace XF.ChartLibrary.Highlight
{
    public class PieHighligher : ChartLibrary.Highlight.PieRadarHighlighter<PieChart>
    {
        public PieHighligher(PieChart chart) : base(chart)
        {
        }

        protected override Highlight GetClosestHighlight(int index, float x, float y)
        {
            IDataSet<Data.PieEntry> set = Chart.Data.DataSet;
            var entry = set[index];
            return new Highlight(index, entry.Y, x, y, 0, set.AxisDependency);
        }
    }
}
