using System.Windows;

namespace XF.ChartLibrary.Charts
{
    partial class PieRadarChartBase<TData, TDataSet>
    {
        public static readonly DependencyProperty RotationEnabledProperty = DependencyProperty.Register(nameof(RotationEnabled), typeof(bool), typeof(PieRadarChartBase<TData, TDataSet>), new PropertyMetadata(true));

        public bool RotationEnabled
        {
            get => (bool)GetValue(RotationEnabledProperty);
            set => SetValue(RotationEnabledProperty, value);
        }
    }
}
