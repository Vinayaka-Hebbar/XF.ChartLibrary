using System.Collections.Generic;
using XF.ChartLibrary.Utils;

namespace XF.ChartLibrary.Components
{
#if __IOS__ || __TVOS
    using Paint = UIKit.UIFont;
#elif __ANDROID__
    using Paint = Android.Graphics.Paint;
#elif NETSTANDARD
    using Paint = SkiaSharp.SKPaint;
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

    public partial class Legend : IComponent
    {
        private bool _isLegendCustom;

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
        public Orientation Orientation { get; set; } = Orientation.Horizontal;

        /// Flag indicating whether the legend will draw inside the chart or outside
        public bool DrawInside { get; set; } = false;

        /// Flag indicating whether the legend will draw inside the chart or outside
        public bool IsDrawInsideEnabled => DrawInside;

        /// The text direction of the legend
        public Direction Direction { get; set; } = Direction.LeftToRight;

        /// The form/shape of the legend forms
        public Form Form { get; set; } = Form.Square;

        /// The size of the legend forms
        public float FormSize { get; set; } = 8.0f;

        /// The line width for forms that consist of lines
        public float FormLineWidth { get; set; } = 3.0f;

        /// Line dash configuration for shapes that consist of lines.
        ///
        /// This is how much (in pixels) into the dash pattern are we starting from.
        public float FormLineDashPhase { get; set; }

        /// Line dash configuration for shapes that consist of lines.
        ///
        /// This is the actual dash pattern.
        /// I.e. [2, 3] will paint [--   --   ]
        /// [1, 3, 4, 2] will paint [-   ----  -   ----  ]
        public IList<float> FormLineDashLengths { get; set; }

        public float XEntrySpace { get; set; } = 6.0f;
        public float YEntrySpace { get; set; } = 0.0f;
        public float FormToTextSpace { get; set; } = 5.0f;
        public float StackSpace { get; set; } = 3.0f;


        public IList<ChartSize> CalculatedLabelSizes { get; set; } = new List<ChartSize>();
        public IList<bool> CalculatedLabelBreakPoints { get; set; } = new List<bool>();
        public IList<ChartSize> CalculatedLineSizes { get; set; } = new List<ChartSize>();

        public Legend(IList<LegendEntry> entries)
        {
            Entries = entries;
        }

        public Legend()
        {
            Entries = new List<LegendEntry>();
        }

        public IList<LegendEntry> Entries { get; }

        public float XOffset { get; set; } = 5.0f;

        public float YOffset { get; set; } = 3.0f;

        public bool IsEnabled { get; set; } = true;

        public ChartSize GetMaximumEntrySize(Paint paint)
        {
            var maxW = 0.0f;
            var maxH = 0.0f;


            var maxFormSize = 0.0f;

            foreach (var entry in Entries)
            {
#if __ANDROID__
                var formSize = (entry.FormSize.IsNaN() ? FormSize : entry.FormSize).DpToPixel();
#else
                var formSize = entry.FormSize.IsNaN() ? FormSize : entry.FormSize;
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
#if __ANDROID__

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

        
    }
}
