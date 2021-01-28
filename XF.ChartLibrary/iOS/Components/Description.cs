namespace XF.ChartLibrary.Components
{
    public partial class Description
    {
        public Description()
        {
#if __TVOS__
        // 23 is the smallest recommended font size on the TV
        Font = UIKit.UIFont.SystemFontOfSize(23);
#elif __OSX__
        Font = UIKit.UIFont.SystemFontOfSize(NSUIFont.SystemFontSize);
#else
            Font = UIKit.UIFont.SystemFontOfSize(8.0f);
#endif
        }
    }
}
