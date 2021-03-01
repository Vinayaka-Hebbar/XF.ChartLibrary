using CoreGraphics;
using Xamarin.Forms.Platform.iOS;

namespace XF.ChartLibrary.Platform.iOS
{
    public class MarkerViewRenderer : VisualElementRenderer<Components.MarkerView>
    {
        private SkiaSharp.SKImageInfo info;

        public SkiaSharp.SKImageInfo CanvasInfo => info;

        public void Draw(SkiaSharp.SKCanvas canvas)
        {
            var info = this.info;
            var viewBounds = Bounds;
            var scale = (float)UIKit.UIScreen.MainScreen.Scale;
            using (var image = new SkiaSharp.SKBitmap(info, info.RowBytes))
            {
                using (var pixels = image.PeekPixels())
                using (var colorSpace = CGColorSpace.CreateDeviceRGB())
                using (var ctx = new CGBitmapContext(pixels.GetPixels(), info.Width, info.Height, 8, info.RowBytes, colorSpace, CGBitmapFlags.PremultipliedLast | CGBitmapFlags.ByteOrder32Big))
                {
                    ctx.ClearRect(viewBounds);
                    ctx.TranslateCTM(0, info.Height);
                    ctx.ScaleCTM(scale, -scale);
                    Layer.RenderInContext(ctx);
                }
                canvas.DrawBitmap(image, SkiaSharp.SKPoint.Empty);
            }
        }

        public void UpdateMarkerLayout()
        {
            Components.MarkerView element = Element;
            if (element == null)
                return;
            var size = element.Measure(double.PositiveInfinity, double.PositiveInfinity, Xamarin.Forms.MeasureFlags.IncludeMargins);
            Xamarin.Forms.Size request = size.Request;
            Frame = new CGRect(element.X, element.Y, request.Width, request.Height);
            LayoutSubviews();
            UpdateCanvasInfo(Frame, UIKit.UIScreen.MainScreen.Scale);
        }

        void UpdateCanvasInfo(CGRect frame, System.nfloat scale)
        {
            info = new SkiaSharp.SKImageInfo((int)(Frame.Width * scale), (int)(frame.Height * scale));
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            Xamarin.Forms.Layout.LayoutChildIntoBoundingRegion(Element, new Xamarin.Forms.Rectangle(0, 0, Frame.Width, Frame.Height));
        }
    }
}
