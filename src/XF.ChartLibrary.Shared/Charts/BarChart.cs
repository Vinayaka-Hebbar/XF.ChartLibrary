using XF.ChartLibrary.Data;
using XF.ChartLibrary.Highlight;
using XF.ChartLibrary.Interfaces.DataSets;
using XF.ChartLibrary.Renderer;

#if NETSTANDARD || SKIASHARP
using Rect = SkiaSharp.SKRect;
#elif __IOS__ || __TVOS__
using Rect = CoreGraphics.CGRect;
#elif __ANDROID__
using Rect = Android.Graphics.RectF;
#endif

namespace XF.ChartLibrary.Charts
{
    public partial class BarChart : BarLineChartBase<BarData, IBarDataSet>, Interfaces.DataProvider.IBarDataProvider
    {
        public BarData BarData => data;

        public override void Initialize()
        {
            base.Initialize();
            renderer = new BarChartRenderer(this, Animator, ViewPortHandler);

            Highlighter = new BarHighlighter(this);

            XAxis.SpaceMin = 0.5f;
            XAxis.SpaceMax = 0.5f;
        }

        protected override void CalcMinMax()
        {
            if (FitBars)
            {
                XAxis.Calculate(data.xMin - Data.BarWidth / 2f, data.xMax + data.BarWidth / 2f);
            }
            else
            {
                XAxis.Calculate(data.xMin, data.xMax);
            }

            // calculate axis range (min / max) according to provided data
            AxisLeft.Calculate(data.GetYMin(Components.YAxisDependency.Left), data.GetYMax(Components.YAxisDependency.Left));
            AxisRight.Calculate(data.GetYMin(Components.YAxisDependency.Right), data.GetYMax(Components.YAxisDependency.Right));
        }

        public override Highlight.Highlight GetHighlightByTouchPoint(float x, float y)
        {
            if (data == null)
            {
                System.Diagnostics.Trace.TraceError("Can't select by touch. No data set.");
                return null;
            }
            else
            {
                var h = Highlighter.GetHighlight(x, y);
                if (h == null || !IsHighlightFullBar)
                    return h;

                // For isHighlightFullBarEnabled, remove stackIndex
                return new Highlight.Highlight(h.X, h.Y,
                        h.XPx, h.YPx,
                        h.DataSetIndex, -1, h.Axis);
            }
        }

        /// <summary>
        /// The passed outputRect will be assigned the values of the bounding box of the specified Entry in the specified DataSet.
        /// The rect will be assigned Float.MIN_VALUE in all locations if the Entry could not be found in the charts data.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
#if __ANDROID__ && !SKIASHARP
        public virtual void GetBarBounds(BarEntry e, Rect outputRect) 
#else
        public virtual Rect GetBarBounds(BarEntry e)
#endif
        {
            IBarDataSet set = data.GetDataSetForEntry(e);

            if (set == null)
            {
#if __ANDROID__ && !SKIASHARP
                outputRect.Set(float.MinValue, float.MinValue, float.MinValue, float.MinValue); 
                return;
#else
                return Rect.Empty;
#endif
            }

            float y = e.Y;
            float x = e.X;

            float barWidth = data.BarWidth;

            float left = x - barWidth / 2f;
            float right = x + barWidth / 2f;
            float top = y >= 0 ? y : 0;
            float bottom = y <= 0 ? y : 0;

#if __ANDROID__ && !SKIASHARP
            outputRect.Set(left, top, right, bottom); 
            GetTransformer(set.AxisDependency).RectValueToPixel(outputRect); 
#else
            var outputRect = new Rect(left, top, right, bottom);
            return GetTransformer(set.AxisDependency).RectValueToPixel(outputRect);
#endif
        }

        /// <summary>
        /// Highlights the value at the given x-value in the given DataSet.Provide
        /// -1 as the dataSetIndex to undo all highlighting.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="dataSetIndex"></param>
        /// <param name="stackIndex">the index inside the stack - only relevant for stacked entries</param>
        public void HighlightValue(float x, int dataSetIndex, int stackIndex)
        {
            HighlightValue(new Highlight.Highlight(x, dataSetIndex, stackIndex), false);
        }

        /// <summary>
        ///  Groups all BarDataSet objects this data object holds together by modifying the x-value of their entries.
        /// Previously set x-values of entries will be overwritten. Leaves space between bars and groups as specified
        /// by the parameters.
        /// Calls notifyDataSetChanged() afterwards.
        /// </summary>
        /// <param name="fromX">the starting point on the x-axis where the grouping should begin</param>
        /// <param name="groupSpace">the space between groups of bars in values (not pixels) e.g. 0.8f for bar width 1f</param>
        /// <param name="barSpace">the space between individual bars in values (not pixels) e.g. 0.1f for bar width 1f</param>
        public void GroupBars(float fromX, float groupSpace, float barSpace)
        {

            if (BarData == null)
            {
                throw new System.InvalidOperationException("You need to set data for the chart before grouping bars.");
            }
            else
            {
                BarData.GroupBars(fromX, groupSpace, barSpace);
                NotifyDataSetChanged();
            }
        }
    }
}
