using SkiaSharp;
using System;
using XF.ChartLibrary.Charts;
using XF.ChartLibrary.Data;

namespace XF.ChartLibrary.Components
{
    partial class MarkerView : IMarker
    {
        private Platform.iOS.MarkerViewRenderer renderer;

        public Platform.iOS.MarkerViewRenderer GetRenderer()
        {
            if (renderer == null)
            {
                renderer = (Platform.iOS.MarkerViewRenderer)Xamarin.Forms.Platform.iOS.Platform.GetRenderer(this);
            }
            return renderer;
        }

        public void Draw(SKCanvas canvas, SKPoint pos, IChartBase chart)
        {
            var renderer = GetRenderer();
            if (renderer != null)
            {
                var offset = GetOffsetForDrawingAtPoint(pos, renderer.CanvasInfo, chart);
                int saveId = canvas.Save();
                OnDraw(canvas, pos, chart);
                // translate to the correct position and draw
                canvas.Translate(pos.X + offset.X, pos.Y + offset.Y);

                renderer.Draw(canvas);
                canvas.RestoreToCount(saveId);
            }
        }

        public virtual void RefreshContent(Data.Entry e, Highlight.Highlight highlight)
        {
            var renderer = GetRenderer();
            if (renderer != null)
            {
                renderer.UpdateMarkerLayout();
            }
        }
    }
}
