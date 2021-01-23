using XF.ChartLibrary.Utils;

namespace XF.ChartLibrary.Renderer
{
    public abstract class ChartRenderer : IRenderer
    {
        protected readonly ViewPortHandler ViewPortHandler;

        ViewPortHandler IRenderer.ViewPortHandler => ViewPortHandler;

        protected ChartRenderer(ViewPortHandler viewPortHandler)
        {
            ViewPortHandler = viewPortHandler;
        }
    }
}
