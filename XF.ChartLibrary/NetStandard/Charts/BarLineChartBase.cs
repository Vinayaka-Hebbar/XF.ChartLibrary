using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Text;
using XF.ChartLibrary.Interfaces.DataSets;

namespace XF.ChartLibrary.Charts
{
    partial class BarLineChartBase<TData, TDataSet>
    {
        /// <summary>
        /// paint object for the (by default) lightgrey background of the grid
        /// </summary>
        protected SKPaint mGridBackgroundPaint;

        private SKPoint decelerationVelocity = SKPoint.Empty;

        private SKPoint decelerationCurrentPoint = SKPoint.Empty;

        private SKMatrix savedMatrix = SKMatrix.CreateIdentity();

        private SKPoint touchStartPoint = SKPoint.Empty;

        protected SKPaint mBorderPaint;

        private IDataSet closestDatasetToTouch;

        private float mSavedXDist = 1f;
        private float mSavedYDist = 1f;
        private float mSavedDist = 1f;

        public override void Initialize()
        {
            base.Initialize();
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

            var tap = new Xamarin.Forms.TapGestureRecognizer();
            tap.Tapped += OnTap;
            GestureRecognizers.Add(tap);
        }

        private void OnTap(object sender, EventArgs e)
        {
            var 
            if (data == null)
                return;
        }

        protected override void OnTouch(SKTouchEventArgs e)
        {
            if (IsDragEnabled == false && (mScaleXEnabled && mScaleYEnabled) == false)
                return;
            switch (e.ActionType)
            {
                case SKTouchAction.Entered when e.Pressure < 20:
                    StopDeceleration();
                    SaveTouchStart(e.Location);
                    break;
                case SKTouchAction.Pressed when e.Pressure > 20:
                    SaveTouchStart(e.Location);
                    break;
                case SKTouchAction.Moved:
                    break;
                case SKTouchAction.Released:
                    break;
                case SKTouchAction.Cancelled:
                    break;
                case SKTouchAction.Exited:
                    break;
                case SKTouchAction.WheelChanged:
                    break;
                default:
                    break;
            }
            base.OnTouch(e);
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
    }
}
