using XF.ChartLibrary.Components;
using XF.ChartLibrary.Utils;

namespace XF.ChartLibrary.Renderer
{
    public partial class YAxisRenderer : AxisRenderer
    {
        protected YAxis YAxis;

        public YAxisRenderer(ViewPortHandler viewPortHandler, YAxis axis, Transformer trans) : base(viewPortHandler, axis, trans)
        {
            YAxis = axis;
        }
    }
}
