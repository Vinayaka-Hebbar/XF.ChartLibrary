using Android.Content;
using Android.Views;
using System.ComponentModel;
using Xamarin.Forms.Platform.Android;
using XF.ChartLibrary.Charts;
using XF.ChartLibrary.Interfaces;
using XF.ChartLibrary.Interfaces.DataSets;
using NativeView = SkiaSharp.Views.Android.SKCanvasView;

namespace XF.ChartLibrary.Platform.Droid
{
    public class ChartViewBaseRenderer<TData, TDataSet> : ViewRenderer<ChartBase<TData, TDataSet>, NativeView>
        where TData : IChartData<TDataSet> where TDataSet : IDataSet, IBarLineScatterCandleBubbleDataSet
    {
        Gestures.ChartGestureRecognizer gestureRecognizer;
        private GestureDetector gestureDetector;

        protected ChartViewBaseRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<ChartBase<TData, TDataSet>> e)
        {
            if (e.OldElement != null)
            {
                var oldElement = e.OldElement;
                oldElement.SurfaceInvalidated -= Control.Invalidate;
                oldElement.ChartGesture.Detach();
            }
            if (e.NewElement != null)
            {
                var newElement = e.NewElement;
                if (Control == null)
                {
                    NativeView native = CreateNativeView();
                    SetNativeControl(native);
                }
                gestureRecognizer = newElement.ChartGesture;
                gestureDetector = new GestureDetector(Context, newElement.ChartGesture);
                newElement.SurfaceInvalidated += Control.Invalidate;
                Invalidate();
            }
            base.OnElementChanged(e);
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
            int size = (int)50f.DpToPixel();
            SetMeasuredDimension(System.Math.Max(SuggestedMinimumWidth,
                            ResolveSize(size,
                                    widthMeasureSpec)),
                    System.Math.Max(SuggestedMinimumHeight,
                            ResolveSize(size,
                                    heightMeasureSpec)));
        }

        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            Element.OnSizeChanged(w, h);
            base.OnSizeChanged(w, h, oldw, oldh);
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            if (gestureRecognizer != null)
            {
                if (gestureRecognizer.NotInUse)
                    gestureDetector.OnTouchEvent(e);
                return gestureRecognizer.OnTouch(this, e);
            }
            return false;
        }

        void OnPaintSurface(object sender, SkiaSharp.Views.Android.SKPaintSurfaceEventArgs e)
        {
            Element?.OnPaintSurface(e.Surface, e.Info);
        }

        protected virtual NativeView CreateNativeView()
        {
            NativeView view = new NativeView(Context);
            view.PaintSurface += OnPaintSurface;
            return view;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (gestureRecognizer != null)
                {
                    gestureRecognizer.Detach();
                    gestureDetector.Dispose();
                    gestureRecognizer = null;
                }
                if (Control != null)
                {
                    Control.PaintSurface -= OnPaintSurface;
                    if (Element != null)
                        Element.SurfaceInvalidated -= Control.Invalidate;
                }
            }
            base.Dispose(disposing);
        }
    }
}
