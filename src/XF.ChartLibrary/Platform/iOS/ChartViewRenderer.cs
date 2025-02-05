﻿using CoreGraphics;
using Foundation;
using System;
using System.ComponentModel;
using UIKit;
using Xamarin.Forms.Platform.iOS;
using XF.ChartLibrary.Charts;

namespace XF.ChartLibrary.Platform.iOS
{
    public class ChartViewRenderer<TElement> : VisualElementRenderer<TElement>, IComponent, IVisualElementRenderer where TElement : Xamarin.Forms.VisualElement, IChartController
    {
        private static readonly NSString boundsPath = new NSString("bounds");

        private static readonly NSString framePath = new NSString("frame");

        private const int BitsPerByte = 8; // 1 byte = 8 bits
        private const CGBitmapFlags BitmapFlags = CGBitmapFlags.ByteOrder32Big | CGBitmapFlags.PremultipliedLast;

        private NSMutableData bitmapData;
        private CGDataProvider dataProvider;

#pragma warning disable IDE0052 // Not Used
        private SkiaSharp.SKImageInfo info;
#pragma warning restore IDE0052 

        private bool designMode;
        #region IComponent
        // for IComponent
        private event EventHandler DisposedInternal;

        ISite IComponent.Site { get; set; }
        event EventHandler IComponent.Disposed
        {
            add { DisposedInternal += value; }
            remove { DisposedInternal -= value; }
        }
        #endregion

        public ChartViewRenderer()
        {
            Initialize();
        }

        void Initialize()
        {
            designMode = ((IComponent)this).Site?.DesignMode == true;

            AddObserver(this, keyPath: boundsPath, options: NSKeyValueObservingOptions.New, context: IntPtr.Zero);
            AddObserver(this, keyPath: framePath, options: NSKeyValueObservingOptions.New, context: IntPtr.Zero);
        }

        public override void AwakeFromNib()
        {
            Initialize();
        }

        /// <summary>
        /// Overriding Set size
        /// </summary>
        /// <param name="size"></param>
        void IVisualElementRenderer.SetElementSize(Xamarin.Forms.Size size)
        {
            Element.OnSizeChanged((float)size.Width, (float)size.Height);
            SetElementSize(size);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ICanvasController.IgnorePixelScaling))
            {
                Element?.Gesture?.SetScale(Element.IgnorePixelScaling ? 1f : ContentScaleFactor);
                SetNeedsDisplay();
                return;
            }
            if (e.PropertyName == nameof(IChartController.TouchEnabled))
            {
                UpdateGesture(Element);
                return;
            }
            if (e.PropertyName == nameof(IChartBase.Marker))
            {
                UpdateMarker();
                return;
            }
            base.OnElementPropertyChanged(sender, e);
        }

        void UpdateGesture(TElement element)
        {
            if (element == null)
                return;
            if (element.Gesture is Gestures.IChartGesture gesture)
            {
                if (element.TouchEnabled)
                {
                    gesture.Attach(this);
                }
                else
                {
                    gesture.Detach(this);
                }
            }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<TElement> e)
        {
            if (e.OldElement != null)
            {
                var oldElement = e.OldElement;
                oldElement.SurfaceInvalidated -= SetNeedsDisplay;
                oldElement.Gesture?.Clear();
                oldElement.OnUnbind();
            }
            if (e.NewElement != null)
            {
                var newElement = e.NewElement;
                var gesture = newElement.Gesture;
                if (gesture != null)
                {
                    // clear previous if not
                    gesture.Clear();
                    gesture.OnInitialize(this, newElement.IgnorePixelScaling ? 1f : ContentScaleFactor);
                }

                newElement.SurfaceInvalidated += SetNeedsDisplay;
                UpdateMarker();
                SetNeedsDisplay();
            }
            base.OnElementChanged(e);
        }

        void UpdateMarker()
        {
            if (Element.Marker is Components.MarkerView marker)
            {
                MarkerViewRenderer renderer = Xamarin.Forms.Platform.iOS.Platform.GetRenderer(marker) as MarkerViewRenderer;
                if (renderer == null)
                {
                    renderer = new MarkerViewRenderer();
                    Xamarin.Forms.Platform.iOS.Platform.SetRenderer(marker, renderer);
                }
                renderer.SetElement(marker);
                renderer.UpdateMarkerLayout();
            }
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
            contentsBounds.Width *= scale;
            contentsBounds.Height *= scale;
            // apply a scale
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
                Element.OnSizeChanged((float)contentsBounds.Width, (float)contentsBounds.Height);
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
                Element?.Gesture?.Clear();
                RemoveObserver(this, keyPath: boundsPath);
                RemoveObserver(this, keyPath: framePath);
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
