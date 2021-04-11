#nullable enable

#if NETSTANDARD || SKIASHARP
using Alpha = System.Byte;
using Color = SkiaSharp.SKColor;
#elif __IOS__ || __TVOS
using Color = UIKit.UIColor;
using Alpha = System.Single;
#elif __ANDROID__
using Color = Android.Graphics.Color;
using Alpha = System.Byte;
#endif

namespace XF.ChartLibrary.Interfaces.DataSets
{
    public interface IRadarDataSet : ILineRadarDataSet<Data.RadarEntry>, IDataSet
    {
        /// <summary>
        /// flag indicating whether highlight circle should be drawn or not
        /// </summary>
        bool DrawHighlightCircleEnabled { get; set; }

        Color? HighlightCircleFillColor { get; }

        /// <summary>
        /// The stroke color for highlight circle.
        /// </summary>
        Color? HighlightCircleStrokeColor { get; }

        Alpha HighlightCircleStrokeAlpha { get; }

        float HighlightCircleInnerRadius { get; }

        float HighlightCircleOuterRadius { get; }

        float HighlightCircleStrokeWidth { get; }
    }
}
