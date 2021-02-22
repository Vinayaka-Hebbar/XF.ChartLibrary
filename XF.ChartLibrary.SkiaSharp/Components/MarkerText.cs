using SkiaSharp;
using System;
using XF.ChartLibrary.Charts;
using XF.ChartLibrary.Data;

#if WPF
using Thickness = System.Windows.Thickness;
#else
using Thickness = Xamarin.Forms.Thickness;
#endif

namespace XF.ChartLibrary.Components
{
    /// <summary>
    /// <see cref="https://www.programmersought.com/article/71324588374/"/>
    /// </summary>
    public class MarkerText : IMarker
    {
        private string text;
        private SKPoint offset;

        private Thickness padding;

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

        protected SKPaint TextPaint;

        protected SKPaint ContentPaint;

        protected SKPaint BorderPaint;

        private int index;

        private SKColor backgroundColor = SKColors.White;
        public SKColor BackgroundColor
        {
            get => backgroundColor;
            set => backgroundColor = value;
        }

        public MarkerText()
        {
            TextPaint = new SKPaint
            {
                IsAntialias = true,
                TextSize = 11f.DpToPixel(),
                Color = Utils.ColorTemplate.Black
            };
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
            float horizontal = 10f.DpToPixel();
            float vertical = 6f.DpToPixel();
            padding = new Thickness(horizontal, vertical, horizontal, vertical);
            strokeWidth = 2f.DpToPixel();
            arrowSize = (int)12f.DpToPixel();
            circleOffset = 4f.DpToPixel();
        }

        public SKPoint Offset => offset;

        public void Draw(SKCanvas canvas, float posX, float posY, IChartBase chart)
        {
            if (text == null)
                return;
            var paint = BorderPaint;//The brush for drawing the border
            paint.StrokeWidth = strokeWidth;
            paint.Color = chart.Data[index].Color;

            var whitePaint = ContentPaint;//Draw a brush with a white background
            whitePaint.Color = backgroundColor;

            SKRect bounds = SKRect.Empty;
            float lineHeight = paint.GetFontMetrics(out SKFontMetrics fontMatrics);
            TextPaint.MeasureText(text, ref bounds);
            var padding = this.padding;
            var size = new SKRect(bounds.Left - (float)padding.Left, bounds.Top - (float)padding.Top, bounds.Right + (float)padding.Right, bounds.Bottom + (float)padding.Bottom);
            float width = size.Width;
            float height = size.Height;

            var offset = GetOffsetForDrawingAtPoint(size, posX, posY, chart);
            int saveId = canvas.Save();
            var offsetX = (float)padding.Left - strokeWidth;
            var offsetY = lineHeight - fontMatrics.Ascent;
            path.Reset();
            if (posY < height + arrowSize)
            {
                // Processing exceeds the upper boundary
                path.MoveTo(0, 0);
                if (posX > chart.ChartWidth - width)
                {
                    //Exceed the right boundary
                    path.LineTo(width - arrowSize, 0);
                    path.LineTo(width, -arrowSize + circleOffset);
                    path.LineTo(width, 0);
                }
                else
                {
                    float half = width / 2;
                    if (posX > half)
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
                path.Offset(posX + offset.X, posY + offset.Y);
                offsetY += (float)padding.Bottom * 0.8f;
            }
            else
            {
                // Does not exceed the upper boundary
                path.MoveTo(0, 0);
                path.LineTo(0 + width, 0);
                path.LineTo(0 + width, 0 + height);
                if (posX > chart.ChartWidth - width)
                {
                    path.LineTo(width, height + arrowSize - circleOffset);
                    path.LineTo(width - arrowSize, 0 + height);
                    path.LineTo(0, 0 + height);
                }
                else
                {
                    float half = width / 2;
                    if (posX > half)
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
                path.Offset(posX + offset.X, posY + offset.Y);
                offsetY += (float)padding.Top * 0.8f;
            }

            // translate to the correct position and draw
            canvas.DrawPath(path, whitePaint);
            canvas.DrawPath(path, paint);
            canvas.Translate(posX + offset.X, posY + offset.Y);
            canvas.DrawText(text, offsetX, offsetY, TextPaint);
            canvas.RestoreToCount(saveId);
        }

        public virtual SKPoint GetOffsetForDrawingAtPoint(SKRect size, float posX, float posY, IChartBase chart)
        {
            float width = size.Width;
            float height = size.Height;

            // posY \posX refers to the position of the upper left corner of the markerView on the chart
            //Handle Y direction
            if (posY <= height + arrowSize)
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
            if (posX > chart.ChartWidth - width)
            {
                //If it exceeds the right boundary, offset the width of the markerView to the left
                offset.X = -width;
            }
            else
            {
                //By default, no offset (because the point is in the upper left corner)
                offset.X = 0;
                float half = width / 2;
                if (posX > half)
                {
                    //If it is greater than half of the markerView, the arrow is in the middle, so it is offset by half the width to the right
                    offset.X = -half;
                }
            }

            return offset;
        }

        public virtual void RefreshContent(Entry e, Highlight.Highlight highlight)
        {
            text = GetText(e, highlight);
            index = highlight.DataSetIndex;
        }

        protected virtual string GetText(Entry e, Highlight.Highlight highlight)
        {
            return e.Y.ToString();
        }
    }
}
