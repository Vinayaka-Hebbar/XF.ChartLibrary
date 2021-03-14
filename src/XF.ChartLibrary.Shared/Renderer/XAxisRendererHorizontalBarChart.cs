using XF.ChartLibrary.Charts;
using XF.ChartLibrary.Components;
using XF.ChartLibrary.Utils;

namespace XF.ChartLibrary.Renderer
{
    public partial class XAxisRendererHorizontalBarChart : XAxisRenderer
    {
        protected BarChart Chart;

        public XAxisRendererHorizontalBarChart(ViewPortHandler viewPortHandler, XAxis axis, Transformer trans, BarChart chart) : base(viewPortHandler, axis, trans)
        {
            Chart = chart;
        }

        public override void ComputeAxis(float min, float max, bool inverted)
        {
            // calculate the starting and entry point of the y-labels (depending on
            // zoom / contentrect bounds)
            if (ViewPortHandler.ContentWidth > 10 && !ViewPortHandler.IsFullyZoomedOutY)
            {

                var p1 = Trasformer.ValueByTouchPoint(ViewPortHandler.ContentLeft, ViewPortHandler.ContentBottom);
                var p2 = Trasformer.ValueByTouchPoint(ViewPortHandler.ContentLeft, ViewPortHandler.ContentTop);

                if (inverted)
                {
                    min = (float)p2.Y;
                    max = (float)p1.Y;
                }
                else
                {

                    min = (float)p1.Y;
                    max = (float)p2.Y;
                }
            }

            ComputeAxisValues(min, max);
        }
    }
}
