namespace XF.ChartLibrary.Components
{
#if NETSTANDARD || SKIASHARP
    using DashPathEffect = SkiaSharp.SKPathEffect;
    using PaintStyle = SkiaSharp.SKPaintStyle;
    using Color = SkiaSharp.SKColor;
#elif __ANDROID__
    using DashPathEffect = Android.Graphics.DashPathEffect;
    using Color = Android.Graphics.Color;
    using PaintStyle = Android.Graphics.Paint.Style;
#elif __IOS__ || __TVOS__
    using DashPathEffect = XF.ChartLibrary.Utils.DashPathEffect;
    using Color = UIKit.UIColor;
#endif
    public class LimitLine : ComponentBase
    {
        /** limit / maximum (the y-value or xIndex) */
        private float _limit = 0f;

        /** the width of the limit line */
        private float _lineWidth = 2f;

        /** the color of the limit line */
        private Color _lineColor = ChartUtil.FromRGB(237, 91, 91);

#if NETSTANDARD || SKIASHARP
        private PaintStyle _textStyle = PaintStyle.StrokeAndFill;
#elif __ANDROID__
        private PaintStyle _textStyle = PaintStyle.FillAndStroke;
#endif

        /** label string that is drawn next to the limit line */
        private string _label = string.Empty;

        /** the path effect of this LimitLine that makes dashed lines possible */
        private DashPathEffect dashPathEffect = null;

        /** indicates the position of the LimitLine label */
        private LimitLabelPosition _labelPosition = LimitLabelPosition.RightTop;

        /** enum that indicates the position of the LimitLine label */
        public enum LimitLabelPosition
        {
            LeftTop, LeftBottom, RightTop, RightBottom
        }

        /// <summary>
        /// Constructor with limit.
        /// </summary>
        /// <param name="limit">the position(the value) on the y-axis(y-value) or x-axis
        /// (xIndex) where this line should appear</param>
        public LimitLine(float limit)
        {
            _limit = limit;
        }

         /// <summary>
        /// Constructor with limit.
        /// </summary>
        /// <param name="limit">the position(the value) on the y-axis(y-value) or x-axis
        /// (xIndex) where this line should appear</param>
        /// <param name="label">provide "" if no label is required</param>
        public LimitLine(float limit, string label)
        {
            _limit = limit;
            _label = label;
        }

        public float Limit
        {
            get => _limit;
            set => _limit = value;
        }

        public string Label
        {
            get => _label;
            set => _label = value;
        }

        public Color LineColor
        {
            get => _lineColor;
            set => _lineColor = value;
        }

        public float LineWidth
        {
            get => _lineWidth;
            set
            {
                if (value < 0.2f)
                    value = 0.2f;
                if (value > 12.0f)
                    value = 12.0f;
#if __ANDROID__ || SKIASHARP
                _lineWidth = value.DpToPixel();
#else
                _lineWidth = value;
#endif
            }
        }

        public LimitLabelPosition LabelPosition
        {
            get => _labelPosition;
            set => _labelPosition = value;
        }

        public DashPathEffect DashPathEffect
        {
            get => dashPathEffect;
            set => dashPathEffect = value;
        }


#if __ANDROID__ || NETSTANDARD || SKIASHARP
        public PaintStyle TextStyle
        {
            get => _textStyle;
            set => _textStyle = value;
        }
#endif
    }
}
