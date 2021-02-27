using SkiaSharp;
using Xamarin.Forms;
using XF.ChartLibrary.Charts;

namespace XF.ChartLibrary.Components
{
    public partial class MarkerView : ContentView
    {
        public virtual SKPoint Offset
        {
            get
            {
                return new SKPoint(-((float)Width).DpToPixel() / 2, -((float)Height).DpToPixel());
            }
        }

        public virtual void OnDraw(SKCanvas canvas, SKPoint pos, IChartBase chart)
        {

        }

        public virtual SKPoint GetOffsetForDrawingAtPoint(SKPoint pos, SKImageInfo info, IChartBase chart)
        {
            var offset = Offset;

            float width = info.Width;
            float height = info.Height;

            if (pos.X + offset.X < 0)
            {
                offset.X = -pos.X;
            }
            else if (chart != null && pos.X + width + offset.X > chart.ChartWidth)
            {
                offset.X = chart.ChartWidth - pos.X - width;
            }

            if (pos.Y + offset.Y < 0)
            {
                offset.Y = -pos.Y;
            }
            else if (chart != null && pos.Y + height + offset.Y > chart.ChartHeight)
            {
                offset.Y = chart.ChartHeight - pos.Y - height;
            }

            return offset;
        }
    }
}
