using System.Collections.Generic;
using XF.ChartLibrary.Data;

namespace XF.ChartLibrary.Interfaces
{
    public interface IChartData<TDataSet> where TDataSet : DataSets.IDataSet
    {
        void NotifyDataSetChanged();
        /// <summary>
        /// maximum y-value in the value array across all axes
        /// </summary>
        float YMax { get; }

        /// <summary>
        /// the minimum y-value in the value array across all axes
        /// </summary>
        float YMin { get; }

        /// <summary>
        /// maximum x-value in the value array
        /// </summary>
        float XMax { get; }

        /// <summary>
        /// minimum x-value in the value array
        /// </summary>
        float XMin { get; }

        IList<TDataSet> DataSets { get; }

        TDataSet this[int index] { get; }

        /// <summary>
        /// Get the Entry for a corresponding highlight object
        /// </summary>
        /// <returns>the entry that is highlighted</returns>
        public Entry GetEntryForHighlight(Highlight.Highlight highlight);

        void ClearValues();

        bool Contains(TDataSet dataSet);

        TDataSet GetMaxEntryCountSet();
    }
}