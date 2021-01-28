using XF.ChartLibrary.Utils;

namespace XF.ChartLibrary.Renderer
{
    public partial class LegendRenderer : ChartRenderer
    {
        protected readonly Components.Legend Legend;

        public LegendRenderer(ViewPortHandler viewPortHandler, Components.Legend legend) : base(viewPortHandler)
        {
            Legend = legend;
        }
    }
}
