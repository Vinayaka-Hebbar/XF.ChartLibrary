using XF.ChartLibrary.Components;
using XF.ChartLibrary.Utils;

namespace XF.ChartLibrary.Interfaces.DataProvider
{
    public interface IBarLineScatterCandleBubbleDataProvider : IChartDataProvider, IBarLineScatterCandleBubbleProvider
    {
    }

    public interface IBarLineScatterCandleBubbleProvider
    {
        IChartData Data { get; }

        Transformer GetTransformer(YAxisDependency axis);
        bool IsInverted(YAxisDependency axis);

        float LowestVisibleX { get; }
        float HighestVisibleX { get; }
    }
}
