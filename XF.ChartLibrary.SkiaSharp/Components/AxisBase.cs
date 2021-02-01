using SkiaSharp;
using System.Collections.Generic;

namespace XF.ChartLibrary.Components
{
    public partial class AxisBase
    {
        public SKColor LabelTextColor { get; set; } = SKColors.Black;
        public SKColor GridColor { get; set; } = SKColors.Gray.WithAlpha(240);
        public SKColor AxisLineColor { get; set; } = SKColors.Gray;

        public AxisBase()
        {
            LimitLines = new List<LimitLine>();
        }

        /// <summary>
        /// Enables the axis line to be drawn in dashed mode, e.g.like this
        /// "- - - - - -". THIS ONLY WORKS IF HARDWARE-ACCELERATION IS TURNED OFF.
        /// Keep in mind that hardware acceleration boosts performance.
        /// </summary>
        /// <param name="lineLength">the length of the line pieces</param>
        /// <param name="spaceLength">the length of space in between the pieces</param>
        /// <param name="phase">offset, in degrees (normally, use 0)</param>
        public void EnableGridDashedLine(float lineLength, float spaceLength, float phase)
        {
            gridDashPathEffect = SKPathEffect.CreateDash(new float[]{
                lineLength, spaceLength
            }, phase);
        }

        /// <summary>
        /// Enables the axis line to be drawn in dashed mode, e.g.like this
        /// "- - - - - -". THIS ONLY WORKS IF HARDWARE-ACCELERATION IS TURNED OFF.
        /// Keep in mind that hardware acceleration boosts performance.
        /// </summary>
        /// <param name="lineLength">the length of the line pieces</param>
        /// <param name="spaceLength">the length of space in between the pieces</param>
        /// <param name="phase">offset, in degrees (normally, use 0)</param>
        public void EnableAxisLineDashedLine(float lineLength, float spaceLength, float phase)
        {
            axisLineDashPathEffect = SKPathEffect.CreateDash(new float[]{
                lineLength, spaceLength
            }, phase);
        }

    }
}
