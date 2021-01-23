using UIKit;
using XF.ChartLibrary.Utils;

namespace XF.ChartLibrary.Components
{
    public abstract partial class ComponentBase
    {
        public UIFont Font { get; set; } = UIFont.SystemFontOfSize(10.0f);

        public UIColor TextColor { get; set; } = Constants.LabelOrBlack;

        public float TextSize => (float)Font.PointSize;
    }
}
