using SkiaSharp;
using XF.ChartLibrary.Utils;

namespace XF.ChartLibrary.Renderer
{
    public partial class LineRadarRenderer
    {
        /// <summary>
        /// Draws the provided path in filled mode with the provided drawable.
        /// </summary>
        protected void DrawFilledPath(SKCanvas c, SKPath filledPath, SKPaint paint, IFill fill)
        {
            int save = c.Save();
            c.ClipPath(filledPath);
            fill.Draw(c, filledPath, paint, ViewPortHandler.ContentRect);

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
