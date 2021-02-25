using System.Collections.Generic;
using XF.ChartLibrary.Data;
using XF.ChartLibrary.Utils;

#if NETSTANDARD || SKIASHARP
using Color = SkiaSharp.SKColor;
using Alpha = System.Byte;
#elif __IOS__ || __TVOS
    using Color = UIKit.UIColor;
    using Alpha = System.Single;
#elif __ANDROID__
    using Color = Android.Graphics.Color;
    using Alpha = System.Byte;
#endif

namespace XF.ChartLibrary.Interfaces.DataSets
{
    public interface IBarDataSet : IBarLineScatterCandleBubbleDataSet<BarEntry>
    {
#if __ANDROID__ || SKIASHARP
        IList<IRectFill> Fills { get; }

        IRectFill GetFill(int index); 
#endif

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
        Color BarBorderColor { get; }

        /// <summary>
        ///  Returns the alpha value (transparency) that is used for drawing the
        /// highlight indicator.
        /// </summary>
        Alpha HighLightAlpha { get; }

        /// <summary>
        /// Returns the labels used for the different value-stacks in the legend.
        /// This is only relevant for stacked bar entries.
        /// </summary>
        IList<string> StackLabels { get; }
    }
}
