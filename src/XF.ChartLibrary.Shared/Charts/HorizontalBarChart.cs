using System;
using XF.ChartLibrary.Components;
using XF.ChartLibrary.Data;
using XF.ChartLibrary.Highlight;
using XF.ChartLibrary.Interfaces.DataSets;
using XF.ChartLibrary.Renderer;
using XF.ChartLibrary.Utils;

#if NETSTANDARD || SKIASHARP
using Rect = SkiaSharp.SKRect;
using Point = SkiaSharp.SKPoint;
#elif __IOS__ || __TVOS__
using Rect = CoreGraphics.CGRect;
using Point = CoreGraphics.CGPoint;
#elif __ANDROID__
using Rect = Android.Graphics.RectF;
using Point = Android.Graphics.PointF;
#endif

namespace XF.ChartLibrary.Charts
{
    public partial class HorizontalBarChart : BarChart
    {
        public override void Initialize()
        {
            base.Initialize();
            LeftAxisTransformer = new TransformerHorizontalBarChart(ViewPortHandler);
            RightAxisTransformer = new TransformerHorizontalBarChart(ViewPortHandler);

            renderer = new HorizontalBarChartRenderer(this, Animator, ViewPortHandler);
            Highlighter = new HorizontalBarHighlighter(this);

            axisRendererLeft = new YAxisRendererHorizontalBarChart(ViewPortHandler, AxisLeft, LeftAxisTransformer);
            axisRendererRight = new YAxisRendererHorizontalBarChart(ViewPortHandler, AxisRight, RightAxisTransformer);
            xAxisRenderer = new XAxisRendererHorizontalBarChart(ViewPortHandler, XAxis, LeftAxisTransformer, this);
        }

#if __ANDROID__ && !SKIASHARP
        public override void GetBarBounds(BarEntry e, Rect outputRect)
#else
        public override Rect GetBarBounds(BarEntry e)
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

            float top = x - barWidth / 2f;
            float bottom = x + barWidth / 2f;
            float left = y >= 0 ? y : 0;
            float right = y <= 0 ? y : 0;

#if __ANDROID__ && !SKIASHARP
            outputRect.Set(left, top, right, bottom);
            GetTransformer(set.AxisDependency).RectValueToPixel(outputRect);
#else
            var outputRect = new Rect(left, top, right, bottom);
            return GetTransformer(set.AxisDependency).RectValueToPixel(outputRect);
#endif
        }

        public override Point GetPosition(Entry e, YAxisDependency axis)
        {
            if (e == null)
                return default;

            return GetTransformer(axis).PointValueToPixel(e.Y, e.X);
        }

        public override float LowestVisibleX
        {
            get
            {
                var point = GetTransformer(YAxisDependency.Left).ValueByTouchPoint(ViewPortHandler.ContentLeft,
                   ViewPortHandler.ContentBottom);
                return (float)Math.Max(XAxis.axisMinimum, point.Y);
            }
        }

        public override float HighestVisibleX
        {
            get
            {
                var res = GetTransformer(YAxisDependency.Left).ValueByTouchPoint(ViewPortHandler.ContentLeft,
               ViewPortHandler.ContentTop);
                return (float)Math.Min(XAxis.AxisMaximum, res.Y);
            }
        }

        public override float VisibleXRangeMaximum
        {
            set => ViewPortHandler.SetMinimumScaleY(XAxis.axisRange / value);
        }

        public override float VisibleXRangeMinimum
        {
            set => ViewPortHandler.SetMaximumScaleY(XAxis.axisRange / value);
        }

        public override void SetVisibleXRange(float minXRange, float maxXRange)
        {
            float minScale = XAxis.AxisRange / minXRange;
            float maxScale = XAxis.AxisRange / maxXRange;
            ViewPortHandler.SetMinMaxScaleY(minScale, maxScale);
        }

        public override void SetVisibleYRange(float minYRange, float maxYRange, YAxisDependency axis)
        {
            float range = GetAxisRange(axis);
            ViewPortHandler.SetMinMaxScaleX(range / minYRange, range / maxYRange);
        }

        public override void SetVisibleYRangeMaximum(float maxYRange, YAxisDependency axis)
        {
            ViewPortHandler.SetMinimumScaleX(GetAxisRange(axis) / maxYRange);
        }

        public override void SetVisibleYRangeMinimum(float minYRange, YAxisDependency axis)
        {
            ViewPortHandler.SetMaximumScaleX(GetAxisRange(axis) / minYRange);
        }
    }
}
