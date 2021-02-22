using System;
using System.Collections.Generic;
using System.Text;
using XF.ChartLibrary.Charts;
using XF.ChartLibrary.Data;

#if NETSTANDARD || SKIASHARP
using Canvas = SkiaSharp.SKCanvas;
#elif __IOS__ || __TVOS__
using Canvas = CoreGraphics.CGContext;
#elif __ANDROID__
using Canvas = Android.Graphics.Canvas;
#endif

namespace XF.ChartLibrary.Components
{
    public interface IMarker
    {
        /// <summary>
        ///  This method enables a specified custom IMarker to update it's content every time the IMarker is redrawn.
        /// </summary>
        /// <param name="e">The Entry the IMarker belongs to.This can also be any subclass of Entry, like BarEntry or  CandleEntry, simply cast it at runtime.</param>
        /// <param name="highlight">The highlight object contains information about the highlighted value such as it's dataset-index, the selected range or stack-index (only stacked bar entries).</param>
        void RefreshContent(Entry e, Highlight.Highlight highlight);

        /// <summary>
        /// Draws the IMarker on the given position on the screen with the given Canvas object.
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        void Draw(Canvas canvas, float posX, float posY, IChartBase chart);
    }
}
