using SkiaSharp;
using XF.ChartLibrary.Charts;

namespace XF.ChartLibrary.Components
{
    public partial class MarkerView : IMarker
    {
        private Platform.Droid.MarkerViewRenderer renderer;

        public Platform.Droid.MarkerViewRenderer GetRenderer()
        {
            if (renderer == null)
            {
                renderer = (Platform.Droid.MarkerViewRenderer)Xamarin.Forms.Platform.Android.Platform.GetRenderer(this);
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
