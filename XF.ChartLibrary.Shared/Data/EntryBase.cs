namespace XF.ChartLibrary.Data
{
    public partial class EntryBase
    {
        internal float y;
        private object data;

        public EntryBase()
        {
        }

        public EntryBase(float y)
        {
            this.y = y;
        }

        public EntryBase(float y, object data) : this(y)
        {
            this.data = data;
        }

        public float Y
        {
            get => y;
            set => y = value;
        }

        public object Data
        {
            get => data;
            set => data = value;
        }

        public override string ToString()
        {
            return $"{nameof(EntryBase)} y: {y}";
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
