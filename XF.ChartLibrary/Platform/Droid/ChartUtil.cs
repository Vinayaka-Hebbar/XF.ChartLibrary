using Android.Views;
using System;
using System.Collections.Generic;
using System.Text;

namespace XF.ChartLibrary
{
    partial class ChartUtil
    {
        static readonly float density;
        internal static readonly int MaxFlingVelocity;

        internal static readonly int MinimumFlingVelocity;


        static ChartUtil()
        {
            var context = Android.App.Application.Context;
            if (context == null)
            {
                MinimumFlingVelocity = 50;
                MaxFlingVelocity = 8000;
                System.Diagnostics.Trace.TraceError("Android Context null; ChartUtil()");
            }
            else
            {
                ViewConfiguration config = ViewConfiguration.Get(context);
                MinimumFlingVelocity = config.ScaledMinimumFlingVelocity;
                MaxFlingVelocity = config.ScaledMaximumFlingVelocity;
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
