using SkiaSharp;
using System;
using Xamarin.Forms;
using XF.ChartLibrary.Animation;
using XF.ChartLibrary.Components;
using XF.ChartLibrary.Jobs;

namespace XF.ChartLibrary.Charts
{
    public abstract partial class ChartBase<TData, TDataSet> : View, IAnimator
    {
        public static readonly BindableProperty DataProperty = BindableProperty.Create(nameof(Data), typeof(TData), typeof(ChartBase<TData, TDataSet>), defaultValue: null, propertyChanged: OnDataChanged);

        public static readonly BindableProperty XAxisProperty = BindableProperty.Create(nameof(XAxis), typeof(XAxis), typeof(ChartBase<TData, TDataSet>), defaultValue: new XAxis());


        protected SKPaint InfoPaint;
        protected SKPaint DescPaint;

        internal event Action SurfaceInvalidated;

        static void OnDataChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((ChartBase<TData, TDataSet>)bindable).OnDataChanged((TData)newValue);
        }

        protected virtual void OnDataChanged(TData value)
        {
            offsetsCalculated = false;
            if (value == null)
                return;
            SetUpDefaultFormatter(value.YMin, value.YMax);
            foreach (TDataSet set in value.DataSets)
            {
                if (set.NeedsFormatter || set.ValueFormatter == DefaultValueFormatter)
                    set.ValueFormatter = DefaultValueFormatter;
            }
            NotifyDataSetChanged();
        }

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

        public TData Data
        {
            get => (TData)GetValue(DataProperty);
            set => SetValue(DataProperty, value);
        }

        public XAxis XAxis
        {
            get => (XAxis)GetValue(XAxisProperty);
            set => SetValue(XAxisProperty, value);
        }

        public virtual void AnimatorStopped(Animator animator)
        {
        }

        public void AnimatorUpdated(Animator animator)
        {
            InvalidateSurface();
        }

        public void InvalidateSurface() => SurfaceInvalidated?.Invoke();

        public virtual void OnPaintSurface(SKSurface surface, SKImageInfo e)
        {
            var canvas = surface.Canvas;
            if (Data == null && !string.IsNullOrEmpty(NoDataText))
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
                if (jobs[index++] is ViewPortJob job)
                {
                    job.DoJob();
                }
            }

            base.OnSizeAllocated(width, height);
        }
    }


}
