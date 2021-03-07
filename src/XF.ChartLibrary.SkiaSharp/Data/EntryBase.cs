namespace XF.ChartLibrary.Data
{
    public partial class EntryBase
    {
        public EntryBase(float y, Components.IDrawable icon) : this(y)
        {
            Icon = icon;
        }

        public EntryBase(float y, Components.IDrawable icon, object data) : this(y)
        {
            Icon = icon;
            Data = data;
        }

        public Components.IDrawable Icon { get; set; }
    }
}
