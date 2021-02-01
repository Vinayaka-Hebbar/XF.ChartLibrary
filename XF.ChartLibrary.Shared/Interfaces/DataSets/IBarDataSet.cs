using System.Collections.Generic;
using XF.ChartLibrary.Data;
using XF.ChartLibrary.Utils;


#if NETSTANDARD || SKIASHARP
using Color = SkiaSharp.SKColor;
#elif __IOS__ || __TVOS
using Color = UIKit.UIColor;
#elif __ANDROID__
    using Color = Android.Graphics.Color;
#endif

namespace XF.ChartLibrary.Interfaces.DataSets
{
    public interface IBarDataSet : IBarLineScatterCandleBubbleDataSet<BarEntry>
    {
        IList<IFill> Fills { get; }

        IFill GetFill(int index);

        /// <summary>
        ///  Returns true if this DataSet is stacked (stacksize > 1) or not.
        /// </summary>
        bool IsStacked { get; }

        /// <summary>
        ///  Returns the maximum number of bars that can be stacked upon another in
        /// this DataSet.This should return 1 for non stacked bars, and > 1 for stacked bars.
        /// </summary>
        /// <returns></returns>
        int StackSize { get; }

        /// <summary>
        ///  Returns the color used for drawing the bar-shadows. The bar shadows is a
        /// surface behind the bar that indicates the maximum value.
        /// </summary>
        Color BarShadowColor { get; }

        /// <summary>
        /// Returns the width used for drawing borders around the bars.
        /// If borderWidth == 0, no border will be drawn.
        /// </summary>
        float BarBorderWidth { get; }

        /// <summary>
        /// Returns the color drawing borders around the bars.
        /// </summary>
        int BarBorderColor { get; }

        /// <summary>
        ///  Returns the alpha value (transparency) that is used for drawing the
        /// highlight indicator.
        /// </summary>
        byte HighLightAlpha { get; }

        /// <summary>
        /// Returns the labels used for the different value-stacks in the legend.
        /// This is only relevant for stacked bar entries.
        /// </summary>
        IList<string> StackLabels { get; }
    }
}
