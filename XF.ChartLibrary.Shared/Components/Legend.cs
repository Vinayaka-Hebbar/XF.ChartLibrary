using System;
using System.Collections.Generic;
using XF.ChartLibrary.Utils;

namespace XF.ChartLibrary.Components
{
#if NETSTANDARD || SKIASHARP
    using Paint = SkiaSharp.SKPaint;
    using DashPathEffect = SkiaSharp.SKPathEffect;
    using Color = SkiaSharp.SKColor;
#elif __IOS__ || __TVOS
    using Paint = UIKit.UIFont;
    using Color = UIKit.UIColor;
    using DashPathEffect = XF.ChartLibrary.Utils.DashPathEffect;
#elif __ANDROID__
    using Paint = Android.Graphics.Paint;
    using DashPathEffect = Android.Graphics.DashPathEffect;
    using Color = Android.Graphics.Color;
#endif
    public enum Form
    {
        /// Avoid drawing a form
        None,

        /// Do not draw the a form, but leave space for it
        Empty,
        /// Draw a square
        Square,
        Default,
        /// Draw a circle
        Circle,

        /// Draw a horizontal line
        Line
    }

    public enum HorizontalAlignment
    {
        Left, Center, Right
    }

    public enum VerticalAlignment
    {
        Top, Center, Bottom
    }

    public enum Orientation
    {
        Horizontal, Vertical
    }

    public enum Direction
    {
        LeftToRight, RightToLeft
    }

    public partial class Legend : ComponentBase
    {
        private bool _isLegendCustom;
        private IList<LegendEntry> entries;

        /// <summary>
        /// Entries that will be appended to the end of the auto calculated entries after calculating the legend.
        /// (if the legend has already been calculated, you will need to call notifyDataSetChanged() to let the changes take effect)
        /// </summary>
        private IList<LegendEntry> extraEntries;
        private bool drawInside = false;

        /// <summary>
        ///  The horizontal alignment of the legend
        /// </summary>
        public HorizontalAlignment HorizontalAlignment
        {
            get; set;
        }

        /// <summary>
        /// The vertical alignment of the legend
        /// </summary>
        public VerticalAlignment VerticalAlignment
        {
            get; set;
        } = VerticalAlignment.Bottom;


        /// The orientation of the legend
        public Orientation Orientation { get; set; }

        /// Flag indicating whether the legend will draw inside the chart or outside
        public bool DrawInside
        {
            get => drawInside;
            set => drawInside = value;
        }

        /// Flag indicating whether the legend will draw inside the chart or outside
        public bool IsDrawInsideEnabled => drawInside;

        public bool IsLegendCustom => _isLegendCustom;

        /// The text direction of the legend
        public Direction Direction { get; set; }

        /// The form/shape of the legend forms
        public Form Form { get; set; } = Form.Square;

        /// The size of the legend forms
        public float FormSize { get; set; } = 8.0f;

        /// The line width for forms that consist of lines
        public float FormLineWidth { get; set; } = 3.0f;

        /// dash path effect used for shapes that consist of lines.
        public DashPathEffect FormLineDashEffect { get; set; }

        public float XEntrySpace { get; set; } = 6.0f;
        public float YEntrySpace { get; set; } = 0.0f;
        public float FormToTextSpace { get; set; } = 5.0f;
        public float StackSpace { get; set; } = 3.0f;

        public IList<ChartSize> CalculatedLabelSizes { get; set; } = new List<ChartSize>();

        public IList<bool> CalculatedLabelBreakPoints { get; set; } = new List<bool>();

        public IList<ChartSize> CalculatedLineSizes { get; set; } = new List<ChartSize>();

        public Legend(IList<LegendEntry> entries)
        {
            this.entries = entries;
#if __IOS__ || __TVS__
yOffset = 3f;
#else
            yOffset = 3f.DpToPixel();
#endif
        }

        public Legend()
        {
            entries = new List<LegendEntry>();
        }

        public IList<LegendEntry> Entries
        {
            get => entries;
            set
            {
                entries = value;
                _isLegendCustom = true;
            }
        }

        public IList<LegendEntry> ExtraEntries
        {
            get => extraEntries;
            set => extraEntries = value;
        }

        /// <summary>
        ///  Entries that will be appended to the end of the auto calculated
        ///   entries after calculating the legend.
        /// (if the legend has already been calculated, you will need to call notifyDataSetChanged()
        ///   to let the changes take effect)
        /// </summary>
        public void SetExtra(IList<Color> colors, IList<string> labels)
        {

            List<LegendEntry> entries = new List<LegendEntry>();

            for (int i = 0; i < Math.Min(colors.Count, labels.Count); i++)
            {
                var entry = new LegendEntry
                {
                    FormColor = colors[i],
                    Label = labels[i]
                };

                if (entry.FormColor == default)
                    entry.Form = Form.Empty;

                entries.Add(entry);
            }

            extraEntries = entries;
        }

        public ChartSize GetMaximumEntrySize(Paint paint)
        {
            var maxW = 0.0f;
            var maxH = 0.0f;


            var maxFormSize = 0.0f;

            foreach (var entry in Entries)
            {
#if __ANDROID__ || SKIASHARP
                var formSize = (float.IsNaN(entry.FormSize) ? FormSize : entry.FormSize).DpToPixel();
#else
                var formSize = float.IsNaN(entry.FormSize) ? FormSize : entry.FormSize;
#endif
                if (formSize > maxFormSize)
                {
                    maxFormSize = formSize;
                }

                if (entry.Label is string label)
                {
                    var size = paint.Measure(label);
                    if (size.Width > maxW)
                    {
                        maxW = size.Width;
                    }
                    if (size.Height > maxH)
                    {
                        maxH = size.Height;
                    }
                }
                else
                { continue; }
            }
#if __ANDROID__ || SKIASHARP
            return new ChartSize(
                width: maxW + maxFormSize + FormToTextSpace.DpToPixel(),
                height: maxH
            );
#else
            return new ChartSize(
                width: maxW + maxFormSize + FormToTextSpace,
                height: maxH
            );
#endif
        }


        public float NeededWidth { get; set; } = 0.0f;
        public float NeededHeight { get; set; } = 0.0f;
        public float TextWidthMax { get; set; } = 0.0f;
        public float TextHeightMax { get; set; } = 0.0f;

        /// flag that indicates if word wrapping is enabled
        /// this is currently supported only for `orientation == Horizontal`.
        /// you may want to set maxSizePercent when word wrapping, to set the point where the text wraps.
        /// 
        /// **default**: true
        public bool WordWrapEnabled { get; set; } = true;

        /// if this is set, then word wrapping the legend is enabled.
        public bool IsWordWrapEnabled => WordWrapEnabled;

        /// The maximum relative size out of the whole chart view in percent.
        /// If the legend is to the right/left of the chart, then this affects the width of the legend.
        /// If the legend is to the top/bottom of the chart, then this affects the height of the legend.
        /// 
        /// **default**: 0.95 (95%)
        public float MaxSizePercent { get; set; } = 0.95f;

        public void ResetCustom()
        {
            _isLegendCustom = false;
        }
    }
}
