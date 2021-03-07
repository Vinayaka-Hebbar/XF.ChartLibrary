using XF.ChartLibrary.Data;

#if NETSTANDARD || SKIASHARP
using Color = SkiaSharp.SKColor;
using DashPathEffect = SkiaSharp.SKPathEffect;
using ColorList = SkiaSharp.SKColors;
#elif __IOS__ || __TVOS
using Color = UIKit.UIColor;
    using ColorList = UIKit.UIColor;
    using DashPathEffect = XF.ChartLibrary.Utils.DashPathEffect;
#elif __ANDROID__
using Color = Android.Graphics.Color;
using ColorList = Android.Graphics.Color;
using DashPathEffect = Android.Graphics.DashPathEffect;
#endif
namespace XF.ChartLibrary.Interfaces.DataSets
{
    public interface ILineDataSet : ILineRadarDataSet<Entry>
    {
        /// <summary>
        /// Returns the drawing mode for this line dataset
        /// </summary>
        LineDataSet.LineMode Mode { get; }

        /// <summary>
        /// Returns the intensity of the cubic lines (the effect intensity).
        /// Max = 1f = very cubic, Min = 0.05f = low cubic effect, Default: 0.2f
        /// </summary>
        float CubicIntensity { get; }

        /// <summary>
        /// Returns the size of the drawn circles.
        /// </summary>
        float CircleRadius { get; }

        /**
         * Returns the hole radius of the drawn circles.
         */
        float CircleHoleRadius { get; }

        /// <summary>
        /// Returns the color at the given index of the DataSet's circle-color array.
        /// Performs a IndexOutOfBounds check by modulus.
        /// </summary>
        /// <param name="index">color index</param>
        /// <returns>the color at the given index of the DataSet's circle-color array.</returns>
        Color GetCircleColor(int index);

        /// <summary>
        /// Returns the number of colors in this DataSet's circle-color array.
        /// </summary>
        int CircleColorCount { get; }

        /// <summary>
        /// Returns true if drawing circles for this DataSet is enabled, false if not
        /// </summary>
        bool IsDrawCirclesEnabled { get; }

        /// <summary>
        /// Returns the color of the inner circle (the circle-hole).
        /// </summary>
        Color CircleHoleColor { get; }

        /// <summary>
        /// Returns true if drawing the circle-holes is enabled, false if not.
        /// </summary>
        bool IsDrawCircleHoleEnabled { get; }

        /**
         * Returns the DashPathEffect that is used for drawing the lines.
         *
         * @return
         */
        DashPathEffect DashPathEffect { get; }

        /// <summary>
        /// Returns true if the dashed-line effect is enabled, false if not.
        /// If the <see cref="DashPathEffect"/> object is null, also return false here.
        /// </summary>
        bool IsDashedLineEnabled { get; }

        /// <summary>
        /// Returns the IFillFormatter that is set for this DataSet.
        /// </summary>
        Formatter.IFillFormatter FillFormatter { get; }
    }
}
