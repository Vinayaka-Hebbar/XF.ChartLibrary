namespace XF.ChartLibrary.Data
{
    public interface ILineScatterCandleRadarDataSet<TEntry> : IBarLineScatterCandleBubbleDataSet<TEntry> where TEntry : Entry
    {
        bool DrawHorizontalHighlightIndicator { get; }
        bool DrawVerticalHighlightIndicatorEnabled { get; }
    }
}