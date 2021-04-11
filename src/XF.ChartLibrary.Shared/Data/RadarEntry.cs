namespace XF.ChartLibrary.Data
{
    public class RadarEntry : Entry
    {
        public RadarEntry(float value) : base(0f, value)
        {
        }

        public RadarEntry(float value, object data) : base(0f, value, data)
        {
        }


        public float Value => Y;

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public override float X { get => base.X; set => base.X = value; }

        public override Entry Clone()
        {
            return new RadarEntry(Y, Data);
        }
    }
}
