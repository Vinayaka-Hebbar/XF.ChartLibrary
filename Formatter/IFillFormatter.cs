using XF.ChartLibrary.Data;
using XF.ChartLibrary.Interfaces.DataProvider;

namespace XF.ChartLibrary.Formatter
{
    public interface IFillFormatter
    {
        /// <summary>
        /// The vertical (y-axis) position where the filled-line of the LineDataSet should end.
        /// </summary>
        double GetFillLinePosition(ILineDataSet dataSet, ILineChartDataProvider dataProvider);

    }
}
