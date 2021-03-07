namespace XF.ChartLibrary.Data
{
    public partial class Entry
    {
        public Entry(float x, float y, Components.IDrawable icon) : base(y)
        {
            X = x;
            Icon = icon;
        }

        public Entry(float x, float y, Components.IDrawable icon, object data) : base(y)
        {
            X = x;
            Icon = icon;
            Data = data;
        }
    }
}
