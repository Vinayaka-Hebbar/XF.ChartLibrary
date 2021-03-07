namespace XF.ChartLibrary.Charts
{
    public partial class PieChart
    {
        private float maxAngle = 360f;

        private float minAngleForSlices;
        private bool drawRoundedSlicesEnabled;
        private bool drawSlicesUnderHoleEnabled;
        private bool drawHoleEnabled = true;
        private bool usePercenValuesEnabled;

        private string centerText = string.Empty;
        private bool drawCenterTextEnabled = true;
        private float holeRadiusPercent = 50f;
        private float transparentCircleRadiusPercent = 55f;
        private float centerTextRadiusPercent = 100f;

        /// <summary>
        /// Gets or Sets the max angle that is used for calculating the pie-circle. 360f means
        /// it's a full PieChart, 180f results in a half-pie-chart. Default: 360f
        /// </summary>
        public float MaxAngle
        {
            get => maxAngle;
            set
            {
                if (value > 360)
                    value = 360f;

                if (value < 90)
                    value = 90f;
                maxAngle = value;
            }
        }

        /// <summary>
        /// Get or Set the angle to set minimum size for slices, you must call <see cref="ChartBase{TData, TDataSet}.NotifyDataSetChanged()"/>
        /// and <see cref="ChartBase{TData, TDataSet}.InvalidateSurface()"/> when changing this, only works if there is enough room for all
        /// slices to have the minimum angle.
        /// </summary>
        public float MinAngleForSlices
        {
            get => minAngleForSlices;
            set
            {
                if (value > (maxAngle / 2f))
                    value = maxAngle / 2f;
                else if (value < 0)
                    value = 0f;
                minAngleForSlices = value;
            }
        }

        /// <summary>
        /// If true, the slices of the piechart are rounded
        /// </summary>
        public bool DrawRoundedSlicesEnabled
        {
            get => drawRoundedSlicesEnabled;
            set => drawRoundedSlicesEnabled = value;
        }

        /// <summary>
        /// If true, the hole will see-through to the inner tips of the slices
        /// </summary>
        public bool DrawSlicesUnderHoleEnabled
        {
            get => drawSlicesUnderHoleEnabled;
            set => drawSlicesUnderHoleEnabled = value;
        }

        /// <summary>
        /// If true, the white hole inside the chart will be drawn
        /// </summary>
        public bool DrawHoleEnabled
        {
            get => drawHoleEnabled;
            set => drawHoleEnabled = value;
        }

        /// <summary>
        /// If this is enabled, values inside the PieChart are drawn in percent and
        /// not with their original value.Values provided for the IValueFormatter to
        /// format are then provided in percent.
        /// </summary>
        public bool UsePercenValuesEnabled
        {
            get => usePercenValuesEnabled;
            set => usePercenValuesEnabled = value;
        }

        /// <summary>
        /// Variable for the text that is drawn in the center of the pie-chart
        /// </summary>
        public string CenterText
        {
            get => centerText;
            set
            {
                if (value == null)
                    value = string.Empty;
                centerText = value;
            }
        }

        /// <summary>
        /// If enabled, centertext is drawn
        /// </summary>
        public bool DrawCenterTextEnabled
        {
            get => drawCenterTextEnabled;
            set => drawCenterTextEnabled = value;
        }

        /// <summary>
        /// Indicates the size of the hole in the center of the piechart, default:
        /// radius / 2
        /// </summary>
        public float HoleRadiusPercent
        {
            get => holeRadiusPercent;
            set => holeRadiusPercent = value;
        }

        /// <summary>
        /// The radius of the transparent circle next to the chart-hole in the center
        /// </summary>
        public float TransparentCircleRadiusPercent
        {
            get => transparentCircleRadiusPercent;
            set => transparentCircleRadiusPercent = value;
        }

        /// <summary>
        /// the rectangular radius of the bounding box for the center text, as a percentage of the pie
        /// hole
        /// default 1.f(100%)
        /// </summary>
        public float CenterTextRadiusPercent
        {
            get => centerTextRadiusPercent;
            set => centerTextRadiusPercent = value;
        }

        public override float RequiredLegendOffset => throw new System.NotImplementedException();
    }
}
