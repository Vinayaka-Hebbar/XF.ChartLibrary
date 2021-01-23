using SkiaSharp;

namespace XF.ChartLibrary.Data
{
    public partial class Entry
    {
        public Entry(double x, double y, SKImage icon) : base(y)
        {
            X = x;
            Icon = icon;
        }

        public Entry(double x, double y, SKImage icon, object data) : base(y)
        {
            X = x;
            Icon = icon;
            Data = data;
        }
    }
}
