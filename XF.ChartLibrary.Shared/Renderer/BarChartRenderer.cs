using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;
using XF.ChartLibrary.Data;
using XF.ChartLibrary.Interfaces.DataProvider;
using XF.ChartLibrary.Interfaces.DataSets;
using XF.ChartLibrary.Utils;

namespace XF.ChartLibrary.Renderer
{
    public partial class BarChartRenderer : BarLineScatterCandleBubbleRenderer
    {
        protected IBarDataProvider Chart;

        protected SKPoint[][] BarBuffer;

        public BarChartRenderer(IBarDataProvider chart, Animation.Animator animator,
                                ViewPortHandler viewPortHandler) : base(animator, viewPortHandler)
        {
            Chart = chart;
        }

        public override void InitBuffers()
        {
            var barData = Chart.BarData;
            BarBuffer = new SKPoint[barData.DataSetCount][];

            for (int i = 0; i < BarBuffer.Length; i++)
            {
                var set = barData[i];
                BarBuffer[i] = new SKPoint[set.EntryCount * 2 * (set.IsStacked ? set.StackSize : 1)];
            }
        }

        void PrepareBuffer(IBarDataSet dataSet, int index)
        {
            if (Chart is IBarDataProvider dataProvider && (Chart.BarData is BarData barData))
            {
                var barWidthHalf = barData.BarWidth / 2f;
                var bufferIndex = 0;
                var containsStacks = dataSet.IsStacked;
                var isInverted = dataProvider.IsInverted(axis: dataSet.AxisDependency);
                var phaseY = Animator.PhaseY;
                var size = (float)Math.Ceiling(dataSet.EntryCount * Animator.PhaseX);
                IDataSet<BarEntry> data = dataSet;
                for (int i = 0; i < size; i++)
                {
                    BarEntry e = data[i];

                    if (e == null)
                        continue;

                    var x = e.X;
                    var left = x - barWidthHalf;
                    var right = x + barWidthHalf;

                    var y = e.Y;
                    var vals = e.YVals;
                    if (containsStacks && vals != null)
                    {
                        var posY = 0.0f;
                        var negY = -e.NegativeSum;

                        // fill the stack
                        foreach (var value in vals)
                        {
                            float yStart;
                            if (value == 0.0 && (posY == 0.0 || negY == 0.0))
                            {
                                // Take care of the situation of a 0.0 value, which overlaps a non-zero bar
                                y = value;
                                yStart = y;
                            }
                            else if (value >= 0.0f)
                            {
                                y = posY;
                                yStart = posY + value;
                                posY = yStart;
                            }
                            else
                            {
                                y = negY;
                                yStart = negY + Math.Abs(value);
                                negY += Math.Abs(value);
                            }

                            var top = isInverted
                                ? (y <= yStart ? y : yStart)
                                : (y >= yStart ? y : yStart);
                            var bottom = isInverted
                                ? (y >= yStart ? y : yStart)
                                : (y <= yStart ? y : yStart);

                            // multiply the height of the rect with the phase
                            top *= phaseY;
                            bottom *= phaseY;

                            BarBuffer[index][bufferIndex++] = new SKPoint(left, top);
                            BarBuffer[index][bufferIndex++] = new SKPoint(right, bottom);
                        }
                    }
                    else
                    {
                        float bottom, top;

                        if (isInverted)
                        {
                            bottom = y >= 0 ? y : 0;
                            top = y <= 0 ? y : 0;
                        }
                        else
                        {
                            top = y >= 0 ? y : 0;
                            bottom = y <= 0 ? y : 0;
                        }

                        // multiply the height of the rect with the phase
                        if (top > 0)
                            top *= phaseY;
                        else
                            bottom *= phaseY;
                        BarBuffer[index][bufferIndex++] = new SKPoint(left, top);
                        BarBuffer[index][bufferIndex++] = new SKPoint(right, bottom);
                    }
                }
            }
        }

        public override void DrawExtras(SKCanvas c)
        {
        }
    }
}
