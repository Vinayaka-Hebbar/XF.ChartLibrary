using Android.Graphics.Drawables;

namespace XF.ChartLibrary.Data
{
    public partial class Entry
    {
        public Entry(float x, float y, Drawable icon) : base(y)
        {
            X = x;
            Icon = icon;
        }

        public Entry(float x, float y, Drawable icon, object data) : base(y)
        {
            X = x;
            Icon = icon;
            Data = data;
        }
    }
}
