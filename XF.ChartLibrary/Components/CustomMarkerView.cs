using SkiaSharp;
using XF.ChartLibrary.Charts;
using XF.ChartLibrary.Utils;

namespace XF.ChartLibrary.Components
{
    public class CustomMarkerView : MarkerView
    {
        private SKPoint offset;

        private int arrowSize; // The size of the arrow
        private float circleOffset;//Because my turning point here is a circle, it needs to be offset to prevent it from pointing directly to the center of the circle
        private float strokeWidth;

        public float StrokeWidth
        {
            get => strokeWidth;
            set => strokeWidth = value.DpToPixel();
        }

        public float CircleOffset
        {
            get => circleOffset;
            set => circleOffset = value.DpToPixel();
        }

        public float ArrowSize
        {
            get => arrowSize;
            set => arrowSize = (int)value.DpToPixel();
        }

        private readonly SKPath path = new SKPath();

        protected SKPaint ContentPaint;

        protected SKPaint BorderPaint;

        private int index;

        public CustomMarkerView()
        {
            ContentPaint = new SKPaint()
            {
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
            };
            BorderPaint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                IsAntialias = true,
                StrokeJoin = SKStrokeJoin.Round,
            };
            strokeWidth = 2f.DpToPixel();
            arrowSize = (int)12f.DpToPixel();
            circleOffset = 4f.DpToPixel();
        }

        public override void OnDraw(SKCanvas canvas, SKPoint pos, IChartBase chart)
        {
            var paint = BorderPaint;//The brush for drawing the border
            paint.StrokeWidth = strokeWidth;
            paint.Color = chart.Data[index].Color;

            var whitePaint = ContentPaint;//Draw a brush with a white background
            whitePaint.Color = BackgroundColor.ToSKColor();

            float width = ((float)Width).DpToPixel();
            float height = ((float)Height).DpToPixel();

            path.Reset();
            if (pos.Y < height + arrowSize)
            {
                // Processing exceeds the upper boundary
                path.MoveTo(0, 0);
                if (pos.X > chart.ChartWidth - width)
                {
                    //Exceed the right boundary
                    path.LineTo(width - arrowSize, 0);
                    path.LineTo(width, -arrowSize + circleOffset);
                    path.LineTo(width, 0);
                }
                else
                {
                    float half = width / 2;
                    if (pos.X > half)
                    {
                        //In the middle of the chart
                        path.LineTo(half - arrowSize / 2, 0);
                        path.LineTo(half, -arrowSize + circleOffset);
                        path.LineTo(half + arrowSize / 2, 0);
                    }
                    else
                    {
                        //Exceed the left margin
                        path.LineTo(0, -arrowSize + circleOffset);
                        path.LineTo(0 + arrowSize, 0);
                    }
                }
                path.LineTo(0 + width, 0);
                path.LineTo(0 + width, 0 + height);
                path.LineTo(0, 0 + height);
                path.LineTo(0, 0);
                path.Offset(pos.X + offset.X, pos.Y + offset.Y);
            }
            else
            {
                // Does not exceed the upper boundary
                path.MoveTo(0, 0);
                path.LineTo(0 + width, 0);
                path.LineTo(0 + width, 0 + height);
                if (pos.X > chart.ChartWidth - width)
                {
                    path.LineTo(width, height + arrowSize - circleOffset);
                    path.LineTo(width - arrowSize, 0 + height);
                    path.LineTo(0, 0 + height);
                }
                else
                {
                    float half = width / 2;
                    if (pos.X > half)
                    {
                        path.LineTo(half + arrowSize / 2, 0 + height);
                        path.LineTo(half, height + arrowSize - circleOffset);
                        path.LineTo(half - arrowSize / 2, 0 + height);
                        path.LineTo(0, 0 + height);
                    }
                    else
                    {
                        path.LineTo(0 + arrowSize, 0 + height);
                        path.LineTo(0, height + arrowSize - circleOffset);
                        path.LineTo(0, 0 + height);
                    }
                }
                path.LineTo(0, 0);
                path.Offset(pos.X + offset.X, pos.Y + offset.Y);
            }

            // translate to the correct position and draw
            canvas.DrawPath(path, whitePaint);
            canvas.DrawPath(path, paint);
        }

        public override SKPoint GetOffsetForDrawingAtPoint(SKPoint pos, SKImageInfo info, IChartBase chart)
        {
            float width = info.Width;
            float height = info.Height;

            // posY \posX refers to the position of the upper left corner of the markerView on the chart
            //Handle Y direction
            if (pos.Y <= height + arrowSize)
            {
                // If the y coordinate of the point is less than the height of the markerView, if it is not processed, it will exceed the upper boundary. After processing, the arrow is up at this time, and we need to move the icon down by the size of the arrow
                offset.Y = arrowSize;
            }
            else
            {
                //Otherwise, it is normal, because our default is that the arrow is facing downwards, and then the normal offset is that you need to offset the height of the markerView and the arrow size, plus a stroke width, because you need to see the upper border of the dialog box
                offset.Y = -height - arrowSize - strokeWidth; // 40 arrow height   5 stroke width
            }
            //Processing the X direction, divided into 3 cases, 1. On the left side of the chart 2. On the middle of the chart 3. On the right side of the chart
            if (pos.X > chart.ChartWidth - width)
            {
                //If it exceeds the right boundary, offset the width of the markerView to the left
                offset.X = -width;
            }
            else
            {
                //By default, no offset (because the point is in the upper left corner)
                offset.X = 0;
                float half = width / 2;
                if (pos.X > half)
                {
                    //If it is greater than half of the markerView, the arrow is in the middle, so it is offset by half the width to the right
                    offset.X = -half;
                }
            }

            return offset;
        }

        public override void RefreshContent(Data.Entry e, Highlight.Highlight highlight)
        {
            index = highlight.DataSetIndex;
            base.RefreshContent(e, highlight);
        }
    }
}
