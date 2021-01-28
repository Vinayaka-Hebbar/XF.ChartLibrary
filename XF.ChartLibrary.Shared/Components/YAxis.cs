namespace XF.ChartLibrary.Components
{
    using System;
#if __IOS__ || __TVOS
    using UIKit;
    using Color = UIKit.UIColor;
    using Colors = UIKit.UIColor;
#elif __ANDROID__
    using Color = Android.Graphics.Color;
    using Colors = Android.Graphics.Color;
    using Paint = Android.Graphics.Paint;
#elif NETSTANDARD
    using Color = SkiaSharp.SKColor;
    using Colors = SkiaSharp.SKColors;
    using Paint = SkiaSharp.SKPaint;
#endif


    /// <summary>
    /// Enum that specifies the axis a DataSet should be plotted against, either LEFT or RIGHT.
    /// </summary>
    public enum YAxisDependency
    {
        Left, Right
    }

    /// <summary>
    ///  Class representing the y-axis labels settings and its entries. Only use the setter methods to
    /// modify it.Do not
    /// access public variables directly.Be aware that not all features the YLabels class provides
    /// are suitable for the
    /// RadarChart.Customizations that affect the value range of the axis need to be applied before
    /// setting data for the
    /// chart.
    /// </summary>
    public class YAxis : AxisBase
    {

        /**
         * indicates if the bottom y-label entry is drawn or not
         */
        public bool DrawBottomYLabelEntry { get; set; } = true;

        /**
         * indicates if the top y-label entry is drawn or not
         */
        public bool DrawTopYLabelEntry { get; set; } = true;

        /**
         * flag that indicates if the axis is inverted or not
         */
        public bool Inverted { get; set; } = false;

        /**
         * flag that indicates if the zero-line should be drawn regardless of other grid lines
         */
        public bool DrawZeroLine { get; set; } = false;

        /**
         * flag indicating that auto scale min restriction should be used
         */
        public bool UseAutoScaleRestrictionMin { get; set; } = false;

        /**
         * flag indicating that auto scale max restriction should be used
         */
        public bool UseAutoScaleRestrictionMax { get; set; } = false;

        /**
         * Color of the zero line
         */
        public Color ZeroLineColor { get; set; } = Colors.Gray;

        /**
         * Width of the zero line in pixels
         */
        public float ZeroLineWidth
        {
            get => zeroLineWidth;
            set
            {
#if __ANDROID__
                zeroLineWidth = value.DpToPixel();
#else
                zeroLineWidth = value;
#endif
            }
        }
        /**
         * axis space from the largest value to the top in percent of the total axis range
         */
        public float SpacePercentTop { get; set; } = 10f;

        /**
         * axis space from the smallest value to the bottom in percent of the total axis range
         */
        public float SpacePercentBottom { get; set; } = 10f;

        /**
         * the position of the y-labels relative to the chart
         */
        public YAxisLabelPosition Position { get; set; } = YAxisLabelPosition.OutSideChart;

        /**
         * the horizontal offset of the y-label
         */
        public float XLabelOffset { get; set; } = 0.0f;

        /**
         * enum for the position of the y-labels relative to the chart
         */
        public enum YAxisLabelPosition
        {
            OutSideChart, InsideChart
        }

        /**
         * the side this axis object represents
         */
        public YAxisDependency AxisDependency { get; set; }

        /**
         * the minimum width that the axis should take (in dp).
         * <p/>
         * default: 0.0
         */
        public float MinWidth { get; set; } = 0.0f;

        /**
         * the maximum width that the axis can take (in dp).
         * use Inifinity for disabling the maximum
         * default: Float.POSITIVE_INFINITY (no maximum specified)
         */
        public float MaxWidth { get; set; } = float.MaxValue;
        private float zeroLineWidth = 1f;


        public YAxis()
        {
            // default left
            AxisDependency = YAxisDependency.Left;
            yOffset = 0f;
        }

        public YAxis(YAxisDependency position)
        {
            AxisDependency = position;
            yOffset = 0f;
        }


        /**
         * This is for normal (not horizontal) charts horizontal spacing.
         *
         * @param p
         * @return
         */
#if __ANDROID__ || NETSTANDARD
        public Utils.ChartSize GetRequiredWidthSpace(Paint p)
        {
            p.TextSize = TextSize;

            string label = GetLongestLabel();
            Utils.ChartSize size = p.Measure(label);
            float width = (float)size.Width + XOffset * 2f;
            float height = (float)size.Height + YOffset * 2f;
            float minWidth = MinWidth;
            float maxWidth = MaxWidth;

#if __ANDROID__
            if (minWidth > 0.0f)
                minWidth = minWidth.DpToPixel();
            if (maxWidth > 0.0f && maxWidth != float.MaxValue)

                maxWidth = maxWidth.DpToPixel();
#endif

            width = MathF.Max(minWidth, Math.Min(width, maxWidth > 0.0 ? maxWidth : width));

            return new Utils.ChartSize(width, height);
        }
#elif __IOS__ || __TVOS__
        public Utils.ChartSize RequiredSize()
        {
            var label = GetLongestLabel();
            var size = label.StringSize(Font);
            var width = (float)size.Width + (xOffset * 2.0f);
            var height = (float)size.Height + yOffset * 2.0f;
            width = MathF.Max(MinWidth, MathF.Min((float)width, MaxWidth > 0.0f ? MaxWidth : (float)width));
            return new Utils.ChartSize(width, height);
        }

        public float GetRequiredHeightSpace()
        {
            return RequiredSize().Height;
        }
#endif


        /**
         * Returns true if this axis needs horizontal offset, false if no offset is needed.
         *
         * @return
         */
        public bool NeedsOffset
        {
            get
            {
                return IsEnabled && IsDrawLabelsEnabled && Position == YAxisLabelPosition
                    .OutSideChart;
            }
        }


        public override void Calculate(float dataMin, float dataMax)
        {

            float min = dataMin;
            float max = dataMax;

            // Make sure max is greater than min
            // Discussion: https://github.com/danielgindi/Charts/pull/3650#discussion_r221409991
            if (min > max)
            {
                if (CustomAxisMax && CustomAxisMin)
                {
                    float t = min;
                    min = max;
                    max = t;
                }
                else if (CustomAxisMax)
                {
                    min = max < 0f ? max * 1.5f : max * 0.5f;
                }
                else if (CustomAxisMin)
                {
                    max = min < 0f ? min * 0.5f : min * 1.5f;
                }
            }

            float range = MathF.Abs(max - min);

            // in case all values are equal
            if (range == 0f)
            {
                max += 1f;
                min -= 1f;
            }

            // recalculate
            range = MathF.Abs(max - min);

            // calc extra spacing
            AxisMinimum = CustomAxisMin ? AxisMinimum : min - (range / 100f) * SpacePercentBottom;
            AxisMaximum = CustomAxisMax ? AxisMaximum : max + (range / 100f) * SpacePercentTop;

            AxisRange = MathF.Abs(AxisMinimum - AxisMaximum);
        }
    }
}
