using System;
using System.Collections.Generic;
using System.Text;
using XF.ChartLibrary.Interfaces.DataSets;

namespace XF.ChartLibrary.Data
{
    public abstract class DataSet<TEntry> : DataSetBase<TEntry>, IDataSet where TEntry : Entry
    {
        private IList<TEntry> entries;

        public override TEntry this[int i] => entries == null ? default : entries[i];

        Entry IDataSet.this[int i] => entries == null ? default : entries[i];

        /// <summary>
        ///  Creates a new DataSet object with the given values (entries) it represents. Also, a
        ///label that describes the DataSet can be specified.The label can also be
        /// used to retrieve the DataSet from a ChartData object.
        /// </summary>
        public DataSet(IList<TEntry> entries, string label) : base(label)
        {
            if (entries == null)
                entries = new List<TEntry>();

            this.entries = entries;
            CalcMinMax();
        }

        public override void CalcMinMax()
        {
            yMax = -float.MaxValue;
            yMin = float.MaxValue;
            xMax = -float.MaxValue;
            xMin = float.MaxValue;

            if (entries == null || entries.Count == 0)
                return;

            foreach (var e in entries)
            {
                CalcMinMax(e);
            }
        }

        public override int EntryCount => entries.Count;

        public IList<TEntry> Entries
        {
            get
            {
                return entries;
            }
            set
            {
                entries = value;
                NotifyDataSetChanged();
            }
        }

        /// <summary>
        /// Updates the min and max x and y value of this DataSet based on the given Entry.
        /// </summary>
        protected virtual void CalcMinMax(TEntry e)
        {
            if (e == null)
                return;

            CalcMinMaxX(e);

            CalcMinMaxY(e);
        }

        protected void CalcMinMaxX(TEntry e)
        {
            if (e.X < xMin)
                xMin = e.X;

            if (e.X > xMax)
                xMax = e.X;
        }

        protected void CalcMinMaxY(TEntry e)
        {
            if (e.Y < yMin)
                yMin = e.Y;

            if (e.Y > yMax)
                yMax = e.Y;
        }

        public override void CalcMinMaxY(float fromX, float toX)
        {
            yMax = -float.MaxValue;
            yMin = float.MaxValue;

            if (entries == null || entries.Count == 0)
                return;

            int indexFrom = EntryIndex(fromX, float.NaN, rounding: DataSetRounding.Down);
            int indexTo = EntryIndex(toX, float.NaN, DataSetRounding.Up);

            if (indexTo < indexFrom)
                return;

            for (int i = indexFrom; i <= indexTo; i++)
            {

                // only recalculate y
                CalcMinMaxY(entries[i]);
            }
        }

        public override bool AddEntryOrdered(TEntry e)
        {
            if (e == null)
                return false;

            if (entries == null)
            {
                entries = new List<TEntry>();
            }

            CalcMinMax(e);

            int count = entries.Count;
            if (count > 0 && entries[count - 1].X > e.X)
            {
                int closestIndex = EntryIndex(e.X, e.Y, DataSetRounding.Up);
                if (closestIndex > -1)
                {
                    entries.Insert(closestIndex, e);
                }
            }
            else
            {
                entries.Add(e);
            }
            return true;
        }


        public override void Clear()
        {
            entries.Clear();
            NotifyDataSetChanged();
        }

        public override bool AddEntry(TEntry e)
        {
            if (e == null)
                return false;

            IList<TEntry> values = Entries;
            if (values == null)
            {
                return false;
            }

            CalcMinMax(e);

            // add the entry
            values.Add(e);
            return true;
        }

        bool IDataSet.Contains(Entry e)
        {
            if (entries == null)
                return false;
            int entryCount = EntryCount;
            for (int i = 0; i < entryCount; i++)
            {
                if (entries[i].Equals(e))
                    return true;
            }

            return false;
        }

        public override bool Contains(TEntry e)
        {
            if (entries == null)
                return false;
            return entries.Contains(e);
        }

        public override bool RemoveEntry(int index)
        {
            if (entries == null || index < entries.Count)
                return false;

            // remove the entry
            entries.RemoveAt(index);
            return true;
        }

        int IDataSet.EntryIndex(Entry e)
        {
            if (entries != null)
            {
                int count = entries.Count;
                for (int i = 0; i < count; i++)
                {
                    if (entries[i].Equals(e))
                        return i;
                }
            }

            return -1;
        }

        public override int EntryIndex(TEntry e)
        {
            if (entries != null)
            {
                return entries.IndexOf(e);
            }

            return -1;
        }

        public override IList<TEntry> EntriesForXValue(float xValue)
        {
            List<TEntry> entries = new List<TEntry>();

            int low = 0;
            int high = this.entries.Count - 1;

            while (low <= high)
            {
                int m = (high + low) / 2;
                TEntry entry = this.entries[m];

                // if we have a match
                if (xValue == entry.X)
                {
                    while (m > 0 && this.entries[m - 1].X == xValue)
                        m--;

                    high = this.entries.Count;

                    // loop over all "equal" entries
                    for (; m < high; m++)
                    {
                        entry = this.entries[m];
                        if (entry.X == xValue)
                        {
                            entries.Add(entry);
                        }
                        else
                        {
                            break;
                        }
                    }

                    break;
                }
                else
                {
                    if (xValue > entry.X)
                        low = m + 1;
                    else
                        high = m - 1;
                }
            }

            return entries;
        }

        IList<Entry> IDataSet.EntriesForXValue(double xValue)
        {
            List<Entry> entries = new List<Entry>();

            int low = 0;
            int high = this.entries.Count - 1;

            while (low <= high)
            {
                int m = (high + low) / 2;
                Entry entry = this.entries[m];

                // if we have a match
                if (xValue == entry.X)
                {
                    while (m > 0 && this.entries[m - 1].X == xValue)
                        m--;

                    high = this.entries.Count;

                    // loop over all "equal" entries
                    for (; m < high; m++)
                    {
                        entry = this.entries[m];
                        if (entry.X == xValue)
                        {
                            entries.Add(entry);
                        }
                        else
                        {
                            break;
                        }
                    }

                    break;
                }
                else
                {
                    if (xValue > entry.X)
                        low = m + 1;
                    else
                        high = m - 1;
                }
            }

            return entries;
        }

        public override TEntry EntryForXValue(float xValue, float yValue, DataSetRounding rounding)
        {
            int index = EntryIndex(xValue, yValue, rounding);
            if (index > -1)
                return entries[index];
            return default;
        }


        Entry IDataSet.EntryForXValue(float xValue, float yValue, DataSetRounding rounding)
        {
            int index = EntryIndex(xValue, yValue, rounding);
            if (index > -1)
                return entries[index];
            return default;
        }

        Entry IDataSet.EntryForXValue(float xValue, float yValue)
        {
            return EntryForXValue(xValue, yValue, DataSetRounding.Closest);
        }

        public override int EntryIndex(float xValue, float yValue, DataSetRounding rounding)
        {
            if (entries == null || entries.Count == 0)
                return -1;

            int low = 0;
            int high = entries.Count - 1;
            int closest = high;

            while (low < high)
            {
                int m = (low + high) / 2;

                double d1 = entries[m].X - xValue,
                        d2 = entries[m + 1].X - xValue,
                        ad1 = Math.Abs(d1), ad2 = Math.Abs(d2);

                if (ad2 < ad1)
                {
                    // [m + 1] is closer to xValue
                    // Search in an higher place
                    low = m + 1;
                }
                else if (ad1 < ad2)
                {
                    // [m] is closer to xValue
                    // Search in a lower place
                    high = m;
                }
                else
                {
                    // We have multiple sequential x-value with same distance

                    if (d1 >= 0.0)
                    {
                        // Search in a lower place
                        high = m;
                    }
                    else if (d1 < 0.0)
                    {
                        // Search in an higher place
                        low = m + 1;
                    }
                }

                closest = high;
            }

            if (closest > -1)
            {
                var closestXValue = entries[closest].X;
                if (rounding == DataSetRounding.Up)
                {
                    // If rounding up, and found x-value is lower than specified x, and we can go upper...
                    if (closestXValue < xValue && closest < entries.Count - 1)
                    {
                        ++closest;
                    }
                }
                else if (rounding == DataSetRounding.Down)
                {
                    // If rounding down, and found x-value is upper than specified x, and we can go lower...
                    if (closestXValue > xValue && closest > 0)
                    {
                        --closest;
                    }
                }

                // Search by closest to y-value
                if (!double.IsNaN(yValue))
                {
                    while (closest > 0 && entries[closest - 1].X == closestXValue)
                        closest -= 1;

                    var closestYValue = entries[closest].Y;
                    int closestYIndex = closest;

                    while (true)
                    {
                        closest += 1;
                        if (closest >= entries.Count)
                            break;

                        var value = entries[closest];

                        if (value.X != closestXValue)
                            break;

                        if (Math.Abs(value.Y - yValue) <= Math.Abs(closestYValue - yValue))
                        {
                            closestYValue = yValue;
                            closestYIndex = closest;
                        }
                    }

                    closest = closestYIndex;
                }
            }

            return closest;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            ToSimpleString(builder);
            foreach (TEntry e in entries)
            {
                builder.Append(e);
            }
            return builder.ToString();
        }

        public virtual void ToSimpleString(StringBuilder builder)
        {
            builder.AppendLine("DataSet, label: " + (Label ?? string.Empty) + ", entries: " + entries.Count);
        }

    }
}
