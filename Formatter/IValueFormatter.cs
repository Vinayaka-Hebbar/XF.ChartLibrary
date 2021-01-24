using XF.ChartLibrary.Utils;

namespace XF.ChartLibrary.Formatter
{
    public interface IValueFormatter
    {
        string GetFormattedValue(float value, Data.Entry entry, int dataSetIndex, ViewPortHandler viewPortHandler);
    }
}
