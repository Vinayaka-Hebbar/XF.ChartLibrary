using XF.ChartLibrary.Data;

namespace XF.ChartLibrary.Listener
{
    /// <summary>
    /// Listener for callbacks when selecting values inside the chart by
    /// touch-gesture.
    /// </summary>
    public interface IChartValueSelectionListener
    {
        /// <summary>
        ///  Called when a value has been selected inside the chart.
        /// </summary>
        /// <param name="e">The selected Entry</param>
        /// <param name="h"> The corresponding highlight object that contains information
        /// about the highlighted position such as dataSetIndex, ...</param>
        void OnValueSelected(Entry e, Highlight.Highlight h);

        /// <summary>
        /// Called when nothing has been selected or an "un-select" has been made.
        /// </summary>
        void OnNothingSelected();
    }
}
