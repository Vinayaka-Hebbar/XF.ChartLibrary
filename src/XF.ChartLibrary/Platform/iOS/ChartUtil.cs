using SkiaSharp;
using System;
using UIKit;

namespace XF.ChartLibrary
{
    partial class ChartUtil
    {
        static readonly float density;

        static ChartUtil()
        {
            density = (float)UIScreen.MainScreen.Scale;
        }

        public static float DpToPixel(this float self)
        {
            return self * density;
        }
    }
}
