using XF.ChartLibrary.Interfaces.DataSets;

namespace XF.ChartLibrary.Interfaces.DataProvider
{
    public interface IChartProvider
    {
        IDataSet GetData();

        float YChartMax { get; }

        float YChartMin { get; }

        int MaxVisibleCount { get; }

    }
}
