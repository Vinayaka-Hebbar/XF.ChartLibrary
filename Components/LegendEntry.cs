namespace XF.ChartLibrary.Components
{
#if __IOS__ || __TVOS
    using Color = UIKit.UIColor;
#elif __ANDROID__
    using Color = Android.Graphics.Color;
#elif NETSTANDARD
    using Color = SkiaSharp.SKColor;
#endif

    using System.Collections.Generic;
    public class LegendEntry
    {
        public LegendEntry()
        {
        }

        public LegendEntry(string label)
        {
            Label = label;
        }

        public string Label { get; set; }

        public Color LabelColor { get; set; }

        public Form Form { get; set; } = Form.Default;

        public float FormSize { get; set; } = float.NaN;

        public float FormLineWidth { get; set; } =  float.NaN;

        public float FormLineDashPhase { get; set; }

        public IList<float> FormLineDashLengths { get; set; }

        public Color FormColor { get; set; }

    }
}
