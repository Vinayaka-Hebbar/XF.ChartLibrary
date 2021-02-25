using Xamarin.Forms;

namespace XF.ChartLibrary.Charts
{
    partial class BarChart
    {
        public static readonly BindableProperty IsDrawBarShadowProperty = BindableProperty.Create(nameof(IsDrawBarShadow), typeof(bool), typeof(BarChart));

        public static readonly BindableProperty IsDrawValueAboveBarProperty = BindableProperty.Create(nameof(IsDrawValueAboveBar), typeof(bool), typeof(BarChart), defaultValue: true);

        public static readonly BindableProperty IsHighlightFullBarProperty = BindableProperty.Create(nameof(IsHighlightFullBar), typeof(bool), typeof(BarChart));

        public static readonly BindableProperty FitBarsProperty = BindableProperty.Create(nameof(FitBars), typeof(bool), typeof(BarChart));

        /// <summary>
        ///  Adds half of the bar width to each side of the x-axis range in order to allow the bars of the barchart to be
        /// fully displayed. Default: false
        /// </summary>
        public bool FitBars
        {
            get => (bool)GetValue(FitBarsProperty);
            set => SetValue(FitBarsProperty, value);
        }

        /// <summary>
        /// true if drawing shadows (maxvalue) for each bar is enabled, false if not
        /// </summary>
        public bool IsDrawBarShadow
        {
            get => (bool)GetValue(IsDrawBarShadowProperty);
            set => SetValue(IsDrawBarShadowProperty, value);
        }

        /// <summary>
        /// true if drawing values above bars is enabled, false if not
        /// </summary>
        public bool IsDrawValueAboveBar
        {
            get => (bool)GetValue(IsDrawValueAboveBarProperty);
            set => SetValue(IsDrawValueAboveBarProperty, value);
        }

        /// <summary>
        /// the highlight operation is be full-bar oriented, false if single-value
        /// </summary>
        public bool IsHighlightFullBar
        {
            get => (bool)GetValue(IsHighlightFullBarProperty);
            set => SetValue(IsHighlightFullBarProperty, value);
        }
    }
}
