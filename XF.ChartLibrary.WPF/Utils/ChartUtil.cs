using System.Windows.Interop;
using System.Windows.Media;

namespace XF.ChartLibrary
{
    static partial class ChartUtil
    {
        private static readonly float density;

        static ChartUtil()
        {
            Matrix transformToDevice;
            using (var source = new HwndSource(new HwndSourceParameters()))
                transformToDevice = source.CompositionTarget.TransformToDevice;
            density = (float)transformToDevice.M11;
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
