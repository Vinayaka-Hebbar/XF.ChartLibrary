using XF.ChartLibrary.Components;
using XF.ChartLibrary.Utils;

namespace XF.ChartLibrary.Renderer
{
    public partial class YAxisRendererHorizontalBarChart : YAxisRenderer
    {
        public YAxisRendererHorizontalBarChart(ViewPortHandler viewPortHandler, YAxis axis, Transformer trans) : base(viewPortHandler, axis, trans)
        {
        }

        /// <summary>
        /// Computes the axis values.
        /// </summary>
        /// <param name="min">the minimum y-value in the data object for this axis</param>
        /// <param name="max">the maximum y-value in the data object for this axis</param>
        /// <param name="inverted"></param>
        public override void ComputeAxis(float min, float max, bool inverted)
        {
            // calculate the starting and entry point of the y-labels (depending on
            // zoom / contentrect bounds)
            if (ViewPortHandler.ChartHeight > 10 && !ViewPortHandler.IsFullyZoomedOutX)
            {

                var p1 = Trasformer.ValueByTouchPoint(ViewPortHandler.ContentLeft, ViewPortHandler.ContentTop);
                var p2 = Trasformer.ValueByTouchPoint(ViewPortHandler.ContentRight, ViewPortHandler.ContentTop);

                if (inverted)
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
    }
}
