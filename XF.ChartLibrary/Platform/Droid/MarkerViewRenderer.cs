using Android.Content;
using Android.Graphics;
using SkiaSharp;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace XF.ChartLibrary.Platform.Droid
{
    public class MarkerViewRenderer : VisualElementRenderer<Components.MarkerView>
    {
        private SKImageInfo info;

        private Bitmap bitmap;

        private Canvas bitmapCanvas;

        public MarkerViewRenderer(Context context) : base(context)
        {
            LayoutParameters = new LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent);
        }

        public SKImageInfo CanvasInfo => info;

        public void Draw(SKCanvas canvas)
        {
            var info = this.info;

            // if there are no pixels, clean up and return
            if (info.Width == 0 || info.Height == 0)
            {
                return;
            }

            // if the memory size has changed, then reset the underlying memory
            if (bitmap != null && (bitmap.Handle == IntPtr.Zero || bitmap.Width != info.Width || bitmap?.Height != info.Height))
                FreeBitmap();

            if (bitmap == null)
            {
                bitmap = Bitmap.CreateBitmap(info.Width, info.Height, Bitmap.Config.Argb8888);
                bitmapCanvas = new Canvas(bitmap);
            }
            bitmap.EraseColor(Android.Graphics.Color.Transparent);
            base.Draw(bitmapCanvas);
            using (var image = SKImage.FromPixels(info, bitmap.LockPixels(), info.RowBytes))
            {
                canvas.DrawImage(image, SKPoint.Empty);
                bitmap.UnlockPixels();
            }
        }

        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            base.OnSizeChanged(w, h, oldw, oldh);
            UpdateCanvasSize(w, h);
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            Element.Layout(new Rectangle(new Xamarin.Forms.Point(l, t), Context.FromPixels(r - l, b - t)));
            base.OnLayout(changed, l, t, r, b);
        }

        public void UpdateMarkerLayout()
        {
            var view = Element;
            var _context = Context;
            var headlessOffset = CompressedLayout.GetHeadlessOffset(view);
            var x = (int)_context.ToPixels(view.X + headlessOffset.X);
            var y = (int)_context.ToPixels(view.Y + headlessOffset.Y);
            var size = view.Measure(double.PositiveInfinity, double.PositiveInfinity, MeasureFlags.IncludeMargins);
            var width = Math.Max(0, (int)_context.ToPixels(size.Request.Width));
            var height = Math.Max(0, (int)_context.ToPixels(size.Request.Height));

            Measure(MeasureSpec.MakeMeasureSpec(width, Android.Views.MeasureSpecMode.Unspecified), MeasureSpec.MakeMeasureSpec(height, Android.Views.MeasureSpecMode.Unspecified));
            Layout(x, y, x + width, y + height);
        }

        internal void UpdateCanvasSize(int w, int h)
        {
            info = new SKImageInfo(w, h, SKColorType.Rgba8888, SKAlphaType.Premul);

            // if there are no pixels, clean up
            if (info.Width == 0 || info.Height == 0)
                FreeBitmap();
        }

        private void FreeBitmap()
        {
            if (bitmap == null)
                return;

            // free and recycle the bitmap data
            if (bitmap.Handle != IntPtr.Zero && !bitmap.IsRecycled)
                bitmap.Recycle();

            bitmap.Dispose();
            bitmap = null;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                FreeBitmap();
            }
        }
    }
}
