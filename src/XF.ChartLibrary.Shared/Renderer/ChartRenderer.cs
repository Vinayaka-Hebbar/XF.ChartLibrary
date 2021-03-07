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
            Initialize();
        }

        /// <summary>
        ///  Initialize renderer
        /// </summary>
        protected virtual void Initialize()
        {
        }
    }
}
