namespace XF.ChartLibrary.Components
{
    /// <summary>
    /// Class representing the x-axis labels settings. Only use the setter methods to
    /// modify it.Do not access public variables directly.Be aware that not all
    /// features the XLabels class provides are suitable for the RadarChart.
    /// </summary>
    public class XAxis : AxisBase
    {
        /**
         * width of the x-axis labels in pixels - this is automatically
         * calculated by the computeSize() methods in the renderers
         */
        public int LabelWidth { get; set; } = 1;

        /**
         * height of the x-axis labels in pixels - this is automatically
         * calculated by the computeSize() methods in the renderers
         */
        public int LabelHeight { get; set; } = 1;

        /**
         * width of the (rotated) x-axis labels in pixels - this is automatically
         * calculated by the computeSize() methods in the renderers
         */
        public int LabelRotatedWidth { get; set; } = 1;

        /**
         * height of the (rotated) x-axis labels in pixels - this is automatically
         * calculated by the computeSize() methods in the renderers
         */
        public int LabelRotatedHeight { get; set; } = 1;

        /**
         * This is the angle for drawing the X axis labels (in degrees)
         */
        public float LabelRotationAngle { get; set; } = 0f;

        /**
         * if set to true, the chart will avoid that the first and last label entry
         * in the chart "clip" off the edge of the chart
         */
        public bool AvoidFirstLastClipping { get; set; } = false;

        /**
         * the position of the x-labels relative to the chart
         */
        public XAxisPosition Position { get; set; } = XAxisPosition.Top;

        /// <summary>
        /// enum for the position of the x-labels relative to the chart
        /// </summary>
        public enum XAxisPosition
        {
            Top, Bottom, BothSided, TopInside, BottomInside
        }

        public XAxis()
        {
            YOffset = 4.0f; // -3
        }
    }
}
