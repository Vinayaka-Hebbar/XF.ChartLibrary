using XF.ChartLibrary.Components;
using XF.ChartLibrary.Data;
using XF.ChartLibrary.Utils;

namespace XF.ChartLibrary.Formatter
{
    /// <summary>
    ///  This IValueFormatter is just for convenience and simply puts a "%" sign after
    /// each value. (Recommeded for PieChart)
    /// </summary>
    public class PercentFormatter : IValueFormatter, IAxisValueFormatter
    {
        protected string Format;

        public PercentFormatter()
        {
            Format = "###,###,##0.0";
        }

        /// <summary>
        /// Allow a custom decimalformat
        /// </summary>
        /// <param name="format"></param>
        public PercentFormatter(string format)
        {
            this.Format = format;
        }

        public int DecimalDigits => 1;

        // IValueFormatter
        public string GetFormattedValue(float value, Entry entry, int dataSetIndex, ViewPortHandler viewPortHandler)
        {
            return value.ToString(Format) + " %";
        }

        // 
        // IAxisValueFormatter
        public string GetFormattedValue(float value, AxisBase axis)
        {
            return value.ToString(Format) + " %";
        }
    }
}
