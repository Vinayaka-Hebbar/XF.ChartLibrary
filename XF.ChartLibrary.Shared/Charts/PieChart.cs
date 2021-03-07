using System;
using XF.ChartLibrary.Data;
using XF.ChartLibrary.Interfaces.DataSets;
using System.Collections.Generic;

#if NETSTANDARD || SKIASHARP
using Rect = SkiaSharp.SKRect;
using Point = SkiaSharp.SKPoint;
using Color = SkiaSharp.SKColor;
#elif __IOS__ || __TVOS__
using Rect = CoreGraphics.CGRect;
using Point = CoreGraphics.CGPoint;
using Color = UIKit.UIColor;
#elif __ANDROID__
using Rect = Android.Graphics.RectF;
using Point = Android.Graphics.PointF;
using Color = Android.Graphics.Color;
#endif


namespace XF.ChartLibrary.Charts
{
    public partial class PieChart : PieRadarChartBase<PieData, IPieDataSet>
    {
        internal Rect circleBox = new Rect();

        internal Point centerTextOffset = new Point();

        /// <summary>
        /// array that holds the width of each pie-slice in degrees
        /// </summary>
        private float[] drawAngles = new float[1];

        /// <summary>
        /// array that holds the absolute angle in degrees of each slice
        /// </summary>
        private float[] absoluteAngles = new float[1];

        public override void Initialize()
        {
            base.Initialize();
            XAxis = null;
            renderer = new Renderer.PieChartRenderer(this, Animator, ViewPortHandler);
            highlighter = new Highlight.PieHighligher(this);
        }

        public Rect CircleBox => circleBox;

        /// <summary>
        /// Returns an integer array of all the different angles the chart slices
        /// have the angles in the returned array determine how much space(of 360°)
        /// each slice takes
        /// </summary>
        public float[] DrawAngles => drawAngles;

        /// <summary>
        /// Returns the absolute angles of the different chart slices (where the
        /// slices end)
        /// </summary>
        /// <returns></returns>
        public float[] AbsoluteAngles
        {
            get => absoluteAngles;
        }

        public override float Radius
        {
            get
            {
                if (circleBox.IsEmpty)
                    return 0;
#if __ANDROID__ && !SKIASHARP
                return Math.Min(circleBox.Width() / 2f, circleBox.Height() / 2f);
#else
                return (float)Math.Min(circleBox.Width / 2f, circleBox.Height / 2f);
#endif
            }
        }

        public override float RequiredBaseOffset => 0;

        public override void CalculateOffsets()
        {
            base.CalculateOffsets();

            // prevent nullpointer when no data set
            if (!(Data is PieData data))
                return;

            float diameter = Diameter;
            float radius = diameter / 2f;

            var c = CenterOffsets;

            float shift = data.DataSet.SelectionShift;

            // create the circle box that will contain the pie-chart (the bounds of
            // the pie-chart)

#if ANDROID
            circleBox.Set(c.X - radius + shift,
                       c.Y - radius + shift,
                       c.X + radius - shift,
                       c.Y + radius - shift);
#else
            circleBox = new Rect(c.X - radius + shift,
                       c.Y - radius + shift,
                       c.X + radius - shift,
                       c.Y + radius - shift);
#endif
        }

        protected override void CalcMinMax()
        {
            CalcAngles();
        }

        /// <summary>
        /// Checks if the given index is set to be highlighted.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool NeedsHighlight(int index)
        {

            // no highlight
            if (!ValuesToHighlight)
                return false;

            foreach (Highlight.Highlight h in indicesToHighlight)

                // check if the xvalue for the given dataset needs highlight
                if ((int)h.X == index)
                    return true;

            return false;
        }


        /// <summary>
        /// Calculates the needed angle for a given value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private float CalcAngle(float value)
        {
            return CalcAngle(value, data.YValueSum);
        }

        /// <summary>
        /// Calculates the needed angle for a given value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="yValueSum"></param>
        /// <returns></returns>
        private float CalcAngle(float value, float yValueSum)
        {
            return value / yValueSum * MaxAngle;
        }

        public override int GetIndexForAngle(float angle)
        {

            // take the current angle of the chart into consideration
            float a = ChartUtil.GetNormalizedAngle(angle - RotationAngle);

            for (int i = 0; i < absoluteAngles.Length; i++)
            {
                if (absoluteAngles[i] > a)
                    return i;
            }

            return -1; // return -1 if no index found
        }

        /// <summary>
        /// Returns the index of the DataSet this x-index belongs to.
        /// </summary>
        /// <param name="xIndex"></param>
        /// <returns></returns>
        public int GetDataSetIndexForIndex(int xIndex)
        {
            int count = data.DataSets.Count;
            for (int i = 0; i < count; i++)
            {
                if (((IDataSet<PieEntry>)data.DataSets[i]).EntryForXValue(xIndex, float.NaN) != null)
                    return i;
            }

            return -1;
        }
        /// <summary>
        /// Calculates the needed angles for the chart slices
        /// </summary>
        private void CalcAngles()
        {
            var data = Data;
            int entryCount = data.EntryCount;

            if (drawAngles.Length != entryCount)
            {
                drawAngles = new float[entryCount];
            }
            else
            {
                for (int i = 0; i < entryCount; i++)
                {
                    drawAngles[i] = 0;
                }
            }
            if (absoluteAngles.Length != entryCount)
            {
                absoluteAngles = new float[entryCount];
            }
            else
            {
                for (int i = 0; i < entryCount; i++)
                {
                    absoluteAngles[i] = 0;
                }
            }

            float yValueSum = data.YValueSum;

            IList<IPieDataSet> dataSets = data.DataSets;

            float minAngleForSlices = MinAngleForSlices;
            bool hasMinAngle = minAngleForSlices != 0f && entryCount * minAngleForSlices <= MaxAngle;
            float[] minAngles = new float[entryCount];

            int cnt = 0;
            float offset = 0f;
            float diff = 0f;

            for (int i = 0; i < data.DataSetCount; i++)
            {
                IDataSet<PieEntry> set = dataSets[i];

                for (int j = 0; j < set.EntryCount; j++)
                {

                    float drawAngle = CalcAngle(Math.Abs(set[j].Y), yValueSum);

                    if (hasMinAngle)
                    {
                        float temp = drawAngle - minAngleForSlices;
                        if (temp <= 0)
                        {
                            minAngles[cnt] = minAngleForSlices;
                            offset += -temp;
                        }
                        else
                        {
                            minAngles[cnt] = drawAngle;
                            diff += temp;
                        }
                    }

                    drawAngles[cnt] = drawAngle;

                    if (cnt == 0)
                    {
                        absoluteAngles[cnt] = drawAngles[cnt];
                    }
                    else
                    {
                        absoluteAngles[cnt] = absoluteAngles[cnt - 1] + drawAngles[cnt];
                    }

                    cnt++;
                }
            }

            if (hasMinAngle)
            {
                // Correct bigger slices by relatively reducing their angles based on the total angle needed to subtract
                // This requires that `entryCount * mMinAngleForSlices <= mMaxAngle` be true to properly work!
                for (int i = 0; i < entryCount; i++)
                {
                    minAngles[i] -= (minAngles[i] - minAngleForSlices) / diff * offset;
                    if (i == 0)
                    {
                        absoluteAngles[0] = minAngles[0];
                    }
                    else
                    {
                        absoluteAngles[i] = absoluteAngles[i - 1] + minAngles[i];
                    }
                }

                drawAngles = minAngles;
            }
        }

        /// <summary>
        /// Sets the offset the center text should have from it's original position in dp. Default x = 0, y = 0
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SetCenterTextOffset(float x, float y)
        {
#if PIXELSCALE
            centerTextOffset.X = x.DpToPixel();
            centerTextOffset.Y = y.DpToPixel();
#else
            centerTextOffset.X = x;
            centerTextOffset.Y = y; 
#endif
        }

        /// <summary>
        /// Returns the offset on the x- and y-axis the center text has in dp.
        /// </summary>
        public Point CenterTextOffset
        {
            get
            {
#if __ANDROID__ && !SKIASHARP
            return new Point(centerTextOffset.X, centerTextOffset.Y);
#else
                return centerTextOffset;
#endif
            }
        }

        /// <summary>
        /// the center of the circlebox
        /// </summary>
        public Point CenterCircleBox
        {
            get
            {
#if SKIASHARP
                return new Point(circleBox.MidX, circleBox.MidY);
#elif __ANDROID__
            return new Point(circleBox.CenterX(), circleBox.CenterY());
#elif __IOS__ || __TVOS__
                return new Point(circleBox.Left + (circleBox.Width / 2), circleBox.Top + (circleBox.Height / 2));
#endif
            }
        }
    }
}
