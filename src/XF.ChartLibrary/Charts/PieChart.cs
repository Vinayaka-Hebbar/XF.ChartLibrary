using Xamarin.Forms;

namespace XF.ChartLibrary.Charts
{
    public partial class PieChart
    {
        public static readonly BindableProperty DrawRoundedSlicesEnabledProperty = BindableProperty.Create(nameof(DrawRoundedSlicesEnabled), typeof(bool), typeof(PieChart));

        public static readonly BindableProperty DrawSlicesUnderHoleEnabledProperty = BindableProperty.Create(nameof(DrawSlicesUnderHoleEnabled), typeof(bool), typeof(PieChart));

        public static readonly BindableProperty DrawHoleEnabledProperty = BindableProperty.Create(nameof(DrawHoleEnabled), typeof(bool), typeof(PieChart), defaultValue: true);

        public static readonly BindableProperty DrawEntryLabelsProperty = BindableProperty.Create(nameof(DrawEntryLabels), typeof(bool), typeof(PieChart), defaultValue: true);

        public static readonly BindableProperty DrawCenterTextEnabledProperty = BindableProperty.Create(nameof(DrawCenterTextEnabled), typeof(bool), typeof(PieChart), defaultValue: true);

        public static readonly BindableProperty UsePercenValuesEnabledProperty = BindableProperty.Create(nameof(UsePercenValuesEnabled), typeof(bool), typeof(PieChart));

        public static readonly BindableProperty MaxAngleProperty = BindableProperty.Create(nameof(MaxAngle), typeof(float), typeof(PieChart), defaultValue: 360f, coerceValue: MaxAngleClamp);

        public static readonly BindableProperty MinAngleForSlicesProperty = BindableProperty.Create(nameof(MinAngleForSlices), typeof(float), typeof(PieChart), defaultValue: 0f, coerceValue: LimitMinAngleForSlices);

        public static readonly BindableProperty CenterTextProperty = BindableProperty.Create(nameof(CenterText), typeof(FormattedString), typeof(PieChart), defaultValue: null, coerceValue: (b, v) => v ?? (FormattedString)string.Empty);

        public static readonly BindableProperty HoleRadiusProperty = BindableProperty.Create(nameof(HoleRadius), typeof(float), typeof(PieChart), defaultValue: 50f);

        public static readonly BindableProperty TransparentCircleRadiusPercentProperty = BindableProperty.Create(nameof(TransparentCircleRadiusPercent), typeof(float), typeof(PieChart), defaultValue: 55f);

        public static readonly BindableProperty CenterTextRadiusPercentProperty = BindableProperty.Create(nameof(CenterTextRadiusPercent), typeof(float), typeof(PieChart), defaultValue: 100f);


        static object MaxAngleClamp(BindableObject bindable, object value)
        {
            var newValue = (float)value;
            if (newValue > 360)
                return 360f;

            if (newValue < 90)
                return 90f;
            return value;
        }

        static object LimitMinAngleForSlices(BindableObject bindable, object value)
        {
            var newValue = (float)value;
            var maxAngle = (float)bindable.GetValue(MaxAngleProperty);
            if (newValue > (maxAngle / 2f))
                return maxAngle / 2f;
            else if (newValue < 0)
                return 0f;
            return value;
        }

        public PieChart()
        {

        }

        /// <summary>
        /// If true, the slices of the piechart are rounded
        /// </summary>
        public bool DrawRoundedSlicesEnabled
        {
            get => (bool)GetValue(DrawRoundedSlicesEnabledProperty);
            set => SetValue(DrawRoundedSlicesEnabledProperty, value);
        }

        /// <summary>
        /// If true, the hole will see-through to the inner tips of the slices
        /// </summary>
        public bool DrawSlicesUnderHoleEnabled
        {
            get => (bool)GetValue(DrawSlicesUnderHoleEnabledProperty);
            set => SetValue(DrawSlicesUnderHoleEnabledProperty, value);
        }

        /// <summary>
        /// If true, the white hole inside the chart will be drawn
        /// </summary>
        public bool DrawHoleEnabled
        {
            get => (bool)GetValue(DrawHoleEnabledProperty);
            set => SetValue(DrawHoleEnabledProperty, value);
        }

        /// <summary>
        ///  Flag indicating if entry labels should be drawn or not
        /// </summary>
        public bool DrawEntryLabels
        {
            get => (bool)GetValue(DrawEntryLabelsProperty);
            set => SetValue(DrawEntryLabelsProperty, value);
        }

        /// <summary>
        /// If this is enabled, values inside the PieChart are drawn in percent and
        /// not with their original value.Values provided for the IValueFormatter to
        /// format are then provided in percent.
        /// </summary>
        public bool UsePercenValuesEnabled
        {
            get => (bool)GetValue(UsePercenValuesEnabledProperty);
            set => SetValue(UsePercenValuesEnabledProperty, value);
        }

        /// <summary>
        /// Gets or Sets the max angle that is used for calculating the pie-circle. 360f means
        /// it's a full PieChart, 180f results in a half-pie-chart. Default: 360f
        /// </summary>
        public float MaxAngle
        {
            get => (float)GetValue(MaxAngleProperty);
            set => SetValue(MaxAngleProperty, value);
        }

        /// <summary>
        /// Get or Set the angle to set minimum size for slices, you must call <see cref="ChartBase{TData, TDataSet}.NotifyDataSetChanged()"/>
        /// and <see cref="ChartBase{TData, TDataSet}.InvalidateSurface()"/> when changing this, only works if there is enough room for all
        /// slices to have the minimum angle.
        /// </summary>
        public float MinAngleForSlices
        {
            get => (float)GetValue(MinAngleForSlicesProperty);
            set => SetValue(MinAngleForSlicesProperty, value);
        }

        /// <summary>
        /// Variable for the text that is drawn in the center of the pie-chart
        /// </summary>
        public FormattedString CenterText
        {
            get => (FormattedString)GetValue(CenterTextProperty);
            set => SetValue(CenterTextProperty, value);
        }

        /// <summary>
        /// If enabled, centertext is drawn
        /// </summary>
        public bool DrawCenterTextEnabled
        {
            get => (bool)GetValue(DrawCenterTextEnabledProperty);
            set => SetValue(DrawCenterTextEnabledProperty, value);
        }

        /// <summary>
        /// Indicates the size of the hole in the center of the piechart, default:
        /// radius / 2
        /// </summary>
        public float HoleRadius
        {
            get => (float)GetValue(HoleRadiusProperty);
            set => SetValue(HoleRadiusProperty, value);
        }

        /// <summary>
        /// The radius of the transparent circle next to the chart-hole in the center
        /// </summary>
        public float TransparentCircleRadiusPercent
        {
            get => (float)GetValue(TransparentCircleRadiusPercentProperty);
            set => SetValue(TransparentCircleRadiusPercentProperty, value);
        }

        /// <summary>
        /// the rectangular radius of the bounding box for the center text, as a percentage of the pie
        /// hole
        /// default 1.f(100%)
        /// </summary>
        public float CenterTextRadiusPercent
        {
            get => (float)GetValue(CenterTextRadiusPercentProperty);
            set => SetValue(CenterTextRadiusPercentProperty, value);
        }

        public override void OnUnbind()
        {
            if (renderer is Renderer.PieChartRenderer l)
            {
                l.ReleaseBitmap();
            }
        }
    }
}
