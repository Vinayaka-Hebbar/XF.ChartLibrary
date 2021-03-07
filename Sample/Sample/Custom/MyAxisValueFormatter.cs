using XF.ChartLibrary.Components;
using XF.ChartLibrary.Formatter;

namespace Sample.Custom
{
    public class MyAxisValueFormatter : IAxisValueFormatter
    {
        private readonly string format;

        public MyAxisValueFormatter()
        {
            format = "###,###,###,##0.0";
        }

        public string GetFormattedValue(float value, AxisBase axis)
        {
            return value.ToString(format) + " $";
        }
    }
}
