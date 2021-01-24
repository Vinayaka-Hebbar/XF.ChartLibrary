using SkiaSharp;

namespace XF.ChartLibrary.Renderer
{
    public partial class LineRadarRenderer
    {
        /// <summary>
        /// Draws the provided path in filled mode with the provided drawable.
        /// </summary>
        protected void DrawFilledPath(SKCanvas c, SKPath filledPath, Utils.IFill fill, byte alpha)
        {
            int save = c.Save();
            c.ClipPath(filledPath);
            fill.Draw(c, ViewPortHandler.ContentRect, alpha);

            c.RestoreToCount(save);
        }
        /// <summary>
        /// Draws the provided path in filled mode with the provided color and alpha.
        /// Special thanks to Angelo Suzuki (https://github.com/tinsukE) for this.
        protected void DrawFilledPath(SKCanvas c, SKPath filledPath, SKColor fillColor, byte fillAlpha)
        {
            int save = c.Save();
            c.ClipPath(filledPath);
            c.DrawColor(fillColor.WithAlpha(fillAlpha));
            c.RestoreToCount(save);
        }
    }
}
