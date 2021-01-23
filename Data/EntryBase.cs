using System;

namespace XF.ChartLibrary.Data
{
    public partial class EntryBase
    {
        public EntryBase()
        {
        }

        public EntryBase(double y)
        {
            Y = y;
        }

        public EntryBase(double y, object data) : this(y)
        {
            Data = data;
        }

        public double Y { get; set; }

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
