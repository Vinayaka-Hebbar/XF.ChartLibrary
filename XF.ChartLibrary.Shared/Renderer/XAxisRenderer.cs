using XF.ChartLibrary.Components;
using XF.ChartLibrary.Utils;

namespace XF.ChartLibrary.Renderer
{
    public partial class XAxisRenderer : AxisRenderer
    {
        protected readonly XAxis XAxis;

        public XAxisRenderer(ViewPortHandler viewPortHandler, XAxis axis, Transformer trans) : base(viewPortHandler, axis, trans)
        {
            XAxis = axis;
        }

        public override void ComputeAxis(float min, float max, bool inverted)
        {
            // calculate the starting and entry point of the y-labels (depending on
            // zoom / contentrect bounds)
            if (ViewPortHandler.ContentWidth > 10 && !ViewPortHandler.IsFullyZoomedOutX)
            {

                var p1 = Trasformer.ValueByTouchPoint(ViewPortHandler.ContentLeft, ViewPortHandler.ContentTop);
                var p2 = Trasformer.ValueByTouchPoint(ViewPortHandler.ContentRight, ViewPortHandler.ContentBottom);

                if (!inverted)
                {
                    min = (float)p2.X;
                    max = (float)p1.X;
                }
                else
                {

                    min = (float)p1.X;
                    max = (float)p2.X;
                }
            }

            ComputeAxisValues(min, max);
        }

        protected override void ComputeAxisValues(float min, float max)
        {
            base.ComputeAxisValues(min, max);
            ComputeSize();
        }

        partial void ComputeSize();
    }

}
