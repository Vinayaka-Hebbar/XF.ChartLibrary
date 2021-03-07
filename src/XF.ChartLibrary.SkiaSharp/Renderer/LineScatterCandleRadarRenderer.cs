using SkiaSharp;
using XF.ChartLibrary.Interfaces.DataSets;

namespace XF.ChartLibrary.Renderer
{
    partial class LineScatterCandleRadarRenderer
    {
        /// path that is used for drawing highlight-lines (drawLines(...) cannot be used because of dashes)
        private readonly SKPath highlightLinePath = new SKPath();

        /// <summary>
        /// Draws vertical & horizontal highlight-lines if enabled.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="x">x-position of the highlight line intersection</param>
        /// <param name="y">y-position of the highlight line intersection</param>
        /// <param name="set">the currently drawn dataset</param>
        protected void DrawHighlightLines(SKCanvas c, float x, float y, ILineScatterCandleRadarDataSet set)
        {
            // set color and stroke-width
            HighlightPaint.Color = set.HighLightColor;
            HighlightPaint.StrokeWidth = set.HighlightLineWidth;

            // draw highlighted lines (if enabled)
            HighlightPaint.PathEffect = set.DashPathEffectHighlight;

            // draw vertical highlight lines
            if (set.DrawVerticalHighlightIndicatorEnabled)
            {
                // create vertical path
                highlightLinePath.Reset();
                highlightLinePath.MoveTo(x, ViewPortHandler.ContentTop);
                highlightLinePath.LineTo(x, ViewPortHandler.ContentBottom);

                c.DrawPath(highlightLinePath, HighlightPaint);
            }

            // draw horizontal highlight lines
            if (set.DrawHorizontalHighlightIndicatorEnabled)
            {

                // create horizontal path
                highlightLinePath.Reset();
                highlightLinePath.MoveTo(ViewPortHandler.ContentLeft, y);
                highlightLinePath.LineTo(ViewPortHandler.ContentRight, y);

                c.DrawPath(highlightLinePath, HighlightPaint);
            }
        }
    }
}
