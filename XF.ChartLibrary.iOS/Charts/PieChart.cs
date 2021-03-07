namespace XF.ChartLibrary.Charts
{
    partial class PieChart
    {
        private float maxAngle = 360f;

        private float minAngleForSlices;

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
    }
}
