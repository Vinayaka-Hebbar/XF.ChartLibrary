using System.Windows;

namespace XF.ChartLibrary.Charts
{
    public partial class PieChart
    {
        public PieChart()
        {

        }

        public override float RequiredLegendOffset
        {
            get
            {
                return LegendRenderer.LabelPaint.TextSize * 2.0f;
            }
        }

    }
}
