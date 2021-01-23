using XF.ChartLibrary.Components;

namespace XF.ChartLibrary.Formatter
{
    public class DefaultAxisValueFormatter : IAxisValueFormatter
    {
        public DefaultAxisValueFormatter(int decimals)
        {
            Decimals = decimals;
        }

        public int Decimals { get; set; }

        public string GetFormattedValue(double value, AxisBase axis)
        {
            return value.ToString();
        }
    }
}
