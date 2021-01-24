using XF.ChartLibrary.Interfaces.DataProvider;
using XF.ChartLibrary.Interfaces.DataSets;

namespace XF.ChartLibrary.Formatter
{
    public interface IFillFormatter
    {
        /// <summary>
        /// The vertical (y-axis) position where the filled-line of the LineDataSet should end.
        /// </summary>
        float GetFillLinePosition(ILineDataSet dataSet, ILineChartDataProvider dataProvider);

    }
}
