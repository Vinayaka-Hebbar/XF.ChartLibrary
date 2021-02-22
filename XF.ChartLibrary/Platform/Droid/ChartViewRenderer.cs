using Android.Content;
using Android.Graphics;
using Android.Views;
using System;
using System.ComponentModel;
using Xamarin.Forms.Platform.Android;
using ABitmap = Android.Graphics.Bitmap;
using AView = Android.Views.View;
using ALayoutDirection = Android.Views.LayoutDirection;

namespace XF.ChartLibrary.Platform.Droid
{
    /// <summary>
    /// Fast Renderer for Chart
    /// </summary>
    /// <typeparam name="TElement">Chart Controller Element</typeparam>
    public class ChartViewRenderer<TElement> : AView, IVisualElementRenderer,
        IViewRenderer,
        AView.IOnFocusChangeListener,
        Xamarin.Forms.IEffectControlProvider
        where TElement : Xamarin.Forms.VisualElement, IChartController
    {
        private ABitmap bitmap;
        private TElement element;
        private SkiaSharp.SKImageInfo info;
        private VisualElementTracker _tracker;
        private Gestures.IChartGesture gesture;
        private int? _defaultLabelFor;
        private bool _disposed;

        public event EventHandler<VisualElementChangedEventArgs> ElementChanged;
        public event EventHandler<PropertyChangedEventArgs> ElementPropertyChanged;

        public Xamarin.Forms.VisualElement Element => element;

        VisualElementTracker IVisualElementRenderer.Tracker => _tracker;

        ViewGroup IVisualElementRenderer.ViewGroup => null;

        public AView View => this;

        public ChartViewRenderer(Context context) : base(context)
        {
            Initialize();
        }

        protected void Initialize()
        {
            OnFocusChangeListener = this;
        }

        protected virtual void OnElementChanged(TElement oldElement, TElement newElement)
        {
            ElementChanged?.Invoke(this, new VisualElementChangedEventArgs(oldElement, newElement));
        }

        #region Update Properies
        void UpdateFlowDirection()
        {
            if (_disposed)
                return;
            // if android:targetSdkVersion < 17 setting these has no effect
            if (Xamarin.Forms.EffectiveFlowDirectionExtensions.IsRightToLeft(element.EffectiveFlowDirection))
            {
                LayoutDirection = ALayoutDirection.Rtl;
            }
            else if (Xamarin.Forms.EffectiveFlowDirectionExtensions.IsRightToLeft(element.EffectiveFlowDirection))
            {
                LayoutDirection = ALayoutDirection.Ltr;
            }
        }

        void UpdateIsEnabled()
        {
            if (element == null || _disposed)
            {
                return;
            }

            Enabled = Element.IsEnabled;
        }

        void UpdateBackgroundColor()
        {
            SetBackgroundColor(Element.BackgroundColor.ToAndroid());
        }

        void UpdateBackground()
        {
            this.UpdateBackground(Element.Background);
        }
        #endregion

        protected virtual void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ICanvasController.IgnorePixelScaling))
            {
                UpdateCanvasSize(Width, Height);
                Invalidate();
                return;
            }
            if (e.PropertyName == nameof(Xamarin.Forms.VisualElement.BackgroundColor))
            {
                UpdateBackgroundColor();
                return;
            }
            if (e.PropertyName == nameof(Xamarin.Forms.VisualElement.Background))
            {
                UpdateBackground();
                return;
            }
            if (e.PropertyName == nameof(Xamarin.Forms.VisualElement.FlowDirection))
            {
                UpdateFlowDirection();
                return;
            }
            if (e.PropertyName == nameof(Xamarin.Forms.VisualElement.IsEnabled))
            {
                UpdateIsEnabled();
                return;
            }
            ElementPropertyChanged?.Invoke(this, e);
        }

        protected virtual void OnFocusChangeRequested(object sender, Xamarin.Forms.VisualElement.FocusRequestArgs e)
        {
            e.Result = true;

            if (e.Focus)
            {
                // Android does the actual focus/unfocus work on the main looper
                // So in case we're setting the focus in response to another control's un-focusing,
                // we need to post the handling of it to the main looper so that it happens _after_ all the other focus
                // work is done; otherwise, a call to ClearFocus on another control will kill the focus we set here
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                {
                    RequestFocus();
                });
            }
            else
            {
                ClearFocus();
            }
        }

        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            element.OnSizeChanged(w, h);
            base.OnSizeChanged(w, h, oldw, oldh);
            UpdateCanvasSize(w, h);
        }

        public override void Draw(Canvas canvas)
        {
            canvas.ClipShape(Context, Element);
            base.Draw(canvas);
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            if (gesture != null)
            {
                return gesture.OnTouch(this, e);
            }
            return false;
        }

        #region OnDraw
        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);// bail out if the view is not actually visible
            if (Visibility != ViewStates.Visible)
            {
                FreeBitmap();
                return;
            }

            // create a skia surface
            var surface = CreateSurface(out var info);
            if (surface == null)
                return;

            // draw using SkiaSharp
            element?.OnPaintSurface(surface, info);

            // draw the surface to the view
            // clean up skia objects
            surface.Canvas.Flush();
            surface.Dispose();

            // get the bitmap ready for drawing
            bitmap.UnlockPixels();

            // get the bounds
            var src = new Rect(0, 0, info.Width, info.Height);
            var dst = new RectF(0, 0, canvas.Width, canvas.Height);

            // draw bitmap to the view canvas
            canvas.DrawBitmap(bitmap, src, dst, null);
        }

        private SkiaSharp.SKSurface CreateSurface(out SkiaSharp.SKImageInfo info)
        {
            // get context details
            info = this.info;

            // if there are no pixels, clean up and return
            if (info.Width == 0 || info.Height == 0)
            {
                Dispose();
                return null;
            }

            // if the memory size has changed, then reset the underlying memory
            if (bitmap?.Handle == IntPtr.Zero || bitmap?.Width != info.Width || bitmap?.Height != info.Height)
                FreeBitmap();

            // create the bitmap data if we need it
            if (bitmap == null)
                bitmap = ABitmap.CreateBitmap(info.Width, info.Height, ABitmap.Config.Argb8888);

            return SkiaSharp.SKSurface.Create(info, bitmap.LockPixels(), info.RowBytes);
        }

        #endregion

        #region Update Canvas Size
        protected void UpdateCanvasSize(int w, int h, float density = 1f)
        {
            if (density != 1)
                info = new SkiaSharp.SKImageInfo((int)(w / density), (int)(h / density), SkiaSharp.SKColorType.Rgba8888, SkiaSharp.SKAlphaType.Premul);
            else
                info = new SkiaSharp.SKImageInfo(w, h, SkiaSharp.SKColorType.Rgba8888, SkiaSharp.SKAlphaType.Premul);

            // if there are no pixels, clean up
            if (info.Width == 0 || info.Height == 0)
                FreeBitmap();
        }
        #endregion

        #region Free Bitmap

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
        #endregion

        protected override void OnDetachedFromWindow()
        {
            FreeBitmap();
            info = SkiaSharp.SKImageInfo.Empty;
            base.OnDetachedFromWindow();
        }

        protected override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();

            UpdateCanvasSize(Width, Height);
            Invalidate();
        }

        public void MeasureExactly()
        {
            var element = Element;
            if (element == null)
            {
                return;
            }

            var width = element.Width;
            var height = element.Height;

            if (width <= 0 || height <= 0)
            {
                return;
            }

            var realWidth = (int)width.DpToPixel();
            var realHeight = (int)height.DpToPixel();
            Measure(realWidth + (int)MeasureSpecMode.Exactly, realHeight + (int)MeasureSpecMode.Exactly);
        }

        public void OnFocusChange(AView v, bool hasFocus)
        {
            ((Xamarin.Forms.IElementController)Element).SetValueFromRenderer(Xamarin.Forms.VisualElement.IsFocusedPropertyKey, hasFocus);
        }

        public Xamarin.Forms.SizeRequest GetDesiredSize(int widthConstraint, int heightConstraint)
        {
            Measure(widthConstraint, heightConstraint);
            return new Xamarin.Forms.SizeRequest(new Xamarin.Forms.Size(MeasuredWidth, MeasuredHeight));
        }

        public void SetElement(Xamarin.Forms.VisualElement element)
        {
            var oldElement = this.element;
            if (oldElement != null)
            {
                oldElement.SurfaceInvalidated -= Invalidate;
                oldElement.FocusChangeRequested -= OnFocusChangeRequested;
                oldElement.PropertyChanged -= OnElementPropertyChanged;
                oldElement.Gesture.Dispose();

                if (element.BackgroundColor != oldElement.BackgroundColor)
                    UpdateBackgroundColor();
            }
            var newElement = element as TElement;
            this.element = newElement;
            gesture = newElement.Gesture;
            newElement.SurfaceInvalidated += Invalidate;
            newElement.FocusChangeRequested += OnFocusChangeRequested;
            UpdateFlowDirection();
            UpdateIsEnabled();
            if (newElement.Background != null)
                UpdateBackground();
            element.PropertyChanged += OnElementPropertyChanged;
            gesture.OnInitialize(this);
            if (_tracker == null)
            {
                // Can't set up the tracker in the constructor because it access the Element (for now)
                _tracker = new VisualElementTracker(this);
            }
            if (!string.IsNullOrEmpty(element.StyleId))
            {
                ContentDescription = element.StyleId;
            }
            Invalidate();
            OnElementChanged(oldElement, newElement);
        }

        public void SetLabelFor(int? id)
        {
            if (_defaultLabelFor == null)
            {
                _defaultLabelFor = AndroidX.Core.View.ViewCompat.GetLabelFor(this);
            }

            AndroidX.Core.View.ViewCompat.SetLabelFor(this, (int)(id ?? _defaultLabelFor));
        }

        public void UpdateLayout()
        {
            _tracker?.UpdateLayout();
        }

        public void RegisterEffect(Xamarin.Forms.Effect effect)
        {
            if (effect is PlatformEffect platformEffect)
            {
                platformEffect.SetControl(this);
                platformEffect.SetContainer(null);
            }
        }

        #region Dispose
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _disposed = true;
                if (element != null)
                    element.SurfaceInvalidated -= Invalidate;

                OnFocusChangeListener = null;
                _tracker?.Dispose();
            }
            FreeBitmap();
            info = SkiaSharp.SKImageInfo.Empty;
            base.Dispose(disposing);
        }
        #endregion
    }
}
