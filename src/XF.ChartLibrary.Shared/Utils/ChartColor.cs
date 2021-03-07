namespace XF.ChartLibrary.Utils
{
    public class ChartColor
    {
#if __IOS__ || __TVOS__
        public readonly UIKit.UIColor Value;

        public ChartColor(UIKit.UIColor value)
        {
            Value = value;
        }

        public ChartColor(double r, double g, double b, double a)
        {
            Value = new UIKit.UIColor((float)r, (float)g, (float)b, (float)a);
        }

#elif NETSTANDARD
        public readonly SkiaSharp.SKColor Value;

        public ChartColor(SkiaSharp.SKColor value)
        {
            Value = value;
        }
#endif
    }
}
