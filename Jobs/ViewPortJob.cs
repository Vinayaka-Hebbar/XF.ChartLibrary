using System;
using XF.ChartLibrary.Utils;
using XF.ChartLibrary.Charts;
#if __IOS__ || __TVOS__
using Point = CoreGraphics.CGPoint;
using Canvas = CoreGraphics.CGContext;
#elif __ANDROID__
using Point = Android.Graphics.PointF;
using Canvas = Android.Graphics.Canvas;
#elif NETSTANDARD
using Point = SkiaSharp.SKPoint;
using Canvas = SkiaSharp.SKCanvas;
#endif

namespace XF.ChartLibrary.Jobs
{
    public partial class ViewPortJob
    {
        protected float[] Points = new float[2];

        public readonly ViewPortHandler ViewPortHandler;
        public double XValue { get; protected set; }
        public double YValue { get; protected set; }
        public Transformer Transformer { get; }
        public IChartBase View { get; }

        public ViewPortJob(
           ViewPortHandler viewPortHandler,
          double xValue,
          double yValue,
          Transformer transformer,
          IChartBase view)
        {
            ViewPortHandler = viewPortHandler;
            XValue = xValue;
            YValue = yValue;
            Transformer = transformer;
            View = view;
        }

#if __IOS__ || __TVOS__
        public void DoJob() => Run();
#endif

        public virtual void Run()
        {
            throw new NotImplementedException("`Run()` must be overridden by subclasses");
        }
    }
}
