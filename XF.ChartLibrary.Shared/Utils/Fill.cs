
#if NETSTANDARD || SKIASHARP
using Canvas = SkiaSharp.SKCanvas;
using Point = SkiaSharp.SKPoint;
using Rect = SkiaSharp.SKRect;
#elif __IOS__ || __TVOS__
using Point = CoreGraphics.CGPoint;
using Canvas = CoreGraphics.CGContext;
using Rect = CoreGraphics.CGRect;
#elif __ANDROID__
using Point = Android.Graphics.PointF;
using Canvas = Android.Graphics.Canvas;
using Rect = Android.Graphics.Rect;
#endif
namespace XF.ChartLibrary.Utils
{
    public enum FillDirection
    {
        Down, Up, Right, Left
    }

    public interface IFill
    {
#if (__IOS__ || __TVOS__) && !SKIASHARP
        void Draw(Canvas c, Rect rect, byte alpha); 
#else
        void Draw(Canvas c, SkiaSharp.SKPath path, SkiaSharp.SKPaint paint, Rect rect);
#endif
    }
}
