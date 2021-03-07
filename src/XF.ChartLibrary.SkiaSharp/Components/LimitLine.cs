namespace XF.ChartLibrary.Components
{
    partial class LimitLine
    {
       /// <summary>
       /// Enables the line to be drawn in dashed mode, e.g.like this "- - - - - -"
       /// </summary>
       /// <param name="lineLength">the length of the line pieces</param>
       /// <param name="spaceLength"></param>
       /// <param name="phase">offset, in degrees (normally, use 0)</param>
       /// <returns>Return current instance</returns>
        public LimitLine EnableDashedLine(float lineLength, float spaceLength, float phase)
        {
            dashPathEffect = SkiaSharp.SKPathEffect.CreateDash(new float[] {
                lineLength, spaceLength
            }, phase);
            return this;
        }
    }
}
