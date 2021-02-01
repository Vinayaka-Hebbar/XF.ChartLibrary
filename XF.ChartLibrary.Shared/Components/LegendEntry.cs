namespace XF.ChartLibrary.Components
{
#if NETSTANDARD || SKIASHARP
    using Color = SkiaSharp.SKColor;
    using DashPathEffect = SkiaSharp.SKPathEffect;
#elif __IOS__ || __TVOS
    using Color = UIKit.UIColor;
    using DashPathEffect = XF.ChartLibrary.Utils.DashPathEffect;
#elif __ANDROID__
    using Color = Android.Graphics.Color;
     using DashPathEffect = Android.Graphics.DashPathEffect;
#endif

    public class LegendEntry
    {
        public LegendEntry()
        {
        }

        public LegendEntry(string label)
        {
            Label = label;
        }

        /// <param name="label">The legend entry text.A `null` label will start a group.</param>
        /// <param name="form">The form to draw for this entry.</param>
        /// <param name="formSize">Set to NaN to use the legend's default.</param>
        /// <param name="formLineWidth">Set to NaN to use the legend's default.</param>
        /// <param name="formLineDashEffect">Set to nil to use the legend's default.</param>
        /// <param name="formColor">The color for drawing the form.</param>
        public LegendEntry(string label,
                           Form form,
                           float formSize,
                           float formLineWidth,
                           DashPathEffect formLineDashEffect,
                           Color formColor)
        {
            Label = label;
            Form = form;
            FormSize = formSize;
            FormLineWidth = formLineWidth;
            FormLineDashEffect = formLineDashEffect;
            FormColor = formColor;
        }

        public string Label { get; set; }

        public Color LabelColor { get; set; }

        public Form Form { get; set; } = Form.Default;

        public float FormSize { get; set; } = float.NaN;

        public float FormLineWidth { get; set; } = float.NaN;

        public DashPathEffect FormLineDashEffect { get; set; }

        public Color FormColor { get; set; }

    }
}
