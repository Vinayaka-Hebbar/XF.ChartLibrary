using System;
using System.Collections.Generic;
using System.Text;

namespace XF.ChartLibrary
{
    partial class ChartUtil
    {
        static readonly float density;

        static ChartUtil()
        {
            var context = Android.App.Application.Context;
            if (context == null)
            {

            }
            else
            {
                var res = context.Resources;
                density = res.DisplayMetrics.Density;
            }
        }

        public static float DpToPixel(this float self)
        {
            return self * density;
        }
    }
}
