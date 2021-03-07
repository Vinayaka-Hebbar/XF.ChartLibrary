namespace XF.ChartLibrary.Data
{
    public partial class PieEntry : Entry
    {
        private string label;

        public PieEntry(float value) : base(0f, value)
        {
        }

        public PieEntry(float value, object data) : base(0f, value, data)
        {
        }

        public PieEntry(float value, string label):base(0, value)
        {
            this.label = label;
        }

        public PieEntry(float value, string label, object data) : base(0f, value, data)
        {
            this.label = label;
        }

        public string Label
        {
            get => label;
            set => label = value;
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Obsolete("Pie entries do not have x values")]
#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member
        public override float X { get => base.X; set => base.X = value; }
#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member

        /// <summary>
        /// This is the same as getY(). Returns the value of the PieEntry.
        /// </summary>
        public float Value => y;


        public override Entry Clone()
        {
            return new PieEntry(Y, Label, Data);
        }
    }
}
