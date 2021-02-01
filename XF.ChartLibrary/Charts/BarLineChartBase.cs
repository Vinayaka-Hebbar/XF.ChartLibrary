using SkiaSharp;
using System;
using Xamarin.Forms;
using XF.ChartLibrary.Components;
using XF.ChartLibrary.Interfaces.DataSets;
using XF.ChartLibrary.Renderer;
using XF.ChartLibrary.Utils;

namespace XF.ChartLibrary.Charts
{
    partial class BarLineChartBase<TData, TDataSet>
    {
        public static readonly BindableProperty DragXEnabledProperty = BindableProperty.Create(nameof(DragXEnabled), typeof(bool), typeof(BarLineChartBase<TData, TDataSet>));

        public static readonly BindableProperty DragYEnabledProperty = BindableProperty.Create(nameof(DragYEnabled), typeof(bool), typeof(BarLineChartBase<TData, TDataSet>));

        public static readonly BindableProperty ScaleXEnabledProperty = BindableProperty.Create(nameof(ScaleXEnabled), typeof(bool), typeof(BarLineChartBase<TData, TDataSet>));

        public static readonly BindableProperty ScaleYEnabledProperty = BindableProperty.Create(nameof(ScaleYEnabled), typeof(bool), typeof(BarLineChartBase<TData, TDataSet>));

        public static readonly BindableProperty AxisLeftProperty = BindableProperty.Create(nameof(AxisLeft), typeof(YAxis), typeof(BarLineChartBase<TData, TDataSet>), defaultValue: new YAxis(YAxisDependency.Left));

        public static readonly BindableProperty AxisRightProperty = BindableProperty.Create(nameof(AxisRight), typeof(YAxis), typeof(BarLineChartBase<TData, TDataSet>), defaultValue: new YAxis(YAxisDependency.Right));

        public static readonly BindableProperty MaxVisibleCountProperty = BindableProperty.Create(nameof(MaxVisibleCount), typeof(int), typeof(BarLineChartBase<TData, TDataSet>), defaultValue:100);


        private SKPoint decelerationVelocity = SKPoint.Empty;

        private SKPoint decelerationCurrentPoint = SKPoint.Empty;

        private SKMatrix savedMatrix = SKMatrix.CreateIdentity();

        private SKPoint touchStartPoint = SKPoint.Empty;

        private IDataSet closestDatasetToTouch;

        private float mSavedXDist = 1f;
        private float mSavedYDist = 1f;
        private float mSavedDist = 1f;

        public Gestures.ChartGesture Gesture { get; }

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
                StrokeWidth = 1f
            };

            Gesture = new Gestures.ChartGesture();
            GestureRecognizers.Add(Gesture);
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
