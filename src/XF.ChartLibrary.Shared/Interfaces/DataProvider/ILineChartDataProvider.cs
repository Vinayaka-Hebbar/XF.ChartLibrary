using XF.ChartLibrary.Components;
using XF.ChartLibrary.Data;

namespace XF.ChartLibrary.Interfaces.DataProvider
{
    public interface ILineChartDataProvider : IBarLineScatterCandleBubbleDataProvider, ILineChartProvider
    {
        
    }

    public interface ILineChartProvider
    {
        LineData Data { get; }

        YAxis GetAxis(YAxisDependency dependency);
    }
}
