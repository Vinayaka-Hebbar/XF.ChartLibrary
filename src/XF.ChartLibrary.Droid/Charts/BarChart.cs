using Android.Content;

namespace XF.ChartLibrary.Charts
{
    partial class BarChart
    {
        public BarChart(Context context) : base(context)
        {

        }

        private bool fitBars;
        /// <summary>
        ///  Adds half of the bar width to each side of the x-axis range in order to allow the bars of the barchart to be
        /// fully displayed. Default: false
        /// </summary>
        public bool FitBars
        {
            get => fitBars;
            set => fitBars = value;
        }

        private bool isDrawBarShadow;
        public bool IsDrawBarShadow
        {
            get => isDrawBarShadow;
            set => isDrawBarShadow = value;
        }

        private bool isDrawValueAboveBar = true;
        public bool IsDrawValueAboveBar
        {
            get => isDrawValueAboveBar;
            set => isDrawValueAboveBar = value;
        }

        private bool highlightFullBar;
        public bool IsHighlightFullBar
        {
            get => highlightFullBar;
            set => highlightFullBar = value;
        }
    }
}
