using XF.ChartLibrary.Data;

namespace XF.ChartLibrary.Utils
{
    public static partial class Extensions
    {
#if SKIASHARP

        /// <summary>
        /// Enables the highlight-line to be drawn in dashed mode, e.g.like this "- - - - - -"
        /// </summary>
        /// <param name="lineLength">the length of the line pieces</param>
        /// <param name="spaceLength">the length of space inbetween the line-pieces</param>
        /// <param name="phase"> offset, in degrees (normally, use 0)</param>
        public static TDataSet EnableDashedHighlightLine<TDataSet>(this TDataSet self, float lineLength, float spaceLength, float phase)  where TDataSet : LineScatterCandleRadarDataSet<Entry>
        {
            self.dashPathEffectHighlight = SkiaSharp.SKPathEffect.CreateDash(new float[] {
                lineLength, spaceLength
            }, phase);
            return self;
        }
#endif
    }
}
