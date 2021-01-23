using System.Text;
using XF.ChartLibrary.Data;
using XF.ChartLibrary.Utils;

namespace XF.ChartLibrary.Formatter
{
    public class DefaultValueFormatter : IValueFormatter
    {
        public static readonly DefaultValueFormatter Instance = new DefaultValueFormatter(1);

        public int Digits { get; }

        public string Format { get; set; }

        public DefaultValueFormatter(int digits)
        {
            Digits = digits;
        }

        public virtual void Setup(int digits)
        {
            StringBuilder b = new StringBuilder("###,###,###,##0");
            for (int i = 0; i < digits; i++)
            {
                if (i == 0)
                    b.Append('.');
                b.Append('0');
            }

            Format = b.ToString();
        }

        public string GetFormattedValue(double value, Entry entry, int dataSetIndex, ViewPortHandler viewPortHandler)
        {
            return value.ToString(Format);
        }
    }
}
