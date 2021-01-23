using SkiaSharp;
using System.Collections.Generic;

namespace XF.ChartLibrary.Components
{
    public partial class AxisBase
    {
        public SKColor LabelTextColor { get; set; } = SKColors.Black;
        public SKColor GridColor { get; set; } = SKColors.Gray.WithAlpha(240);
        public SKColor AxisLineColor { get; set; } = SKColors.Gray;

        private SKPathEffect axisLineDashPathEffect = null;

        private SKPathEffect gridDashPathEffect = null;

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
        ///  Enables the grid line to be drawn in dashed mode, e.g.like this
        /// "- - - - - -". THIS ONLY WORKS IF HARDWARE-ACCELERATION IS TURNED OFF.
        /// Keep in mind that hardware acceleration boosts performance.
        /// </summary>
        public SKPathEffect GridDashedLine
        {
            get => gridDashPathEffect;
            set => gridDashPathEffect = value;
        }

        /// <summary>
        /// Disables the grid line to be drawn in dashed mode.
        /// </summary>
        public void DisableGridDashedLine()
        {
            gridDashPathEffect = null;
        }

        /// <summary>
        /// Returns true if the grid dashed-line effect is enabled, false if not.
        /// </summary>
        public bool IsGridDashedLineEnabled
        {
            get => gridDashPathEffect != null;
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

        /// <summary>
        ///  Enables the axis line to be drawn in dashed mode, e.g.like this
        /// "- - - - - -". THIS ONLY WORKS IF HARDWARE-ACCELERATION IS TURNED OFF.
        /// Keep in mind that hardware acceleration boosts performance.
        /// </summary>
        public SKPathEffect AxisLineDashedLine
        {
            get => axisLineDashPathEffect;
            set => axisLineDashPathEffect = value;
        }

        /// <summary>
        /// Disables the axis line to be drawn in dashed mode.
        /// </summary>
        public void DisableAxisLineDashedLine()
        {
            axisLineDashPathEffect = null;
        }

        /// <summary>
        /// Returns true if the axis dashed-line effect is enabled, false if not.
        /// </summary>
        public bool IsAxisLineDashedLineEnabled
        {
            get => axisLineDashPathEffect != null;
        }

    }
}
