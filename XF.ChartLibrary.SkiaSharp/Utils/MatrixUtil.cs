using SkiaSharp;

namespace XF.ChartLibrary.Utils
{
    public static partial class MatrixUtil
    {
        public static SKMatrix CreateIdentity() => new SKMatrix { ScaleX = 1, ScaleY = 1, Persp2 = 1 };
    }
}
