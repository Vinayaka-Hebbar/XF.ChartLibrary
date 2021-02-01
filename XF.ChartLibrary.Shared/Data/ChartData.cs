using System;
using System.Collections.Generic;
using XF.ChartLibrary.Interfaces;
using XF.ChartLibrary.Interfaces.DataSets;

#if NETSTANDARD || SKIASHARP
using Color = SkiaSharp.SKColor;
using Font = SkiaSharp.SKTypeface;
#elif __IOS__ || __TVOS
using Color = UIKit.UIColor;
using Font = UIKit.UIFont;
#elif __ANDROID__
using Color = Android.Graphics.Color;
using Font = Android.Graphics.Typeface;
#endif

namespace XF.ChartLibrary.Data
{
    public abstract class ChartData<TDataSet, TEntry> : IChartData, IChartData<TDataSet> where TDataSet : IDataSet<TEntry>, IDataSet where TEntry : Entry
    {
        protected float LeftAxisMin = float.MaxValue;

        protected float LeftAxisMax = -float.MaxValue;

        protected float RightAxisMax = -float.MaxValue;

        protected float RightAxisMin = float.MaxValue;

        internal float yMax = -float.MaxValue;

        internal float yMin = float.MaxValue;

        internal float xMax = -float.MaxValue;

        internal float xMin = float.MaxValue;

        private IList<TDataSet> dataSets;

        public IList<TDataSet> DataSets => dataSets;

        public float XMin { get => xMin; }

        public float XMax { get => xMax; }

        public float YMin { get => yMin; }

        public float YMax { get => yMax; }

        /// <summary>
        /// Enables / disables highlighting values for all DataSets this data object
        /// contains.If set to true, this means that values can
        /// be highlighted programmatically or by touch gesture.
        /// </summary>
        /// <param name="enabled"></param>
        public bool SetHighlightEnabled
        {
            get
            {
                if (dataSets == null)
                    return false;
                foreach (var set in dataSets)
                {
                    if (!set.IsHighlightEnabled)
                        return false;
                }
                return true;
            }
            set
            {
                if (dataSets == null)
                    return;
                foreach (var set in dataSets)
                {
                    set.IsHighlightEnabled = value;
                }
            }
        }

        /// <summary>
        /// returns the number of LineDataSets this object contains
        /// </summary>
        public int DataSetCount
        {
            get
            {
                if (dataSets == null)
                    return 0;
                return dataSets.Count;
            }
        }


        public ChartData()
        {
            dataSets = new List<TDataSet>();
        }

        /// <summary>
        ///constructor for chart data
        /// </summary>
        /// <param name="sets">the dataset array</param>
        public ChartData(IList<TDataSet> sets)
        {
            dataSets = sets;
            NotifyDataChanged();
        }

        /// <summary>
        /// Call this method to let the ChartData know that the underlying data has
        /// changed.Calling this performs all necessary recalculations needed when
        /// the contained data has changed.
        /// </summary>
        public void NotifyDataChanged()
        {
            CalcMinMax();
        }

        /// <summary>
        ///  Calc minimum and maximum y-values over all DataSets.
        /// Tell DataSets to recalculate their min and max y-values, this is only needed for autoScaleMinMax.
        /// </summary>
        /// <param name="fromX">the x-value to start the calculation from</param>
        /// <param name="toX">the x-value to which the calculation should be performed</param>
        public void CalcMinMaxY(float fromX, float toX)
        {

            foreach (TDataSet set in DataSets)
            {
                set.CalcMinMaxY(fromX, toX);
            }

            // apply the new data
            CalcMinMax();
        }

        /// <summary>
        ///  Calc minimum and maximum values (both x and y) over all DataSets.
        /// </summary>
        protected void CalcMinMax()
        {

            if (dataSets == null)
                return;

            yMax = -float.MaxValue;
            yMin = float.MaxValue;
            xMax = -float.MaxValue;
            xMin = float.MaxValue;

            foreach (TDataSet set in dataSets)
            {
                CalcMinMax(set);
            }

            LeftAxisMax = -float.MaxValue;
            LeftAxisMin = float.MaxValue;
            RightAxisMax = -float.MaxValue;
            RightAxisMin = float.MaxValue;

            // left axis
            var firstLeft = GetFirstLeft(dataSets);

            if (firstLeft != null)
            {

                LeftAxisMax = firstLeft.YMax;
                LeftAxisMin = firstLeft.YMin;

                foreach (TDataSet dataSet in dataSets)
                {
                    if (dataSet.AxisDependency == Components.YAxisDependency.Left)
                    {
                        if (dataSet.YMin < LeftAxisMin)
                            LeftAxisMin = dataSet.YMin;

                        if (dataSet.YMax > LeftAxisMax)
                            LeftAxisMax = dataSet.YMax;
                    }
                }
            }

            // right axis
            var firstRight = GetFirstRight(dataSets);

            if (firstRight != null)
            {

                RightAxisMax = firstRight.YMax;
                RightAxisMin = firstRight.YMin;

                foreach (TDataSet dataSet in dataSets)
                {
                    if (dataSet.AxisDependency == Components.YAxisDependency.Right)
                    {
                        if (dataSet.YMin < RightAxisMin)
                            RightAxisMin = dataSet.YMin;

                        if (dataSet.YMax > RightAxisMax)
                            RightAxisMax = dataSet.YMax;
                    }
                }
            }
        }

        /// <summary>
        /// Returns the first DataSet from the datasets-array that has it's dependency on the right axis.
        /// Returns null if no DataSet with right dependency could be found.
        /// </summary>
        public TDataSet GetFirstRight(IList<TDataSet> sets)
        {
            foreach (TDataSet dataSet in sets)
            {
                if (dataSet.AxisDependency == Components.YAxisDependency.Right)
                    return dataSet;
            }
            return default;
        }

        /// <summary>
        /// Returns the first DataSet from the datasets-array that has it's dependency on the left axis.
        /// Returns null if no DataSet with left dependency could be found.
        /// </summary>
        protected TDataSet GetFirstLeft(IList<TDataSet> sets)
        {
            foreach (TDataSet dataSet in sets)
            {
                if (dataSet.AxisDependency == Components.YAxisDependency.Left)
                    return dataSet;
            }
            return default;
        }

        /// <summary>
        /// Adjusts the minimum and maximum values based on the given DataSet.
        /// </summary>
        /// <param name="d"></param>
        protected void CalcMinMax(TDataSet d)
        {

            if (yMax < d.YMax)
                yMax = d.YMax;
            if (yMin > d.YMin)
                yMin = d.YMin;

            if (xMax < d.XMax)
                xMax = d.XMax;
            if (xMin > d.XMin)
                xMin = d.XMin;

            if (d.AxisDependency == Components.YAxisDependency.Left)
            {

                if (LeftAxisMax < d.YMax)
                    LeftAxisMax = d.YMax;
                if (LeftAxisMin > d.YMin)
                    LeftAxisMin = d.YMin;
            }
            else
            {
                if (RightAxisMax < d.YMax)
                    RightAxisMax = d.YMax;
                if (RightAxisMin > d.YMin)
                    RightAxisMin = d.YMin;
            }
        }


        /// <summary>
        /// Adjusts the current minimum and maximum values based on the provided Entry object.
        /// </summary>
        protected void CalcMinMax(Entry e, Components.YAxisDependency axis)
        {
            if (yMax < e.Y)
                yMax = e.Y;
            if (yMin > e.Y)
                yMin = e.Y;

            if (xMax < e.X)
                xMax = e.X;
            if (xMin > e.X)
                xMin = e.X;

            if (axis == Components.YAxisDependency.Left)
            {

                if (LeftAxisMax < e.Y)
                    LeftAxisMax = e.Y;
                if (LeftAxisMin > e.Y)
                    LeftAxisMin = e.Y;
            }
            else
            {
                if (RightAxisMax < e.Y)
                    RightAxisMax = e.Y;
                if (RightAxisMin > e.Y)
                    RightAxisMin = e.Y;
            }
        }

        /// <summary>
        ///  Retrieve the index of a DataSet with a specific label from the ChartData.
        /// Search can be case sensitive or not.IMPORTANT: This method does
        /// calculations at runtime, do not over-use in performance critical
        /// situations.
        /// </summary>
        /// <param name="dataSets">DataSets</param>
        /// <param name="label">the DataSet array to search</param>
        /// <param name="ignorecase">if true, the search is not case-sensitive</param>
        /// <returns></returns>
        protected static int GetDataSetIndexByLabel(IList<TDataSet> dataSets, string label,
                                             bool ignorecase)
        {
            if (ignorecase)
            {
                for (int i = 0; i < dataSets.Count; i++)
                    if (label.Equals(dataSets[i].Label, StringComparison.OrdinalIgnoreCase))
                        return i;
            }
            else
            {
                for (int i = 0; i < dataSets.Count; i++)
                    if (label.Equals(dataSets[i].Label))
                        return i;
            }

            return -1;
        }

        /// <summary>
        /// Returns the labels of all DataSets as a string array.
        /// </summary>
        /// <returns></returns>
        public string[] GetDataSetLabels()
        {
            if (dataSets == null)
                return Array.Empty<string>();
            string[] types = new string[dataSets.Count];

            for (int i = 0; i < dataSets.Count; i++)
            {
                types[i] = dataSets[i].Label;
            }

            return types;
        }

        /// <summary>
        /// Get the Entry for a corresponding highlight object
        /// </summary>
        /// <returns> the entry that is highlighted</returns>
        public Entry GetEntryForHighlight(Highlight.Highlight highlight)
        {
            if (dataSets == null)
                return null;
            if (highlight.DataSetIndex >= dataSets.Count)
                return null;
            else
            {
                IDataSet<TEntry> dataSet = dataSets[highlight.DataSetIndex];
                return dataSet.EntryForXValue(highlight.X, highlight.Y);
            }
        }

        /// <summary>
        /// Returns the DataSet object with the given label. Search can be case
        /// sensitive or not.IMPORTANT: This method does calculations at runtime.
        /// Use with care in performance critical situations.
        public TDataSet GetDataSetByLabel(string label, bool ignorecase)
        {
            if (dataSets == null)
                return default;
            int index = GetDataSetIndexByLabel(dataSets, label, ignorecase);

            if (index < 0 || index >= dataSets.Count)
                return default;
            else
                return dataSets[index];
        }

        public TDataSet GetDataSetByIndex(int index)
        {
            if (dataSets == null || index < 0 || index >= dataSets.Count)
                return default;

            return dataSets[index];
        }

        /// <summary>
        /// Adds a DataSet dynamically.
        /// </summary>
        /// <param name="d"></param>
        public void AddDataSet(TDataSet d)
        {
            if (d == null || dataSets == null)
                return;

            CalcMinMax(d);

            dataSets.Add(d);
        }

        /// <summary>
        /// Removes the given DataSet from this data object. Also recalculates all
        /// minimum and maximum values.Returns true if a DataSet was removed, false
        /// if no DataSet could be removed.
        /// </summary>
        public bool RemoveDataSet(TDataSet d)
        {

            if (d == null || dataSets == null)
                return false;

            bool removed = dataSets.Remove(d);

            // if a DataSet was removed
            if (removed)
            {
                NotifyDataChanged();
            }

            return removed;
        }


        /// <summary>
        /// Returns all colors used across all DataSet objects this object
        /// represents.
        /// </summary>
        public Color[] Colors
        {
            get
            {
                if (dataSets == null)
                    return null;

                int clrcnt = 0;

                foreach (TDataSet set in dataSets)
                {
                    clrcnt += set.Colors.Count;
                }

                Color[] colors = new Color[clrcnt];
                int cnt = 0;

                foreach (TDataSet set in DataSets)
                {
                    foreach (Color clr in set.Colors)
                    {
                        colors[cnt] = clr;
                        cnt++;
                    }
                }

                return colors;
            }
        }

        /**
         * Returns the index of the provided DataSet in the DataSet array of this data object, or -1 if it does not exist.
         *
         * @param dataSet
         * @return
         */
        public int GetIndexOfDataSet(TDataSet dataSet)
        {
            return DataSets.IndexOf(dataSet);
        }

        /// <summary>
        /// Returns the minimum y-value for the specified axis.
        /// </summary>
        public float GetYMin(Components.YAxisDependency axis)
        {
            if (axis == Components.YAxisDependency.Left)
            {
                return LeftAxisMin == float.MaxValue ? RightAxisMin : LeftAxisMin;
            }
            else
            {
                return RightAxisMin == float.MaxValue ? LeftAxisMin : RightAxisMin;
            }
        }


        /// <summary>
        /// Returns the maximum y-value for the specified axis.
        /// </summary>
        public float GetYMax(Components.YAxisDependency axis)
        {
            if (axis == Components.YAxisDependency.Left)
            {

                return LeftAxisMax == -float.MaxValue ? RightAxisMax : LeftAxisMax;
            }
            else
            {
                return RightAxisMax == -float.MaxValue ? LeftAxisMax : RightAxisMax;
            }
        }

        /// <summary>
        /// Adds an Entry to the DataSet at the specified index.
        /// Entries are added to the end of the list.
        /// </summary>
        public void AddEntry(TEntry e, int dataSetIndex)
        {
            if (dataSets == null)
                return;
            if (dataSets.Count > dataSetIndex && dataSetIndex >= 0)
            {

                var set = dataSets[dataSetIndex];
                // add the entry to the dataset
                if (!set.AddEntry(e))
                    return;

                CalcMinMax(e, set.AxisDependency);

            }
            else
            {
                System.Diagnostics.Trace.TraceError("Cannot add Entry because dataSetIndex too high or too low.");
            }
        }

        /// <summary>
        /// Removes the given Entry object from the DataSet at the specified index.
        /// </summary>
        public bool RemoveEntry(TEntry e, int dataSetIndex)
        {
            if (dataSets == null)
                return false;
            // entry null, outofbounds
            if (e == null || dataSetIndex >= dataSets.Count)
                return false;

            var set = dataSets[dataSetIndex];

            if (set != null)
            {
                // remove the entry from the dataset
                bool removed = set.RemoveEntry(e);

                if (removed)
                {
                    NotifyDataChanged();
                }

                return removed;
            }
            else
                return false;
        }

        /// <summary>
        /// Removes the Entry object closest to the given DataSet at the
        /// specified index.Returns true if an Entry was removed, false if no Entry
        ///was found that meets the specified requirements.
        /// </summary>
        public bool RemoveEntry(float xValue, int dataSetIndex)
        {
            if (dataSets == null)
                return false;
            if (dataSetIndex >= dataSets.Count)
                return false;

            IDataSet<TEntry> dataSet = dataSets[dataSetIndex];
            TEntry e = dataSet.EntryForXValue(xValue, float.NaN);

            if (e == null)
                return false;

            return RemoveEntry(e, dataSetIndex);
        }

        /// <summary>
        /// Clears this data object from all DataSets and removes all Entries. Don't
        /// forget to invalidate the chart after this.
        /// </summary>
        public void ClearValues()
        {
            if (dataSets != null)
            {
                dataSets.Clear();
            }
            NotifyDataChanged();
        }

        /// <summary>
        /// Checks if this data object contains the specified DataSet. Returns true
        /// if so, false if not.
        /// </summary>
        /// <param name="dataSet"></param>
        /// <returns></returns>
        public bool Contains(TDataSet dataSet)
        {
            if (dataSets != null)
            {
                foreach (TDataSet set in dataSets)
                {
                    if (set.Equals(dataSet))
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns the total entry count across all DataSet objects this data object contains.
        /// </summary>
        public int EntryCount
        {
            get
            {
                int count = 0;
                if (dataSets != null)
                {
                    foreach (TDataSet set in dataSets)
                    {
                        count += set.EntryCount;
                    }
                }

                return count;
            }
        }

        public TDataSet this[int index] => dataSets[index];

        IDataSet IChartData.this[int index] => dataSets[index];

        /// <summary>
        /// Returns the DataSet object with the maximum number of entries or null if there are no DataSets.
        /// </summary>
        public TDataSet GetMaxEntryCountSet()
        {
            if (dataSets == null || dataSets.Count == 0)
                return default;

            var max = dataSets[0];

            foreach (TDataSet set in dataSets)
            {
                if (set.EntryCount > max.EntryCount)
                    max = set;
            }

            return max;
        }

        public void NotifyDataSetChanged()
        {
            throw new NotImplementedException();
        }
    }
}
