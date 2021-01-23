using XF.ChartLibrary.Data;

namespace XF.ChartLibrary.Interfaces.DataProvider
{
    public interface IChartProvider
    {
        IChartData<TDataSet> GetData<TDataSet>() where TDataSet : IDataSet;

        IDataSet GetData();

        double YChartMax { get; }

        double YChartMin { get; }

        int MaxVisibleCount { get; }

    }
}
