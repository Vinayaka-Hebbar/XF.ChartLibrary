using System.Collections.Generic;

namespace Sample.Models
{
    public class ChartTypes : List<ChartType>
    {
        public ChartTypes(string header)
        {
            Header = header;
        }

        public string Header { get; }

        public override string ToString()
        {
            return Header;
        }
    }
}
