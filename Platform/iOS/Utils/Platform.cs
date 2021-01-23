using CoreAnimation;
using System;

namespace XF.ChartLibrary.Utils
{
#if __IOS__ || __TVOS__
    using UIKit;
    using NSUIScreen = UIKit.UIScreen;
    using Color = UIKit.UIColor;

    public static class Constants
    {
        public static readonly Color LabelOrBlack = FetchLabelColor();

        static Color FetchLabelColor()
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
            {
                return Color.LabelColor;
            }
            return Color.Black;
        }
    }

    public static class Extenstion
    {
        public static (nfloat red, nfloat green, nfloat blue, nfloat alpha) ToRgba(this UIColor color)
        {
            color.GetRGBA(out nfloat red, out nfloat green, out nfloat blue, out nfloat alpha);
            return (red, green, blue, alpha);
        }

        public static bool IsScrollEnabled(this UIScrollView view)
        {
            return view.ScrollEnabled;
        }

        public static void IsScrollEnabled(this UIScrollView view, bool value)
        {
            view.ScrollEnabled = value;
        }

        public static nfloat UIScale(this UIScreen self)
        {
            return self.Scale;
        }

        public static NSUIScreen MainScreen() => NSUIScreen.MainScreen;
    }

    public class NSUIView : UIView
    {
        public CALayer NSUILayer => Layer;
    }

#endif
}
