using Android.Content;
using XF.ChartLibrary.Components;

namespace XF.ChartLibrary.Charts
{
    partial class BarLineChartBase<TData, TDataSet>
    {
        /// <summary>
        /// flag that indicates if pinch-zoom is enabled. if true, both x and y axis
        /// can be scaled with 2 fingers, if false, x and y axis can be scaled
        /// separately
        /// </summary>
        protected bool mPinchZoomEnabled = false;

        /// <summary>
        ///  flag that indicates if double tap zoom is enabled or not
        /// </summary>
        protected bool mDoubleTapToZoomEnabled = true;

        /// <summary>
        /// flag that indicates if highlighting per dragging over a fully zoomed out
        /// chart is enabled
        /// </summary>
        protected bool mHighlightPerDragEnabled = true;

        /// <summary>
        /// if true, dragging is enabled for the chart
        /// </summary>
        private bool mDragXEnabled = true;
        private bool mDragYEnabled = true;

        private bool mScaleXEnabled = true;
        private bool mScaleYEnabled = true;

        protected BarLineChartBase(Context context) : base(context)
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
