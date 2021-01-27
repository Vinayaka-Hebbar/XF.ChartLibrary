using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Text;

namespace XF.ChartLibrary.Charts
{
    partial class BarLineChartBase<TData, TDataSet>
    {
        /// <summary>
        /// paint object for the (by default) lightgrey background of the grid
        /// </summary>
        protected SKPaint mGridBackgroundPaint;

        protected SKPaint mBorderPaint;

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
        }

        protected override void OnTouch(SKTouchEventArgs e)
        {
            base.OnTouch(e);
        }
    }
}
