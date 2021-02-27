using SkiaSharp;
using System;
using XF.ChartLibrary.Charts;
using XF.ChartLibrary.Data;

namespace XF.ChartLibrary.Components
{
    partial class MarkerView : IMarker
    {
        public void Draw(SKCanvas canvas, SKPoint pos, IChartBase chart)
        {
            throw new NotImplementedException();
        }

        public virtual void RefreshContent(Entry e, Highlight.Highlight highlight)
        {
            throw new NotImplementedException();
        }
    }
}
