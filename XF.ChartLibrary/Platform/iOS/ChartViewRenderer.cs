using CoreGraphics;
using Foundation;
using System;
using System.ComponentModel;
using UIKit;
using Xamarin.Forms.Platform.iOS;

namespace XF.ChartLibrary.Platform.iOS
{
    public class ChartViewRenderer<TElement> : VisualElementRenderer<TElement> where TElement : Xamarin.Forms.VisualElement, IChartController, IComponent
    {
        private static readonly NSString boundsPath = new NSString("bounds");

        private static readonly NSString framePath = new NSString("frame");

        private const int BitsPerByte = 8; // 1 byte = 8 bits
        private const CGBitmapFlags BitmapFlags = CGBitmapFlags.ByteOrder32Big | CGBitmapFlags.PremultipliedLast;

        private NSMutableData bitmapData;
        private CGDataProvider dataProvider;

        private Gestures.IChartGesture gesture;

#pragma warning disable IDE0052 // Not Used
        private SkiaSharp.SKImageInfo info;
#pragma warning restore IDE0052 

        private bool designMode;

        public ChartViewRenderer()
        {
            Initialize();
        }

        void Initialize()
        {
            designMode = ((IComponent)this).Site?.DesignMode == true;
        }

        public override void AwakeFromNib()
        {
            Initialize();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ICanvasController.IgnorePixelScaling))
            {
                SetNeedsDisplay();
            }
            base.OnElementPropertyChanged(sender, e);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<TElement> e)
        {
            if (e.OldElement != null)
            {
                var oldElement = e.OldElement;
                oldElement.SurfaceInvalidated -= SetNeedsDisplay;
                Detach();
            }
            if (e.NewElement != null)
            {
                var newElement = e.NewElement;
                gesture = newElement.Gesture;
                gesture.OnInitialize(this);
                OnCreateElement(newElement);
                newElement.SurfaceInvalidated += SetNeedsDisplay;
                SetNeedsDisplay();
            }
            base.OnElementChanged(e);
        }

        protected virtual void Detach()
        {
        }

        protected virtual void OnCreateElement(TElement element)
        {
        }

        #region Draw
        public override void Draw(CGRect rect)
        {
            base.Draw(rect);

            if (designMode)
                return;

            // create the skia context
            using (var surface = CreateSurface(Bounds, Element.IgnorePixelScaling ? 1 : ContentScaleFactor, out var info))
            {
                if (info.Width == 0 || info.Height == 0)
                    return;

                using (var ctx = UIGraphics.GetCurrentContext())
                {
                    Element.OnPaintSurface(surface, info);

                    // draw the surface to the context
                    DrawSurface(ctx, Bounds, info, surface);
                }
            }
        }
        #endregion

        public SkiaSharp.SKSurface CreateSurface(CGRect contentsBounds, nfloat scale, out SkiaSharp.SKImageInfo info)
        {
            // apply a scale
            contentsBounds.Width *= scale;
            contentsBounds.Height *= scale;

            // get context details
            this.info = info = new SkiaSharp.SKImageInfo((int)contentsBounds.Width, (int)contentsBounds.Height);

            // if there are no pixels, clean up and return
            if (info.Width == 0 || info.Height == 0)
            {
                FreeBitmap();
                this.info = SkiaSharp.SKImageInfo.Empty;
                return null;
            }

            // if the memory size has changed, then reset the underlying memory
            if (bitmapData?.Length != (nuint)info.BytesSize)
                FreeBitmap();

            // allocate a memory block for the drawing process
            if (bitmapData == null)
            {
                bitmapData = NSMutableData.FromLength(info.BytesSize);
                dataProvider = new CGDataProvider(bitmapData.MutableBytes, info.BytesSize, Dummy);
            }

            return SkiaSharp.SKSurface.Create(info, bitmapData.MutableBytes, info.RowBytes);
        }

        void Dummy(IntPtr data)
        {
            // do nothing as we manage the memory separately
        }

        public void DrawSurface(CGContext ctx, CGRect viewBounds, SkiaSharp.SKImageInfo info, SkiaSharp.SKSurface surface)
        {
            if (info.Width == 0 || info.Height == 0)
                return;

            surface.Canvas.Flush();

            // draw the image onto the context
            using (var colorSpace = CGColorSpace.CreateDeviceRGB())
            using (var image = new CGImage(info.Width, info.Height, BitsPerByte, info.BytesPerPixel * BitsPerByte, info.RowBytes, colorSpace, BitmapFlags, dataProvider, null, false, CGColorRenderingIntent.Default))
            {
#if __IOS__ || __TVOS__ || __WATCHOS__ || HAS_UNO
                // we need to flip the image as we are mixing CoreGraphics and UIKit functions:
                // https://developer.apple.com/library/ios/documentation/2DDrawing/Conceptual/DrawingPrintingiOS/GraphicsDrawingOverview/GraphicsDrawingOverview.html#//apple_ref/doc/uid/TP40010156-CH14-SW26
                ctx.SaveState();
                ctx.TranslateCTM(0, viewBounds.Height);
                ctx.ScaleCTM(1, -1);
                // draw the image
                ctx.DrawImage(viewBounds, image);
                ctx.RestoreState();
#elif __MACOS__
				// draw the image
				ctx.DrawImage(viewBounds, image);
#else
#error Plaform-specific code missing
#endif
            }
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            Layer.SetNeedsDisplay();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                gesture?.Dispose();
            }
            base.Dispose(disposing);
            // make sure we free the image data
            FreeBitmap();
            info = SkiaSharp.SKImageInfo.Empty;
        }

        private void FreeBitmap()
        {
            dataProvider?.Dispose();
            dataProvider = null;
            bitmapData?.Dispose();
            bitmapData = null;
        }


        public override void ObserveValue(NSString keyPath, NSObject ofObject, NSDictionary change, IntPtr context)
        {
            if (keyPath == boundsPath || keyPath == framePath)
            {
                var bounds = Bounds;
                if (Element != null && (bounds.Size.Width != Element.ChartWidth ||
                bounds.Size.Height != Element.ChartHeight))
                {
                    Element.OnSizeChanged((float)bounds.Width, (float)bounds.Height);
                }
            }
        }

        public override bool GestureRecognizerShouldBegin(UIGestureRecognizer gestureRecognizer)
        {
            if (!base.GestureRecognizerShouldBegin(gestureRecognizer))
            {
                return true;
            }
            if (gestureRecognizer is UIPanGestureRecognizer panGesture)
            {
                var velocity = panGesture.VelocityInView(this);
                var viewPortHandler = Element.ViewPortHandler;
                if (Element is Charts.IGestureController controller && (Element.Data == null || !controller.IsDragEnabled ||
                (viewPortHandler.HasNoDragOffset && viewPortHandler.IsFullyZoomedOut && !controller.HighlightPerDragEnabled) ||
                (!controller.DragYEnabled && Math.Abs(velocity.Y) > Math.Abs(velocity.X)) ||
                (!controller.DragXEnabled && Math.Abs(velocity.Y) < Math.Abs(velocity.X))))
            {
                    return false;
            }
            }
            else
            {
#if !__TVOS__
                if (gestureRecognizer is UIPinchGestureRecognizer)
                {
                    if (Element is Charts.IGestureController controller && (Element.Data == null || (!controller.PinchZoomEnabled && !controller.ScaleXEnabled && !controller.ScaleYEnabled)))
                    {
                        return false;
                    }
                }
#endif
            }

            return true;
        }
    }
}
