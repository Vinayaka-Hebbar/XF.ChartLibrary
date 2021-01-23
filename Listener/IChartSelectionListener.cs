using XF.ChartLibrary.Data;

namespace XF.ChartLibrary.Listener
{
    /// <summary>
    /// Listener for callbacks when selecting values inside the chart by
    /// touch-gesture.
    /// </summary>
    public interface IChartSelectionListener
    {
        /**
     * Called when a value has been selected inside the chart.
     *
     * @param e The selected Entry
     * @param h The corresponding highlight object that contains information
     *          about the highlighted position such as dataSetIndex, ...
     */
        void OnValueSelected(Entry e, Highlight.Highlight h);

        /**
         * Called when nothing has been selected or an "un-select" has been made.
         */
        void OnNothingSelected();
    }
}
