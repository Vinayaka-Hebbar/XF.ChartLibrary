using SkiaSharp;

namespace XF.ChartLibrary.Utils
{
    partial class TransformerHorizontalBarChart
    {
        public override void PrepareMatrixOffset(bool inverted)
        {
            if (!inverted)
                MatrixOffset = SKMatrix.Identity.PostConcat(SKMatrix.CreateTranslation(ViewPortHandler.OffsetLeft,
                        ViewPortHandler.ChartHeight - ViewPortHandler.OffsetBottom));
            else
            {
                MatrixOffset = SKMatrix
                    .CreateTranslation(ViewPortHandler.OffsetRight - ViewPortHandler.ChartWidth, ViewPortHandler.ChartHeight - ViewPortHandler.OffsetTop)
                    .PostConcat(SKMatrix.CreateScale(-1f, 1f));
            }
        }
    }
}
