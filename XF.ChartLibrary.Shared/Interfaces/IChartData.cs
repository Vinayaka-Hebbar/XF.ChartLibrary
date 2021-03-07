using System.Collections.Generic;
using XF.ChartLibrary.Data;

namespace XF.ChartLibrary.Interfaces
{
    public interface IChartData<TDataSet> : IChartDataBase where TDataSet : DataSets.IDataSet
    {
        IList<TDataSet> DataSets { get; }

        TDataSet this[int index] { get; }

        bool Contains(TDataSet dataSet);

        TDataSet GetMaxEntryCountSet();
    }

    public interface IChartData : IChartDataBase
    { 
        DataSets.IDataSet this[int index] { get; }

        DataSets.IDataSet GetMaxEntryCountSet();
    }

    public interface IChartDataBase
    {
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

        int EntryCount { get; }

        int DataSetCount { get; }

        /// <summary>
        /// Get the Entry for a corresponding highlight object
        /// </summary>
        /// <returns>the entry that is highlighted</returns>
        Entry GetEntryForHighlight(Highlight.Highlight highlight);

        void ClearValues();

        /// <summary>
        /// Returns the minimum y-value for the specified axis.
        /// </summary>
        float GetYMin(Components.YAxisDependency axis);

        /// <summary>
        /// Returns the maximum y-value for the specified axis.
        /// </summary>
        float GetYMax(Components.YAxisDependency axis);

        /// <summary>
        ///  Calc minimum and maximum y-values over all DataSets.
        /// Tell DataSets to recalculate their min and max y-values, this is only needed for autoScaleMinMax.
        /// </summary>
        /// <param name="fromX">the x-value to start the calculation from</param>
        /// <param name="toX">the x-value to which the calculation should be performed</param>
        void CalcMinMaxY(float fromX, float toX);
    }
}