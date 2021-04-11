using System;
using Xamarin.Forms;
using XF.ChartLibrary.Components;
using XF.ChartLibrary.Utils;

namespace XF.ChartLibrary.Charts
{
    partial class RadarChart
    {
        public static readonly BindableProperty YAxisProperty = BindableProperty.Create(nameof(YAxis), typeof(YAxis), typeof(RadarChart), defaultBindingMode: BindingMode.OneWayToSource);
        
        public static readonly Color DefaultWebColor = Color.FromRgb(122 / 255f, 122 / 255f, 122 / 255f);
        
        public static readonly BindableProperty WebColorProperty = BindableProperty.Create(nameof(WebColor), typeof(Color), typeof(RadarChart), defaultValue: DefaultWebColor);

        public static readonly BindableProperty WebColorInnerProperty = BindableProperty.Create(nameof(WebColorInner), typeof(Color), typeof(RadarChart), defaultValue: DefaultWebColor);

        public static readonly BindableProperty WebAlphaProperty = BindableProperty.Create(nameof(WebAlpha), typeof(double), typeof(RadarChart), defaultValue: 150 / 255);

        public static readonly BindableProperty DrawWebProperty = BindableProperty.Create(nameof(DrawWeb), typeof(bool), typeof(RadarChart), defaultValue: true);

        public static readonly BindableProperty SkipWebLineCountProperty = BindableProperty.Create(nameof(SkipWebLineCount), typeof(int), typeof(RadarChart), defaultValue: 0, coerceValue: (b, v) => Math.Max(0, (int)v));

        public static readonly BindableProperty WebLineWidthProperty = BindableProperty.Create(nameof(WebLineWidth), typeof(float), typeof(RadarChart), coerceValue: Extensions.DpToPx);

        public static readonly BindableProperty WebLineWidthInnerProperty = BindableProperty.Create(nameof(WebLineWidthInner), typeof(float), typeof(RadarChart), coerceValue: Extensions.DpToPx);


        public RadarChart()
        {

        }

        public YAxis YAxis
        {
            get => (YAxis)GetValue(YAxisProperty);
            set => SetValue(YAxisProperty, value);
        }

        public Color WebColor
        {
            get => (Color)GetValue(WebColorProperty);
            set => SetValue(WebColorProperty, value);
        }

        public Color WebColorInner
        {
            get => (Color)GetValue(WebColorInnerProperty);
            set => SetValue(WebColorInnerProperty, value);
        }

        public double WebAlpha
        {
            get => (double)GetValue(WebAlphaProperty);
            set => SetValue(WebAlphaProperty, value);
        }

        public bool DrawWeb
        {
            get => (bool)GetValue(DrawWebProperty);
            set => SetValue(DrawWebProperty, value);
        }

        public float WebLineWidth
        {
            get => (float)GetValue(WebLineWidthProperty);
            set => SetValue(WebLineWidthProperty, value);
        }

        public float WebLineWidthInner
        {
            get => (float)GetValue(WebLineWidthInnerProperty);
            set => SetValue(WebLineWidthInnerProperty, value);
        }

        public int SkipWebLineCount
        {
            get => (int)GetValue(SkipWebLineCountProperty);
            set => SetValue(SkipWebLineCountProperty, value);
        }

        internal SkiaSharp.SKColor GetWebColor()
        {
            return WebColor.ToSKColor(WebAlpha);
        }

        internal SkiaSharp.SKColor GetWebColorInner()
        {
            return WebColorInner.ToSKColor(WebAlpha);
        }
    }
}
