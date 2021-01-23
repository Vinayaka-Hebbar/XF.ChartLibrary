using Android.Graphics.Drawables;

namespace XF.ChartLibrary.Data
{
    public partial class EntryBase
    {
        public EntryBase(double y, Drawable icon) : this(y)
        {
            Icon = icon;
        }

        public EntryBase(double y, Drawable icon, object data) : this(y)
        {
            Icon = icon;
            Data = data;
        }

        public Drawable Icon { get; set; }
    }
}
