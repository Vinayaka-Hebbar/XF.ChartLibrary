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
            mGridBackgroundPaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                // Color = SKColors.White;
                Color = new SKColor(240, 240, 240) // light
                                                   // grey
            };

            mBorderPaint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = SKColors.Black,
                StrokeWidth = 1f.DpToPixel()
            };
        }

       
        public void StopDeceleration()
        {
            decelerationVelocity = SKPoint.Empty;
        }


        private void SaveTouchStart(SKPoint point)
        {
            savedMatrix = ViewPortHandler.TouchMatrix;
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
