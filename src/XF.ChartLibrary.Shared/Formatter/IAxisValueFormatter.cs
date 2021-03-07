using System;
using XF.ChartLibrary.Components;

namespace XF.ChartLibrary.Formatter
{
    /// <summary>
    /// Created by Philipp Jahoda on 20/09/15.
    ///Custom formatter interface that allows formatting of
    /// axis labels before they are being drawn.
    /// </summary>
    public interface IAxisValueFormatter
    {
        /// <summary>
        ///  Called when a value from an axis is to be formatted
        /// before being drawn.For performance reasons, avoid excessive calculations
        /// and memory allocations inside this method.
        /// </summary>
        /// <param name="value">the value to be formatted</param>
        /// <param name="axis">the axis the value belongs to</param>
        /// <returns></returns>
        String GetFormattedValue(float value, AxisBase axis);
    }
}
