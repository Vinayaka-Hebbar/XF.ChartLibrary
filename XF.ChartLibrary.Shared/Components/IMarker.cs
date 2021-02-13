using System;
using System.Collections.Generic;
using System.Text;
using XF.ChartLibrary.Data;

#if NETSTANDARD || SKIASHARP
using Point = SkiaSharp.SKPoint;
using Canvas = SkiaSharp.SKCanvas;
#elif __IOS__ || __TVOS__
using Point = CoreGraphics.CGPoint;
using Canvas = CoreGraphics.CGContext;
#elif __ANDROID__
using Point = Android.Graphics.PointF;
using Canvas = Android.Graphics.Canvas;
#endif

namespace XF.ChartLibrary.Components
{
    public interface IMarker
    {

        /// <summary>
        /// The desired (general) offset you wish the IMarker to have on the x- and y-axis.
        /// By returning x: -(width / 2) you will center the IMarker horizontally.
        /// By returning y: -(height / 2) you will center the IMarker vertically.
        /// </summary>
        Point Offset { get; }

        /// <summary>
        /// The offset for drawing at the specific `point`. This allows conditional adjusting of the Marker position.
        /// If you have no adjustments to make, return Offset.
        /// </summary>
        /// <param name="posX">This is the X position at which the marker wants to be drawn. You can adjust the offset conditionally based on this argument.</param>
        /// <param name="posY">This is the X position at which the marker wants to be drawn. You can adjust the offset conditionally based on this argument.</param>
        /// <returns>The offset for drawing at the specific `point`. This allows conditional adjusting of the Marker position.</returns>
        Point GetOffsetForDrawingAtPoint(float posX, float posY);

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
        void Draw(Canvas canvas, float posX, float posY);
    }
}
