using System.Collections.Generic;

namespace XF.ChartLibrary.Charts
{
    public interface IChartBase
    {
        IList<Highlight.Highlight> IndicesToHighlight { get; set; }
        bool IsDrawMarkersEnabled { get; set; }
        string NoDataText { get; set; }
        bool ValuesToHighlight { get; }
        Utils.ViewPortHandler ViewPortHandler { get; }
        float ChartWidth { get; }
        float ChartHeight { get; }

        Interfaces.IChartData Data { get; }

        void Clear();
        void NotifyDataSetChanged();
#if SKIASHARP && !NATIVE
        void InvalidateSurface();
#elif __ANDROID__
        void Invalidate();
        bool Post(Java.Lang.IRunnable r);
#elif __IOS__ || __TVOS__
        void SetNeedsDisplay();
#elif WPF
        void InvalidateVisual();
#endif
    }
}