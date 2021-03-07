using SkiaSharp;
using System;
using XF.ChartLibrary.Components;

namespace Sample
{
    public static class Icons
    {
        private static Lazy<SKPath> StarInstance = new Lazy<SKPath>(() =>
        {
            var source = new SKPath();
            source.MoveTo(12, 2);
            source.AddPoly(new SKPoint[] { new SKPoint(12, 2), new SKPoint(15.09F, 8.26F), new SKPoint(22, 9.27F), new SKPoint(17, 14.14F), new SKPoint(18.18F, 21.02F), new SKPoint(12, 17.77F), new SKPoint(5.82F, 21.02F), new SKPoint(7, 14.14F), new SKPoint(2, 9.27F), new SKPoint(8.91F, 8.26F), new SKPoint(12, 2), }, true);
            return source;
        });

        public static SKPath Star => StarInstance.Value;
    }
}
