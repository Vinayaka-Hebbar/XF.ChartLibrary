using System;

namespace XF.ChartLibrary.Platform.iOS
{
    public static class Extensions
    {
        public static void SetLocation(this Gestures.TouchEvent self, CoreGraphics.CGPoint location, nfloat scale)
        {
            self.x = (float)(location.X * scale);
            self.y = (float)(location.Y * scale);
        }
    }
}
