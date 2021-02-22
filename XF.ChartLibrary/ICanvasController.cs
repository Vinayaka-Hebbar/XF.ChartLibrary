using SkiaSharp;
using System;
using Xamarin.Forms;

namespace XF.ChartLibrary
{
    public interface ICanvasController : IVisualElementController
    {
        public event Action SurfaceInvalidated;

        bool IgnorePixelScaling { get; }

        Gestures.IChartGesture Gesture { get; }

        void OnSizeChanged(float w, float h);

        void OnPaintSurface(SKSurface surface, SKImageInfo e);
    }

    public interface IChartController : ICanvasController, Charts.IChartBase
    {
    }
}
