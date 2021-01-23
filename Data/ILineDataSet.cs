using System;
using System.Collections.Generic;
using System.Text;
#if __IOS__ || __TVOS
    using Color = UIKit.UIColor;
    using ColorList = UIKit.UIColor;
    using DashPathEffect = XF.ChartLibrary.Utils.DashPathEffect;
#elif __ANDROID__
    using Color = Android.Graphics.Color;
    using ColorList = Android.Graphics.Color;
    using DashPathEffect = Android.Graphics.DashPathEffect;
#elif NETSTANDARD
using Color = SkiaSharp.SKColor;
using DashPathEffect = SkiaSharp.SKPathEffect;
using ColorList = SkiaSharp.SKColors;
#endif
namespace XF.ChartLibrary.Data
{
   public interface ILineDataSet : ILineScatterCandleRadarDataSet<Entry>
    {
        /// <summary>
        /// Returns the drawing mode for this line dataset
        /// </summary>
        LineDataSet.LineMode Mode { get; }

        /**
         * Returns the intensity of the cubic lines (the effect intensity).
         * Max = 1f = very cubic, Min = 0.05f = low cubic effect, Default: 0.2f
         *
         * @return
         */
        float CubicIntensity { get; }

        /**
         * Returns the size of the drawn circles.
         */
        float CircleRadius { get; }

        /**
         * Returns the hole radius of the drawn circles.
         */
        float CircleHoleRadius { get; }

        /**
         * Returns the color at the given index of the DataSet's circle-color array.
         * Performs a IndexOutOfBounds check by modulus.
         *
         * @param index
         * @return
         */
        Color GetCircleColor(int index);

        /**
         * Returns the number of colors in this DataSet's circle-color array.
         *
         * @return
         */
        int CircleColorCount { get; }

        /**
         * Returns true if drawing circles for this DataSet is enabled, false if not
         *
         * @return
         */
        bool IsDrawCirclesEnabled { get; }

        /**
         * Returns the color of the inner circle (the circle-hole).
         *
         * @return
         */
        Color CircleHoleColor { get; }

        /**
         * Returns true if drawing the circle-holes is enabled, false if not.
         *
         * @return
         */
        bool IsDrawCircleHoleEnabled { get; }

        /**
         * Returns the DashPathEffect that is used for drawing the lines.
         *
         * @return
         */
        DashPathEffect DashPathEffect { get; }

        /**
         * Returns true if the dashed-line effect is enabled, false if not.
         * If the DashPathEffect object is null, also return false here.
         *
         * @return
         */
        bool IsDashedLineEnabled { get; }

        /**
         * Returns the IFillFormatter that is set for this DataSet.
         *
         * @return
         */
        Formatter.IFillFormatter FillFormatter { get; }
    }
}
