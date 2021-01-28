using SkiaSharp;
using SkiaSharp.Views.Forms;
using XF.ChartLibrary.Animation;
using XF.ChartLibrary.Jobs;

namespace XF.ChartLibrary.Charts
{
    public abstract partial class ChartBase<TData, TDataSet> : SKCanvasView, ISKCanvasViewController, IAnimator
    {
        public enum ChartGesture
        {
            None, Drag, XZoom, YZoom, PinchZoom, Rotate, SingleTap, DoubleTap, LongPress, Fling
        }

        protected SKPaint InfoPaint;
        protected SKPaint DescPaint;

        protected ChartBase()
        {
            InfoPaint = new SKPaint
            {
                Color = new SKColor(247, 189, 51), // orange
                TextAlign = SKTextAlign.Center,
                TextSize = 12f,
                IsAntialias = true
            };
            DescPaint = new SKPaint { IsAntialias = false };
            Initialize();
        }

        public virtual void AnimatorStopped(Animator animator)
        {
        }

        public void AnimatorUpdated(Animator animator)
        {
            InvalidateSurface();
        }

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;

            if (data == null && !string.IsNullOrEmpty(NoDataText))
            {
                var pt = Bounds.Center;

                switch (InfoPaint.TextAlign)
                {
                    case SKTextAlign.Left:
                        pt.X = 0;
                        canvas.DrawText(NoDataText, (float)pt.X, (float)pt.Y, InfoPaint);
                        break;

                    case SKTextAlign.Right:
                        pt.X *= 2.0;
                        canvas.DrawText(NoDataText, (float)pt.X, (float)pt.Y, InfoPaint);
                        break;

                    default:
                        canvas.DrawText(NoDataText, (float)pt.X, (float)pt.Y, InfoPaint);
                        break;
                }

                return;
            }

            if (!offsetsCalculated)
            {

                CalculateOffsets();
                offsetsCalculated = true;
            }
        }

        /// <summary>
        /// Draws the description text in the bottom right corner of the chart (per default)
        /// </summary>
        protected void DrawDescription(SKCanvas c)
        {

            // check if description should be drawn
            if (description != null && description.IsEnabled)
            {

                var position = description.Position;

                DescPaint.Typeface = description.Typeface;
                DescPaint.TextSize = description.TextSize;
                DescPaint.Color = description.TextColor;
                DescPaint.TextAlign = description.TextAlign;

                float x, y;

                // if no position specified, draw on default position
                if (position == null)
                {
                    x = (float)Width - ViewPortHandler.OffsetRight - description.XOffset;
                    y = (float)Height - ViewPortHandler.OffsetBottom - description.YOffset;
                }
                else
                {
                    x = position.X;
                    y = position.Y;
                }

                c.DrawText(description.Text, x, y, DescPaint);
            }
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            if (width > 0 && height > 0 && width < 10000 && height < 10000)
            {
                ViewPortHandler.SetChartDimens((float)width, (float)height);
            }

            // This may cause the chart view to mutate properties affecting the view port --
            //   lets do this before we try to run any pending jobs on the view port itself
            NotifyDataSetChanged();

            var jobs = ViewPortJobs;
            int count = jobs.Count;
            int index = 0;
            while (index < count)
            {
                if(jobs[index++] is ViewPortJob job)
                {
                    job.DoJob();
                }
            }

            base.OnSizeAllocated(width, height);
        }
    }


}
