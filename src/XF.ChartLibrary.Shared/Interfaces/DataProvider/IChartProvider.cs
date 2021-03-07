namespace XF.ChartLibrary.Interfaces.DataProvider
{
    public interface IChartProvider
    {
        float YChartMax { get; }

        float YChartMin { get; }

        int MaxVisibleCount { get; }

        float MaxHighlightDistance { get; }

    }

    public interface IChartDataProvider : IChartProvider
    {
        IChartData Data { get; }
    }
}
