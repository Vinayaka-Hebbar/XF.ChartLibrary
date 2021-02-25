using System.Windows;

namespace XF.ChartLibrary.Charts
{
    partial class BarChart
    {
        public static readonly DependencyProperty IsDrawBarShadowProperty = DependencyProperty.Register(nameof(IsDrawBarShadow), typeof(bool), typeof(BarChart));

        public static readonly DependencyProperty IsDrawValueAboveBarProperty = DependencyProperty.Register(nameof(IsDrawValueAboveBar), typeof(bool), typeof(BarChart), new PropertyMetadata(true));

        public static readonly DependencyProperty IsHighlightFullBarProperty = DependencyProperty.Register(nameof(IsHighlightFullBar), typeof(bool), typeof(BarChart));

        public static readonly DependencyProperty FitBarsProperty = DependencyProperty.Register(nameof(FitBars), typeof(bool), typeof(BarChart));

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
