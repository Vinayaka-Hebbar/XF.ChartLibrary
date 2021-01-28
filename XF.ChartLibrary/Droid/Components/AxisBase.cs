using Android.Graphics;
using System.Collections.Generic;

namespace XF.ChartLibrary.Components
{
    public partial class AxisBase
    {
        public Color LabelTextColor { get; set; } = Color.Black;
        public Color GridColor { get; set; } = Color.Gray.WithAlpha(240);
        public Color AxisLineColor { get; set; } = Color.Gray;

        public AxisBase()
        {
            yOffset = xOffset = 5f.DpToPixel();
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
            gridDashPathEffect = new DashPathEffect(new float[]{
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
            axisLineDashPathEffect = new DashPathEffect(new float[]{
                lineLength, spaceLength
            }, phase);
        }

        

    }
}
