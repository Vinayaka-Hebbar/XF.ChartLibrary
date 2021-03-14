using Android.Graphics;
using Android.Util;
using Android.Views;
using XF.ChartLibrary.Utils;

namespace XF.ChartLibrary
{
    public static partial class ChartUtil
    {
        private static DisplayMetrics metrics;
        private static readonly int minimumFlingVelocity = 50;
        private static readonly int maximumFlingVelocity = 8000;

        [System.Obsolete]
        static ChartUtil()
        {
            var context = Android.App.Application.Context;
            if (context == null)
            {
                // noinspection deprecation
                minimumFlingVelocity = ViewConfiguration.MinimumFlingVelocity;
                // noinspection deprecation
                maximumFlingVelocity = ViewConfiguration.MaximumFlingVelocity;

                Log.Error("MPChartLib-Utils"
                        , "Utils.init(...) PROVIDED CONTEXT OBJECT IS NULL");

            }
            else
            {
                ViewConfiguration viewConfiguration = ViewConfiguration.Get(context);
                minimumFlingVelocity = viewConfiguration.ScaledMinimumFlingVelocity;
                maximumFlingVelocity = viewConfiguration.ScaledMaximumFlingVelocity;

                var res = context.Resources;
                metrics = res.DisplayMetrics;
            }
        }

        public static float DpToPixel(this float self)
        {
            if (metrics == null)
            {

                Log.Error("MPChartLib-Utils",
                        "Utils NOT INITIALIZED. You need to call Utils.init(...) at least once before" +
                                " calling Utils.convertDpToPixel(...). Otherwise conversion does not " +
                                "take place.");
                return self;
            }

            return self * metrics.Density;
        }

        public static Color WithAlpha(this Color self, byte a)
        {
            return new Color(self.A, self.R, self.B, a);
        }

        public static float LineHeight(this Paint self)
        {
            var matrics = self.GetFontMetrics();
            return matrics.Descent - matrics.Ascent;
        }

        public static float LineSpacing(this Paint self)
        {
            var matrics = self.GetFontMetrics();
            return matrics.Descent - matrics.Top + matrics.Bottom;
        }

        static Rect calcTextHeightRect = new Rect();

        public static ChartSize Measure(this Paint self, string text)
        {
            var r = calcTextHeightRect;
            self.GetTextBounds(text, 0, text.Length, r);
            return new ChartSize(r.Width(), r.Height());
        }

        public static float MeasureWidth(this Paint self, string text)
        {
            var r = calcTextHeightRect;
            self.GetTextBounds(text, 0, text.Length, r);
            return r.Width();
        }

        public static float MeasureHeight(this Paint self, string text)
        {
            var r = calcTextHeightRect;
            self.GetTextBounds(text, 0, text.Length, r);
            return r.Height();
        }
    }
}
