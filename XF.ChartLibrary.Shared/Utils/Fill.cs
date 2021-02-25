
#if NETSTANDARD || SKIASHARP
using Canvas = SkiaSharp.SKCanvas;
using Point = SkiaSharp.SKPoint;
using Rect = SkiaSharp.SKRect;
using Path = SkiaSharp.SKPath;
using Paint = SkiaSharp.SKPaint;
#elif __IOS__ || __TVOS__
using Point = CoreGraphics.CGPoint;
using Canvas = CoreGraphics.CGContext;
using Rect = CoreGraphics.CGRect;
#elif __ANDROID__
using Point = Android.Graphics.PointF;
using Canvas = Android.Graphics.Canvas;
using Rect = Android.Graphics.Rect;
using Path = Android.Graphics.Path;
using Paint = Android.Graphics.Paint;
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
        void Draw(Canvas c, Path path, Paint paint, Rect rect);
#endif
    }

#if __ANDROID__ || SKIASHARP
    public interface IRectFill
    {
        void Draw(Canvas c, Paint paint, float left, float top, float right, float bottom,
                         FillDirection gradientDirection);
    } 
#endif
}
