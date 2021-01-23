using CoreGraphics;
using System.Collections.Generic;
using UIKit;
using XF.ChartLibrary.Utils;

namespace XF.ChartLibrary.Components
{
    public partial class AxisBase
    {
        public UIFont LabelFont { get; set; } = UIFont.SystemFontOfSize(10.0f);
        public UIColor LabelTextColor { get; set; } = Constants.LabelOrBlack;
        public UIColor GridColor { get; set; } = UIColor.Gray.ColorWithAlpha(0.9f);
        public UIColor AxisLineColor { get; set; } = UIColor.Gray;

        public float AxisLineDashPhase { get; set; }
        public IList<float> AxisLineDashLengths { get; set; }

        public float GridLineDashPhase { get; set; } = (0.0f);
        public IList<float> GridLineDashLengths { get; set; }

        public CGLineCap GridLineCap { get; set; } = CGLineCap.Butt;


        public AxisBase()
        {
            LimitLines = new List<LimitLine>();
        }
    }
}
