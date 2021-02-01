using System;
using System.Collections.Generic;
using System.Text;

namespace XF.ChartLibrary.Data
{
    public partial class Entry : EntryBase, System.ICloneable
    {
        public Entry()
        {
        }

        public Entry(float x, float y) : base(y)
        {
            X = x;
        }

        public Entry(float x, float y, object data) : base(y)
        {
            X = x;
            Data = data;
        }

        public float X { get; set; }

        public object Clone()
        {
            return new Entry(X, Y, Data);
        }

        public override bool Equals(object obj)
        {
            return obj is Entry entry &&
                   X == entry.X &&
            Y == entry.Y;
        }

        public override int GetHashCode()
        {
            int hashCode = 458382817;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            return hashCode;
        }

        public override string ToString()
        {
            return $"{nameof(EntryBase)} x:{X}, y: {Y}";
        }

    }
}
