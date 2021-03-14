namespace XF.ChartLibrary.Components
{
    using System;

#if NETSTANDARD || SKIASHARP
    using Color = SkiaSharp.SKColor;
    using Colors = SkiaSharp.SKColors;
    using Paint = SkiaSharp.SKPaint;
#elif __IOS__ || __TVOS
    using UIKit;
    using Color = UIKit.UIColor;
    using Colors = UIKit.UIColor;
#elif __ANDROID__
    using Color = Android.Graphics.Color;
    using Colors = Android.Graphics.Color;
    using Paint = Android.Graphics.Paint;
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

        /// <summary>
        ///  indicates if the bottom y-label entry is drawn or not
        /// </summary>
        public bool DrawBottomYLabelEntry { get; set; } = true;

        /// <summary>
        /// indicates if the top y-label entry is drawn or not
        /// </summary>
        public bool DrawTopYLabelEntry { get; set; } = true;

        /// <summary>
        /// flag that indicates if the axis is inverted or not
        /// </summary>
        public bool Inverted { get; set; } = false;

        /// <summary>
        /// flag that indicates if the zero-line should be drawn regardless of other grid lines
        /// </summary>
        public bool DrawZeroLine { get; set; } = false;

        /// <summary>
        /// flag indicating that auto scale min restriction should be used
        /// </summary>
        public bool UseAutoScaleRestrictionMin { get; set; } = false;

        /// <summary>
        /// flag indicating that auto scale max restriction should be used
        /// </summary>
        public bool UseAutoScaleRestrictionMax { get; set; } = false;

        /// <summary>
        /// Color of the zero line
        /// </summary>
        public Color ZeroLineColor { get; set; } = Colors.Gray;

        /// <summary>
        /// Width of the zero line in pixels
        /// </summary>
        public float ZeroLineWidth
        {
            get => zeroLineWidth;
            set
            {
#if PIXELSCALE
                zeroLineWidth = value.DpToPixel();
#else
                zeroLineWidth = value;
#endif
            }
        }

        /// <summary>
        /// axis space from the largest value to the top in percent of the total axis range
        /// </summary>
        public float SpacePercentTop { get; set; } = 10f;

        /// <summary>
        /// axis space from the smallest value to the bottom in percent of the total axis range
        /// </summary>
        public float SpacePercentBottom { get; set; } = 10f;

        /// <summary>
        ///  the position of the y-labels relative to the chart
        /// </summary>
        public YAxisLabelPosition Position { get; set; } = YAxisLabelPosition.OutSideChart;

        /// <summary>
        /// the horizontal offset of the y-label
        /// </summary>
        public float XLabelOffset { get; set; } = 0.0f;

        /// <summary>
        /// enum for the position of the y-labels relative to the chart
        /// </summary>
        public enum YAxisLabelPosition
        {
            OutSideChart, InsideChart
        }

        /// <summary>
        /// the side this axis object represents
        /// </summary>
        public YAxisDependency AxisDependency { get; set; }

        /// <summary>
        ///  the minimum width that the axis should take (in dp).
        /// default: 0.0
        /// </summary>
        public float MinWidth { get; set; } = 0.0f;

        /// <summary>
        /// the maximum width that the axis can take (in dp).
        /// use Inifinity for disabling the maximum
        /// default: <see cref="float.PositiveInfinity"/> (no maximum specified)
        /// </summary>
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

        /// <summary>
        /// This is for normal(not horizontal) charts horizontal spacing.
        /// </summary>
#if __ANDROID__ || SKIASHARP
        public float GetRequiredWidthSpace(Paint p)
        {
            p.TextSize = TextSize;

            string label = GetLongestLabel();
            var size = p.MeasureWidth(label);
            float width = (float)size + XOffset * 2f;
            float minWidth = MinWidth;
            float maxWidth = MaxWidth;

#if PIXELSCALE
            if (minWidth > 0.0f)
                minWidth = minWidth.DpToPixel();
            if (maxWidth > 0.0f && !float.IsPositiveInfinity(maxWidth))
                maxWidth = maxWidth.DpToPixel();
#endif

            width = Math.Max(minWidth, Math.Min(width, maxWidth > 0.0 ? maxWidth : width));

            return width;
        }

        /// <summary>
        ///  This is for HorizontalBarChart vertical spacing.
        /// </summary>
        /// <param name="p">Paint</param>
        /// <returns></returns>
        public float GetRequiredHeightSpace(Paint p)
        {
            p.TextSize = TextSize;
            return (float)p.MeasureHeight(GetLongestLabel()) + YOffset * 2f;
        }

        /// <summary>
        /// This is for normal(not horizontal) charts horizontal spacing.
        /// </summary>
        public Utils.ChartSize GetRequiredSpace(Paint p)
        {
            p.TextSize = TextSize;

            string label = GetLongestLabel();
            Utils.ChartSize size = p.Measure(label);
            float width = (float)size.Width + XOffset * 2f;
            float height = (float)size.Height + YOffset * 2f;
            float minWidth = MinWidth;
            float maxWidth = MaxWidth;

#if PIXELSCALE
            if (minWidth > 0.0f)
                minWidth = minWidth.DpToPixel();
            if (maxWidth > 0.0f && !float.IsPositiveInfinity(maxWidth))
                maxWidth = maxWidth.DpToPixel();
#endif

            width = Math.Max(minWidth, Math.Min(width, maxWidth > 0.0 ? maxWidth : width));

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


        /// <summary>
        /// Returns true if this axis needs horizontal offset, false if no offset is needed.
        /// </summary>
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

            float range = Math.Abs(max - min);

            // in case all values are equal
            if (range == 0f)
            {
                max += 1f;
                min -= 1f;
            }

            // recalculate
            range = Math.Abs(max - min);

            // calc extra spacing
            axisMinimum = CustomAxisMin ? axisMinimum : min - (range / 100f) * SpacePercentBottom;
            axisMaximum = CustomAxisMax ? axisMaximum : max + (range / 100f) * SpacePercentTop;

            axisRange = Math.Abs(axisMinimum - axisMaximum);
        }
    }
}
