namespace XF.ChartLibrary.Data
{
    public partial class EntryBase
    {
        public EntryBase()
        {
        }

        public EntryBase(float y)
        {
            Y = y;
        }

        public EntryBase(float y, object data) : this(y)
        {
            Data = data;
        }

        public float Y { get; set; }

        public object Data { get; set; }

        public override string ToString()
        {
            return $"{nameof(EntryBase)} y: {Y}";
        }

        public override bool Equals(object obj)
        {
            return obj is EntryBase other &&
                   Y == other.Y;
        }

        public override int GetHashCode()
        {
            return Y.GetHashCode();
        }
    }
}
