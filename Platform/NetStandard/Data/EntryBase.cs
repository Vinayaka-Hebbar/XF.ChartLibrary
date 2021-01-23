using SkiaSharp;

namespace XF.ChartLibrary.Data
{
    public partial class EntryBase
    {
        public EntryBase(double y, SKImage icon) : this(y)
        {
            Icon = icon;
        }

        public EntryBase(double y, SKImage icon, object data) : this(y)
        {
            Icon = icon;
            Data = data;
        }

        public SKImage Icon { get; set; }
    }
}
