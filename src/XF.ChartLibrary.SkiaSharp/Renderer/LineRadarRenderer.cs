using SkiaSharp;

namespace XF.ChartLibrary.Renderer
{
    public partial class LineRadarRenderer
    {
        /// <summary>
        /// Draws the provided path in filled mode with the provided color and alpha.
        /// Special thanks to Angelo Suzuki (https://github.com/tinsukE) for this.
        protected void DrawFilledPath(SKCanvas c, SKPath filledPath, SKColor fillColor, byte fillAlpha)
        {
            int save = c.Save();
            c.ClipPath(filledPath);
            c.DrawColor(fillColor.WithAlpha(fillAlpha), SKBlendMode.SrcOver);
            c.RestoreToCount(save);
        }
    }
}
