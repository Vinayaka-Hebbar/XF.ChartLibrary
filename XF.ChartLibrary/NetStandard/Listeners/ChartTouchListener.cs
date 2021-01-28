using System;
using System.Collections.Generic;
using System.Text;
using XF.ChartLibrary.Charts;

namespace XF.ChartLibrary.Platform.NetStandard.Listeners
{
    public class ChartTouchListener
    {

        public void OnTouch(object sender, SkiaSharp.Views.Forms.SKTouchEventArgs e)
        {
            var chart = (IChartBase)sender;

        }
    }
}
