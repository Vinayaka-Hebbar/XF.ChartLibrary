using System.Collections.Generic;
using XF.ChartLibrary.Utils;

namespace XF.ChartLibrary.Data
{
#if __IOS__ || __TVOS
    using Color = UIKit.UIColor;
#elif __ANDROID__
    using Color = Android.Graphics.Color;
#elif NETSTANDARD
    using Color = SkiaSharp.SKColor;
#endif
    public abstract class LineRadarDataSet<TEntry> : LineScatterCandleRadarDataSet<TEntry>, Interfaces.DataSets.ILineRadarDataSet<TEntry> where TEntry : Entry
    {
        private Color fillColor = ChartUtil.FromRGB(140, 234, 255);
        private IFill fill;
#if __ANDROID__
        private float lineWidth = 2.5f;
#else
        private float lineWidth = 1.0f;
#endif

        private bool drawFilled;

        public IFill Fill
        {
            get => fill;
            set => fill = value;
        }


        public Color FillColor
        {
            get => fillColor;
            set
            {
                fillColor = value;
                fill = null;
            }
        }

        public float LineWidth
        {
            get => lineWidth;
            set
            {
                if (value < 0.0f)
                    value = 0.0f;
                if (value > 10.0f)
                    value = 10.0f;
#if __ANDROID__
                lineWidth = value.DpToPixel();
#else
                lineWidth = value;
#endif
            }
        }

        public bool DrawFilled
        {
            get => drawFilled;
            set => drawFilled = value;
        }

#if __IOS__ || __TVOS__
        public float FillAlpha { get; set; } = 0.33f; 
#else
        public byte FillAlpha { get; set; } = 85;
#endif

        protected LineRadarDataSet(IList<TEntry> yVals, string label) : base(yVals, label)
        {
        }
    }
}
