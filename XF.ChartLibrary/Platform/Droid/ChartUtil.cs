using Android.Views;

namespace XF.ChartLibrary
{
    partial class ChartUtil
    {
        private static float density;

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
                density = 1f;
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

        public static float Density
        {
            get => density;
            set => density = value;
        }

        public static float DpToPixel(this float self)
        {
            return self * density;
        }

        public static double DpToPixel(this double self)
        {
            return self * density;
        }

    }
}
