﻿using System;

namespace XF.ChartLibrary.Data
{
    public partial class Entry : EntryBase, ICloneable
    {
        private float x;

        public Entry()
        {
        }

        public Entry(float x, float y) : base(y)
        {
            this.x = x;
        }

        public Entry(float x, float y, object data) : base(y)
        {
            this.x = x;
            Data = data;
        }

        public virtual float X
        {
            get => x;
            set => x = value;
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        public virtual Entry Clone()
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
