using SkiaSharp;

namespace XF.ChartLibrary.Data
{
    public partial class Entry
    {
        public Entry(float x, float y, SKImage icon) : base(y)
        {
            X = x;
            Icon = icon;
        }

        public Entry(float x, float y, SKImage icon, object data) : base(y)
        {
            X = x;
            Icon = icon;
            Data = data;
        }
    }
}
