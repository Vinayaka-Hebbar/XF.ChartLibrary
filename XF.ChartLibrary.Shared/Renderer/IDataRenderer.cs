
#if NETSTANDARD || SKIASHARP
using Canvas = SkiaSharp.SKCanvas;
#elif __IOS__ || __TVOS__
using Canvas = CoreGraphics.CGContext;
#elif __ANDROID__
using Canvas = Android.Graphics.Canvas;
#endif

namespace XF.ChartLibrary.Renderer
{
    public interface IDataRenderer : IRenderer
    {
        void DrawValues(Canvas c);
        void DrawData(Canvas c);
        void DrawExtras(Canvas c);
        bool IsDrawingValuesAllowed(Interfaces.DataProvider.IChartProvider dataProvider);
        void DrawHighlighted(Canvas c, Highlight.Highlight[] indices);
    }
}
