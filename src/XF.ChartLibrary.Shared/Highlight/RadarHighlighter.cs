using System;
using System.Collections.Generic;
using System.Text;
using XF.ChartLibrary.Charts;
using XF.ChartLibrary.Data;
using XF.ChartLibrary.Interfaces.DataSets;

namespace XF.ChartLibrary.Highlight
{
    public class RadarHighlighter : PieRadarHighlighter<RadarChart>
    {
        public RadarHighlighter(RadarChart chart) : base(chart)
        {
        }

        protected override Highlight GetClosestHighlight(int index, float x, float y)
        {
            var highlights = GetHighlightsAtIndex(index);

            float distanceToCenter = Chart.DistanceToCenter(x, y) / Chart.Factor;

            Highlight closest = null;
            float distance = float.MaxValue;

            for (int i = 0; i < highlights.Count; i++)
            {

                Highlight high = highlights[i];

                float cdistance = Math.Abs(high.Y - distanceToCenter);
                if (cdistance < distance)
                {
                    closest = high;
                    distance = cdistance;
                }
            }

            return closest;
        }

        private readonly List<Highlight> highlightBuffer = new List<Highlight>();

        /// <summary>
        /// Returns an array of Highlight objects for the given index. The Highlight
        /// objects give information about the value at the selected index and the
        /// DataSet it belongs to. INFORMATION: This method does calculations at <paramref name="index"/>
        /// runtime. Do not over-use in performance critical situations.
        /// <returns></returns>
        protected List<Highlight> GetHighlightsAtIndex(int index)
        {
            highlightBuffer.Clear();

            float phaseX = Chart.Animator.PhaseX;
            float phaseY = Chart.Animator.PhaseY;
            float sliceangle = Chart.SliceAngle;
            float factor = Chart.Factor;

            var data = Chart.Data;
            for (int i = 0; i < data.DataSetCount; i++)
            {
                IDataSet<RadarEntry> dataSet = data[i];

                Entry entry = dataSet[index];

                float y = entry.Y - Chart.YChartMin;

                var pOut = ChartUtil.GetPosition(
                        Chart.CenterOffsets, y * factor * phaseY,
                        sliceangle * index * phaseX + Chart.RotationAngle);

                highlightBuffer.Add(new Highlight(index, entry.Y, pOut.X, pOut.Y, i, dataSet.AxisDependency));
            }

            return highlightBuffer;
        }
    }
}
