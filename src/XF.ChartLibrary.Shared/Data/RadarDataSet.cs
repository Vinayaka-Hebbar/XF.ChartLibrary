#nullable enable

using System;
using System.Collections.Generic;

#if NETSTANDARD || SKIASHARP
using Alpha = System.Byte;
using Color = SkiaSharp.SKColor;
#elif __IOS__ || __TVOS
using Color = UIKit.UIColor;
using Alpha = System.Single;
#elif __ANDROID__
using Color = Android.Graphics.Color;
using Alpha = System.Byte;
#endif

namespace XF.ChartLibrary.Data
{
    public class RadarDataSet : LineRadarDataSet<RadarEntry>, Interfaces.DataSets.IRadarDataSet
    {
#if __ANDROID__ || SKIASHARP
        private byte highlightCircleStrokeAlpha = 76;
#else
        private float highlightCircleStrokeAlpha = 0.3f;
#endif
        public RadarDataSet(IList<RadarEntry> yVals, string label) : base(yVals, label)
        {
        }

        public bool DrawHighlightCircleEnabled { get; set; }

        public Color HighlightCircleFillColor { get; set; } = Utils.ColorTemplate.White;

        public Color? HighlightCircleStrokeColor { get; set; }

        public Alpha HighlightCircleStrokeAlpha
        {
            get => highlightCircleStrokeAlpha;
            set => highlightCircleStrokeAlpha = value;
        }

        public float HighlightCircleInnerRadius { get; set; } = 3.0f;

        public float HighlightCircleOuterRadius { get; set; } = 4.8f;

        public float HighlightCircleStrokeWidth { get; set; } = 2.0f;
    }
}
