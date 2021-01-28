using XF.ChartLibrary.Components;
using XF.ChartLibrary.Utils;

namespace XF.ChartLibrary.Renderer
{
    public partial class YAxisRenderer : AxisRenderer
    {
        protected YAxis mYAxis;

        public YAxisRenderer(ViewPortHandler viewPortHandler, YAxis axis, Transformer trans) : base(viewPortHandler, axis, trans)
        {
            mYAxis = axis;
        }
    }
}
