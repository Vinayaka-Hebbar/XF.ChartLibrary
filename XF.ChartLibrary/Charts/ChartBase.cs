using SkiaSharp;
using System;
using Xamarin.Forms;
using XF.ChartLibrary.Animation;
using XF.ChartLibrary.Components;
using XF.ChartLibrary.Jobs;

namespace XF.ChartLibrary.Charts
{
    public abstract partial class ChartBase<TData, TDataSet> : View, IAnimator, IChartController
    {
        public static readonly BindableProperty IgnorePixelScalingProperty =
                  BindableProperty.Create(nameof(IgnorePixelScaling), typeof(bool), typeof(ChartBase<TData, TDataSet>), false);

        public static readonly BindableProperty DataProperty = BindableProperty.Create(nameof(Data), typeof(TData), typeof(ChartBase<TData, TDataSet>), defaultValue: null, propertyChanged: OnDataChanged);

        public static readonly BindableProperty XAxisProperty = BindableProperty.Create(nameof(XAxis), typeof(XAxis), typeof(ChartBase<TData, TDataSet>), defaultValue: new XAxis(), defaultBindingMode: BindingMode.OneWayToSource);

        protected SKPaint InfoPaint;
        protected SKPaint DescPaint;

        private float _dragDecelerationFrictionCoef = 0.9f;
        /// <summary>
        /// Deceleration friction coefficient in [0 ; 1] interval, higher values indicate that speed will decrease slowly, for e
        /// if it set to 0, it will stop immediately.
        /// 1 is an invalid value, and will be converted to 0.999 automatically.
        /// </summary>
        public float DragDecelerationFrictionCoef
        {
            get
            {
                return _dragDecelerationFrictionCoef;
            }
            set
            {
                _dragDecelerationFrictionCoef = Math.Max(0, Math.Min(value, 0.999f));
            }
        }

        public bool IgnorePixelScaling
        {
            get { return (bool)GetValue(IgnorePixelScalingProperty); }
            set { SetValue(IgnorePixelScalingProperty, value); }
        }

        static void OnDataChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((ChartBase<TData, TDataSet>)bindable).OnDataChanged((TData)oldValue, (TData)newValue);
        }

        protected virtual void OnDataChanged(TData oldValue, TData newValue)
        {
            offsetsCalculated = false;
            data = newValue;
            if (newValue == null)
                return;
            SetUpDefaultFormatter(newValue.YMin, newValue.YMax);
            foreach (TDataSet set in newValue.DataSets)
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
            get => data;
            set => SetValue(DataProperty, value);
        }

        public XAxis XAxis
        {
            get => (XAxis)GetValue(XAxisProperty);
            protected set => SetValue(XAxisProperty, value);
        }

        public virtual Gestures.IChartGesture Gesture { get; }

        public event Action SurfaceInvalidated;

        public void InvalidateSurface() => SurfaceInvalidated?.Invoke();

        public virtual void AnimatorStopped(Animator animator)
        {
        }

        public void AnimatorUpdated(Animator animator)
        {
            InvalidateSurface();
        }

        public virtual void OnPaintSurface(SKSurface surface, SKImageInfo e)
        {
            var canvas = surface.Canvas;
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


        public virtual void OnSizeChanged(float w, float h)
        {
            if (w > 0 && h > 0 && w < 10000 && h < 10000)
            {
                ViewPortHandler.SetChartDimens(w, h);
            }
            else
            {
                System.Diagnostics.Trace.TraceError("*Avoiding* setting chart dimens! width: " + w + ", height: " + h);
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

        }
    }


}
