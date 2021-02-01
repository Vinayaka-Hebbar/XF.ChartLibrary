using XF.ChartLibrary.Components;

namespace XF.ChartLibrary.Charts
{
    partial class BarLineChartBase<TData, TDataSet>
    {

        protected BarLineChartBase()
        {
            AxisLeft = new YAxis(YAxisDependency.Left);
            AxisRight = new YAxis(YAxisDependency.Right);
        }

        public override int MaxVisibleCount { get; set; } = 100;

        /// <summary>
        /// the object representing the labels on the left y-axis
        /// </summary>
        public YAxis AxisLeft { get; set; }

        /// <summary>
        /// the object representing the labels on the right y-axis
        /// </summary>
        public YAxis AxisRight { get; set; }
    }
}
