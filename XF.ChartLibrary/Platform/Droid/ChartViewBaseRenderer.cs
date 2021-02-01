using Android.Content;
using Xamarin.Forms.Platform.Android;
using XF.ChartLibrary.Charts;
using XF.ChartLibrary.Interfaces;
using XF.ChartLibrary.Interfaces.DataSets;
using NativeView = SkiaSharp.Views.Android.SKCanvasView;


namespace XF.ChartLibrary.Platform.Droid
{
    public abstract class ChartViewBaseRenderer<TData, TDataSet> : ViewRenderer<ChartBase<TData, TDataSet>, NativeView>
        where TData : IChartData<TDataSet> where TDataSet : IDataSet, IBarLineScatterCandleBubbleDataSet
    {
        protected ChartViewBaseRenderer(Context context) : base(context)
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<ChartBase<TData, TDataSet>> e)
        {
            if(e.OldElement != null)
            {
                var oldElement = e.OldElement;
                oldElement.SurfaceInvalidated -= OnSurfaceInvalidated;
            }
            if(e.NewElement != null)
            {
                var newElement = e.NewElement;
                if(Control == null)
                {
                    SetNativeControl(CreateNativeView());
                }
                newElement.SurfaceInvalidated += OnSurfaceInvalidated;
            }
            base.OnElementChanged(e);
        }

        void OnPaintSurface(object sender, SkiaSharp.Views.Android.SKPaintSurfaceEventArgs e)
        {
            Element?.OnPaintSurface(e.Surface, e.Info);
        }

        void OnSurfaceInvalidated()
        {
            Invalidate();
        }

        protected virtual NativeView CreateNativeView()
        {
            NativeView view = new NativeView(Context);
            view.PaintSurface += OnPaintSurface;
            return view;
        }
    }
}
