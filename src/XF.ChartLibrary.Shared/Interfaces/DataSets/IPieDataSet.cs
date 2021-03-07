using XF.ChartLibrary.Data;

#if NETSTANDARD || SKIASHARP
using Color = SkiaSharp.SKColor;
#elif __IOS__ || __TVOS
using Color = UIKit.UIColor;
#elif __ANDROID__
using Color = Android.Graphics.Color;
#endif


namespace XF.ChartLibrary.Interfaces.DataSets
{
    public interface IPieDataSet : IDataSet<PieEntry>, IDataSet
    {
        /// <summary>
        /// Returns the space that is set to be between the piechart-slices of this
        /// DataSet, in pixels.
        /// </summary>
        float SliceSpace { get; }

        /// <summary>
        /// When enabled, slice spacing will be 0.0 when the smallest value is going to be
        ///  smaller than the slice spacing itself.
        /// </summary>
        bool IsAutomaticallyDisableSliceSpacing { get; }

        /// <summary>
        ///  Returns the distance a highlighted piechart slice is "shifted" away from
        /// the chart-center in dp.
        /// </summary>
        float SelectionShift { get; }

        PieDataSet.ValuePosition XValuePosition { get; }
        PieDataSet.ValuePosition YValuePosition { get; }

        /// <summary>
        /// When valuePosition is OutsideSlice, indicates line color
        /// </summary>
        Color ValueLineColor { get; }

        /// <summary>
        /// When valuePosition is OutsideSlice and enabled, line will have the same color as the slice
        /// </summary>
        bool IsUseValueColorForLineEnabled { get; }

        /// <summary>
        /// When valuePosition is OutsideSlice, indicates line width
        /// </summary>
        float ValueLineWidth { get; }

        /// <summary>
        /// When valuePosition is OutsideSlice, indicates offset as percentage out of the slice size
        /// </summary>
        float ValueLinePart1OffsetPercentage { get; }

        /// <summary>
        /// When valuePosition is OutsideSlice, indicates length of first half of the line
        /// </summary>
        float ValueLinePart1Length { get; }

        /// <summary>
        /// When valuePosition is OutsideSlice, indicates length of second half of the line
        /// </summary>
        float ValueLinePart2Length { get; }

        /// <summary>
        /// When valuePosition is OutsideSlice, this allows variable line length
        /// </summary>
        bool IsValueLineVariableLength { get; }

        /// <summary>
        /// Gets the color for the highlighted sector
        /// </summary>
        Color? HighlightColor { get; }
    }
}
