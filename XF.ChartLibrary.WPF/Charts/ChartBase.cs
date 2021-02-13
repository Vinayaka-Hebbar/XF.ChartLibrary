using SkiaSharp;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using XF.ChartLibrary.Animation;
using XF.ChartLibrary.Components;
using XF.ChartLibrary.Jobs;

namespace XF.ChartLibrary.Charts
{
    partial class ChartBase<TData, TDataSet> : FrameworkElement
    {
        public static readonly DependencyProperty DataProperty = DependencyProperty.Register(nameof(Data), typeof(TData), typeof(ChartBase<TData, TDataSet>), new PropertyMetadata(null, OnDataChanged));

        public static readonly DependencyProperty XAxisProperty = DependencyProperty.Register(nameof(XAxis), typeof(XAxis), typeof(ChartBase<TData, TDataSet>), new PropertyMetadata(new XAxis()));

        static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ChartBase<TData, TDataSet>)d).OnDataChanged((TData)e.NewValue);
        }

        private readonly bool designMode;
        private WriteableBitmap bitmap;
        private bool ignorePixelScaling;

        protected SKPaint InfoPaint;
        protected SKPaint DescPaint;

        protected virtual void OnDataChanged(TData value)
        {
            offsetsCalculated = false;
            data = value;
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
                TextSize = 14f,
                IsAntialias = true
            };
            DescPaint = new SKPaint { IsAntialias = false };
            Initialize();
            designMode = DesignerProperties.GetIsInDesignMode(this);
        }

        public bool IgnorePixelScaling
        {
            get { return ignorePixelScaling; }
            set
            {
                ignorePixelScaling = value;
                InvalidateVisual();
            }
        }

        public XAxis XAxis
        {
            get => (XAxis)GetValue(XAxisProperty);
            set => SetValue(XAxisProperty, value);
        }

        public TData Data
        {
            get => (TData)GetValue(DataProperty);
            set => SetValue(DataProperty, value);
        }

        public virtual void AnimatorStopped(Animator animator)
        {
        }

        public void AnimatorUpdated(Animator animator)
        {
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            if (designMode)
                return;

            if (Visibility != Visibility.Visible)
                return;

            var size = CreateSize(out var scaleX, out var scaleY);
            if (size.Width <= 0 || size.Height <= 0)
                return;

            var info = new SKImageInfo(size.Width, size.Height, SKImageInfo.PlatformColorType, SKAlphaType.Premul);

            // reset the bitmap if the size has changed
            if (bitmap == null || info.Width != bitmap.PixelWidth || info.Height != bitmap.PixelHeight)
            {
                bitmap = new WriteableBitmap(info.Width, size.Height, 96 * scaleX, 96 * scaleY, PixelFormats.Pbgra32, null);
            }

            // draw on the bitmap
            bitmap.Lock();
            using (var surface = SKSurface.Create(info, bitmap.BackBuffer, bitmap.BackBufferStride))
            {
                OnPaintSurface(surface, info);
            }

            // draw the bitmap to the screen
            bitmap.AddDirtyRect(new Int32Rect(0, 0, info.Width, size.Height));
            bitmap.Unlock();
            drawingContext.DrawImage(bitmap, new Rect(0, 0, ActualWidth, ActualHeight));
        }

        public virtual void OnPaintSurface(SKSurface surface, SKImageInfo info)
        {
            if (Data == null && !string.IsNullOrEmpty(NoDataText))
            {
                var canvas = surface.Canvas;
                var rect = info.Rect;
                var x = rect.MidX;
                switch (InfoPaint.TextAlign)
                {
                    case SKTextAlign.Left:
                        x = 0;
                        canvas.DrawText(NoDataText, (float)x, (float)rect.MidY, InfoPaint);
                        break;

                    case SKTextAlign.Right:
                        x *= 2;
                        canvas.DrawText(NoDataText, (float)x, (float)rect.MidY, InfoPaint);
                        break;

                    default:
                        canvas.DrawText(NoDataText, (float)x, (float)rect.MidY, InfoPaint);
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

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            var size = sizeInfo.NewSize;
            if (size.Width > 0 && size.Height > 0 && size.Width < 10000 && size.Height < 10000)
            {
                ViewPortHandler.SetChartDimens((float)size.Width.DpToPixel(), (float)size.Height.DpToPixel());
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
            InvalidateVisual();
            base.OnRenderSizeChanged(sizeInfo);
        }

        SKSizeI CreateSize(out double scaleX, out double scaleY)
        {
            scaleX = 1.0;
            scaleY = 1.0;

            var w = ActualWidth;
            var h = ActualHeight;

            if (!IsPositive(w) || !IsPositive(h))
                return SKSizeI.Empty;

            if (IgnorePixelScaling)
                return new SKSizeI((int)w, (int)h);

            var m = PresentationSource.FromVisual(this).CompositionTarget.TransformToDevice;
            scaleX = m.M11;
            scaleY = m.M22;
            return new SKSizeI((int)(w * scaleX), (int)(h * scaleY));
        }

        static bool IsPositive(double value)
        {
            return !double.IsNaN(value) && !double.IsInfinity(value) && value > 0;
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
    }
}
