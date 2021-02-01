using System;
using System.Collections.Generic;
using System.Text;

#if NETSTANDARD || SKIASHARP
using Point = SkiaSharp.SKPoint;
using Canvas = SkiaSharp.SKCanvas;
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
    public interface IFill
    {
        void Draw(Canvas c, Rect rect, byte alpha);
    }
}
