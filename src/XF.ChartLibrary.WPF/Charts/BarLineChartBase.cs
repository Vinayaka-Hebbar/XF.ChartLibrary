using SkiaSharp;
using System.Windows;
using XF.ChartLibrary.Components;
using XF.ChartLibrary.Interfaces.DataSets;
using XF.ChartLibrary.Renderer;
using XF.ChartLibrary.Utils;

namespace XF.ChartLibrary.Charts
{
    partial class BarLineChartBase<TData, TDataSet>
    {
        public static readonly DependencyProperty DragXEnabledProperty = DependencyProperty.Register(nameof(DragXEnabled), typeof(bool), typeof(BarLineChartBase<TData, TDataSet>));

        public static readonly DependencyProperty DragYEnabledProperty = DependencyProperty.Register(nameof(DragYEnabled), typeof(bool), typeof(BarLineChartBase<TData, TDataSet>));

        public static readonly DependencyProperty ScaleXEnabledProperty = DependencyProperty.Register(nameof(ScaleXEnabled), typeof(bool), typeof(BarLineChartBase<TData, TDataSet>));

        public static readonly DependencyProperty ScaleYEnabledProperty = DependencyProperty.Register(nameof(ScaleYEnabled), typeof(bool), typeof(BarLineChartBase<TData, TDataSet>));

        public static readonly DependencyProperty AxisLeftProperty = DependencyProperty.Register(nameof(AxisLeft), typeof(YAxis), typeof(BarLineChartBase<TData, TDataSet>), new PropertyMetadata(new YAxis(YAxisDependency.Left)));

        public static readonly DependencyProperty AxisRightProperty = DependencyProperty.Register(nameof(AxisRight), typeof(YAxis), typeof(BarLineChartBase<TData, TDataSet>), new PropertyMetadata(new YAxis(YAxisDependency.Right)));

        public static readonly DependencyProperty MaxVisibleCountProperty = DependencyProperty.Register(nameof(MaxVisibleCount), typeof(int), typeof(BarLineChartBase<TData, TDataSet>), new PropertyMetadata(100));

        public static readonly DependencyProperty HighlightPerDragEnabledProperty = DependencyProperty.Register(nameof(HighlightPerDragEnabled), typeof(bool), typeof(BarLineChartBase<TData, TDataSet>), new PropertyMetadata(true));

        public static readonly DependencyProperty IsDrawGridBackgroundProperty = DependencyProperty.Register(nameof(IsDrawGridBackground), typeof(bool), typeof(BarLineChartBase<TData, TDataSet>));

        public static readonly DependencyProperty DrawBordersProperty = DependencyProperty.Register(nameof(DrawBorders), typeof(bool), typeof(BarLineChartBase<TData, TDataSet>));

        public static readonly DependencyProperty AutoScaleMinMaxEnabledProperty = DependencyProperty.Register(nameof(AutoScaleMinMaxEnabled), typeof(bool), typeof(BarLineChartBase<TData, TDataSet>));

        public static readonly DependencyProperty ClipDataToContentProperty = DependencyProperty.Register(nameof(ClipDataToContent), typeof(bool), typeof(BarLineChartBase<TData, TDataSet>), new PropertyMetadata(true));

        public static readonly DependencyProperty ClipValuesToContentProperty = DependencyProperty.Register(nameof(ClipValuesToContent), typeof(bool), typeof(BarLineChartBase<TData, TDataSet>));

        public static readonly DependencyProperty MinOffsetProperty = DependencyProperty.Register(nameof(MinOffset), typeof(float), typeof(BarLineChartBase<TData, TDataSet>), new PropertyMetadata(15.0f));

        private SKPoint decelerationVelocity = SKPoint.Empty;

        private SKPoint decelerationCurrentPoint = SKPoint.Empty;

        private SKMatrix savedMatrix = SKMatrix.CreateIdentity();

        private SKPoint touchStartPoint = SKPoint.Empty;

        private IDataSet closestDatasetToTouch;

        private float mSavedXDist = 1f;
        private float mSavedYDist = 1f;
        private float mSavedDist = 1f;

        public BarLineChartBase()
        {
            GridBackgroundPaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                // Color = SKColors.White;
                Color = new SKColor(240, 240, 240) // light
                                                   // grey
            };

            BorderPaint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = SKColors.Black,
                StrokeWidth = 1f.DpToPixel()
            };
            MouseLeftButtonUp += OnMouseLeftButtonUp;
        }

        private void OnMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
           
        }

        /// <summary>
        /// flag indicating if the grid background should be drawn or not
        /// </summary>
        public bool IsDrawGridBackground
        {
            get => (bool)GetValue(IsDrawGridBackgroundProperty);
            set => SetValue(IsDrawGridBackgroundProperty, value);
        }

        public bool DrawBorders
        {
            get => (bool)GetValue(DrawBordersProperty);
            set => SetValue(DrawBordersProperty, value);
        }

        /// <summary>
        ///  flag that indicates if auto scaling on the y axis is enabled
        /// </summary>
        public bool AutoScaleMinMaxEnabled
        {
            get => (bool)GetValue(AutoScaleMinMaxEnabledProperty);
            set => SetValue(AutoScaleMinMaxEnabledProperty, value);
        }

        public bool ClipDataToContent
        {
            get => (bool)GetValue(ClipDataToContentProperty);
            set => SetValue(ClipDataToContentProperty, value);
        }

        public bool ClipValuesToContent
        {
            get => (bool)GetValue(ClipValuesToContentProperty);
            set => SetValue(ClipValuesToContentProperty, value);
        }
        /// <summary>
        /// Sets the minimum offset (padding) around the chart, defaults to 15
        /// </summary>
        public float MinOffset
        {
            get => (float)GetValue(MinOffsetProperty);
            set => SetValue(MinOffsetProperty, value);
        }


        public void StopDeceleration()
        {
            decelerationVelocity = SKPoint.Empty;
        }


        private void SaveTouchStart(SKPoint point)
        {
            savedMatrix = ViewPortHandler.touchMatrix;
            touchStartPoint = point;
            closestDatasetToTouch = GetDataSetByTouchPoint(point.X, point.Y);
        }



        public bool IsDragEnabled
        {
            get => DragXEnabled || DragYEnabled;
        }

        public bool ScaleXEnabled
        {
            get => (bool)GetValue(ScaleXEnabledProperty);
            set => SetValue(ScaleXEnabledProperty, value);
        }

        public bool ScaleYEnabled
        {
            get => (bool)GetValue(ScaleYEnabledProperty);
            set => SetValue(ScaleYEnabledProperty, value);
        }
    }
}
