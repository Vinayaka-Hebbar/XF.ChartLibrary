using CoreGraphics;
using System.Collections.Generic;

namespace XF.ChartLibrary.Utils
{
    public class DashPathEffect
    {
        /// This is how much (in pixels) into the dash pattern are we starting from.
        public float LineDashPhase { get; set; }

        /// This is the actual dash pattern.
        /// I.e. [2, 3] will paint [--   --   ]
        /// [1, 3, 4, 2] will paint [-   ----  -   ----  ]
        public IList<float> LineDashLengths { get; }

        /// Line cap type, default is CGLineCap.Butt
        public CGLineCap LineCapType { get; } = CGLineCap.Butt;
    }
}
