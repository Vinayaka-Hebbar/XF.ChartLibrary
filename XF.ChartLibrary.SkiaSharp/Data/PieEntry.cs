namespace XF.ChartLibrary.Data
{
    partial class PieEntry
    {
        public PieEntry(float value, Components.IDrawable icon) : base(0f, value, icon)
        {
            ;
        }

        public PieEntry(float value, Components.IDrawable icon, object data) : base(0f, value, icon, data)
        {
        }

        public PieEntry(float value, string label, Components.IDrawable icon) : base(0f, value, icon)
        {
            this.label = label;
        }

        public PieEntry(float value, string label, Components.IDrawable icon, object data) : base(0f, value, icon, data)
        {
            this.label = label;
        }
    }
}
