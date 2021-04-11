using System;
using XF.ChartLibrary.Components;
using XF.ChartLibrary.Data;
using XF.ChartLibrary.Highlight;
using XF.ChartLibrary.Interfaces.DataSets;
using XF.ChartLibrary.Renderer;

namespace XF.ChartLibrary.Charts
{
    public partial class RadarChart : PieRadarChartBase<RadarData, IRadarDataSet>
    {
        protected YAxisRendererRadarChart YAxisRenderer;
        protected XAxisRendererRadarChart XAxisRenderer;

        public override float Radius
        {
            get
            {
                var content = ViewPortHandler.ContentRect;
                return Math.Min(content.Width / 2f, content.Height / 2f);
            }
        }

        public override float RequiredBaseOffset
        {
            get
            {
                var xaxis = XAxis;
                return xaxis.IsEnabled && xaxis.IsDrawLabelsEnabled ?
               xaxis.LabelRotatedWidth :
#if PIXELSCALE
        10f.DpToPixel();
#else
        10f;
#endif
            }
        }

        public override float RequiredLegendOffset
        {
            get
            {
#if SKIASHARP
                return LegendRenderer.LabelPaint.TextSize * 4.0f;
#else
                // TODO implementation fr ios and android
                throw null;
#endif
            }
        }

        /// <summary>
        /// the factor that is needed to transform values into pixels.
        /// </summary>
        public float Factor
        {
            get
            {
                var content = ViewPortHandler.ContentRect;
                return Math.Min(content.Width / 2f, content.Height / 2f) / YAxis.AxisRange;
            }
        }

        /// <summary>
        ///  Returns the angle that each slice in the radar chart occupies.
        /// </summary>
        public float SliceAngle
        {
            get => 360f / Data.GetMaxEntryCountSet().EntryCount;
        }

        public override void Initialize()
        {
            base.Initialize();

            YAxis = new YAxis(YAxisDependency.Left)
            {
                XLabelOffset = 10f
            };

#if PIXELSCALE
            WebLineWidth = 1.5f.DpToPixel();
            WebLineWidthInner = 0.75f.DpToPixel();
#else
            WebLineWidth = 1.5f;
            InnerWebLineWidth = 0.75f;
#endif
            Renderer = new RadarChartRenderer(this, Animator, ViewPortHandler);
            YAxisRenderer = new YAxisRendererRadarChart(ViewPortHandler, YAxis, this);
            XAxisRenderer = new XAxisRendererRadarChart(ViewPortHandler, XAxis, this);

            highlighter = new RadarHighlighter(this);
        }

        /// <summary>
        /// the range of y-values this chart can display.
        /// </summary>
        public float YRange => YAxis.AxisRange;

        protected override void CalcMinMax()
        {
            base.CalcMinMax();

            RadarData data = Data;
            YAxis.Calculate(data.GetYMin(YAxisDependency.Left), data.GetYMax(YAxisDependency.Left));
            XAxis.Calculate(0, data.GetMaxEntryCountSet().EntryCount);
        }

        public override void NotifyDataSetChanged()
        {
            if (Data is RadarData data)
            {
                CalcMinMax();

                YAxisRenderer.ComputeAxis(YAxis.axisMinimum, YAxis.axisMaximum, YAxis.Inverted);
                XAxisRenderer.ComputeAxis(XAxis.axisMinimum, XAxis.axisMaximum, false);

                if (Legend != null && !Legend.IsLegendCustom)
                    LegendRenderer.ComputeLegend(data);

                CalculateOffsets();
            }
        }

        public override int GetIndexForAngle(float angle)
        {
            // take the current angle of the chart into consideration
            float a = ChartUtil.GetNormalizedAngle(angle - RotationAngle);

            float sliceangle = SliceAngle;

            int max = Data.GetMaxEntryCountSet().EntryCount;

            int index = 0;

            for (int i = 0; i < max; i++)
            {

                float referenceAngle = sliceangle * (i + 1) - sliceangle / 2f;

                if (referenceAngle > a)
                {
                    index = i;
                    break;
                }
            }

            return index;
        }
    }
}
