using XF.ChartLibrary.Data;

namespace XF.ChartLibrary.Interfaces.DataProvider
{
    public interface IBarDataProvider : IBarLineScatterCandleBubbleDataProvider
    {
        BarData BarData { get; }
        bool IsDrawBarShadow { get; }
        bool IsDrawValueAboveBar { get; }
        bool IsHighlightFullBar{ get; }
    }
}
