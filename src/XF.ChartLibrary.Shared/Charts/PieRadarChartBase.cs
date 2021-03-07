using System;
using XF.ChartLibrary.Interfaces;
using XF.ChartLibrary.Interfaces.DataSets;

using LegendComponent = XF.ChartLibrary.Components.Legend;

#if NETSTANDARD || SKIASHARP
using Point = SkiaSharp.SKPoint;
#elif __IOS__ || __TVOS__
using Point = CoreGraphics.CGPoint;
#elif __ANDROID__
using Point = Android.Graphics.PointF;
#endif

namespace XF.ChartLibrary.Charts
{
    public abstract partial class PieRadarChartBase<TData, TDataSet> : ChartBase<TData, TDataSet>, IPieRadarChartBase where TData : IChartData<TDataSet>, IChartData
        where TDataSet : IDataSet
    {
        /// <summary>
        /// holds the normalized version of the current rotation angle of the chart
        /// </summary>
        private float rotationAngle = 270f;

        /// <summary>
        /// holds the raw version of the current rotation angle of the chart
        /// </summary>
        private float rawRotationAngle = 270f;


        /// <summary>
        ///  Returns the radius of the chart in pixels.
        /// </summary>
        public abstract float Radius { get; }

        /// <summary>
        /// the diameter of the pie- or radar-chart
        /// </summary>
        /// <returns></returns>
        public float Diameter
        {
            get
            {
                var content = ViewPortHandler.ContentRect;
#if APPLE
                Point location = content.Location;
                var size = content.Size;
                location.X += ExtraLeftOffset;
                location.Y += ExtraTopOffset;
                size.Width -= ExtraRightOffset;
                size.Height -= ExtraBottomOffset;
                content.Location = location;
                content.Size = size;
#else
                content.Left += ExtraLeftOffset;
                content.Top += ExtraTopOffset;
                content.Right -= ExtraRightOffset;
                content.Bottom -= ExtraBottomOffset; 
#endif
#if ANDROID
                return Math.Min(content.Width(), content.Height()); 
#else
                return Math.Min((float)content.Width, (float)content.Height);
#endif
            }
        }


        public abstract float RequiredBaseOffset { get; }

        public abstract float RequiredLegendOffset { get; }

        public float MinOffset { get; set; }

        public override float YChartMax
        {
            get
            {
                // TODO Auto-generated method stub
                return 0;
            }
        }

        public override float YChartMin
        {
            get
            {
                // TODO Auto-generated method stub
                return 0;
            }
        }

        /// <summary>
        /// Get or Set an offset for the rotation of the RadarChart in degrees. Default 270f
        /// --> top(NORTH)
        /// </summary>
        /// <param name="angle"></param>
        public float RotationAngle
        {
            get => rotationAngle;
            set
            {
                rawRotationAngle = value;
                rotationAngle = ChartUtil.GetNormalizedAngle(value);
            }
        }

        /// <summary>
        /// Gets the raw version of the current rotation angle of the pie chart the
        /// returned value could be any value, negative or positive, outside of the
        /// 360 degrees. this is used when working with rotation direction, mainly by
        /// gestures and animations.
        /// </summary>
        public float RawRotationAngle
        {
            get => rawRotationAngle;
        }

        protected override void CalcMinMax()
        {

        }

        public abstract int GetIndexForAngle(float angle);

        public override int MaxVisibleCount
        {
            get => data.EntryCount;
            set => throw new NotImplementedException("MaxVisible count not supported for PieRadarChart");
        }

        public override void NotifyDataSetChanged()
        {
            if (data == null)
                return;

            CalcMinMax();

            if (Legend != null)
                LegendRenderer.ComputeLegend(data);

            CalculateOffsets();
        }

        public override void CalculateOffsets()
        {
            float minOffset;
            float legendLeft = 0f, legendRight = 0f, legendBottom = 0f, legendTop = 0f;

            if (Legend is LegendComponent legend && legend.IsEnabled && legend.IsDrawInsideEnabled == false)
            {
                float fullLegendWidth = Math.Min(legend.NeededWidth,
                        ViewPortHandler.ChartWidth * legend.MaxSizePercent);

                switch (legend.Orientation)
                {
                    case Components.Orientation.Vertical:
                        {
                            float xLegendOffset = 0.0f;

                            if (legend.HorizontalAlignment == Components.HorizontalAlignment.Left
                                    || legend.HorizontalAlignment == Components.HorizontalAlignment.Right)
                            {
                                if (legend.VerticalAlignment == Components.VerticalAlignment.Center)
                                {
                                    // this is the space between the legend and the chart
#if PIXELSCALE
                                    float spacing = 13f.DpToPixel();
#else
                                    float spacing = 13f;
#endif

                                    xLegendOffset = fullLegendWidth + spacing;

                                }
                                else
                                {
                                    // this is the space between the legend and the chart
#if PIXELSCALE
                                    // this is the space between the legend and the chart
                                    float spacing = 8f.DpToPixel();
#else
                                    float spacing = 8f;
#endif

                                    float legendWidth = fullLegendWidth + spacing;
                                    float legendHeight = legend.neededHeight + legend.textHeightMax;

                                    float width = ViewPortHandler.ChartWidth;
                                    float height = ViewPortHandler.ChartHeight;
                                    var centerX = width / 2f;
                                    var centerY = height / 2f;

                                    float bottomX = legend.HorizontalAlignment ==
                                            Components.HorizontalAlignment.Right
                                            ? width - legendWidth + 15.0f
                                            : legendWidth - 15.0f;
                                    float bottomY = legendHeight + 15.0f;
                                    float distLegend = DistanceToCenter(bottomX, bottomY);

                                    var reference = GetPosition(centerX, centerY, Radius,
                                            GetAngleForPoint(bottomX, bottomY));

                                    float distReference = DistanceToCenter((float)reference.X, (float)reference.Y);
#if PIXELSCALE
                                    minOffset = 5f.DpToPixel();
#else
                                    minOffset = 5f;
#endif

                                    if (bottomY >= centerY && height - legendWidth > width)
                                    {
                                        xLegendOffset = legendWidth;
                                    }
                                    else if (distLegend < distReference)
                                    {

                                        float diff = distReference - distLegend;
                                        xLegendOffset = minOffset + diff;
                                    }
                                }
                            }

                            switch (legend.HorizontalAlignment)
                            {
                                case Components.HorizontalAlignment.Left:
                                    legendLeft = xLegendOffset;
                                    break;

                                case Components.HorizontalAlignment.Right:
                                    legendRight = xLegendOffset;
                                    break;

                                case Components.HorizontalAlignment.Center:
                                    switch (legend.VerticalAlignment)
                                    {
                                        case Components.VerticalAlignment.Top:
                                            legendTop = Math.Min(legend.neededHeight,
                                                    ViewPortHandler.ChartHeight * legend.MaxSizePercent);
                                            break;
                                        case Components.VerticalAlignment.Bottom:
                                            legendBottom = Math.Min(legend.neededHeight,
                                                    ViewPortHandler.ChartHeight * legend.MaxSizePercent);
                                            break;
                                    }
                                    break;
                            }
                        }
                        break;

                    case Components.Orientation.Horizontal:
                        float yLegendOffset = 0.0f;

                        if (legend.VerticalAlignment == Components.VerticalAlignment.Top ||
                                legend.VerticalAlignment == Components.VerticalAlignment.Bottom)
                        {

                            // It's possible that we do not need this offset anymore as it
                            //   is available through the extraOffsets, but changing it can mean
                            //   changing default visibility for existing apps.
                            float yOffset = RequiredLegendOffset;

                            yLegendOffset = Math.Min(legend.neededHeight + yOffset,
                                    ViewPortHandler.ChartHeight * legend.MaxSizePercent);

                            switch (legend.VerticalAlignment)
                            {
                                case Components.VerticalAlignment.Top:
                                    legendTop = yLegendOffset;
                                    break;
                                case Components.VerticalAlignment.Bottom:
                                    legendBottom = yLegendOffset;
                                    break;
                            }
                        }
                        break;
                }

                float offset = RequiredBaseOffset;
                legendLeft += offset;
                legendRight += RequiredBaseOffset;
                legendTop += RequiredBaseOffset;
                legendBottom += RequiredBaseOffset;
            }

#if PIXELSCALE
            minOffset = MinOffset.DpToPixel();
#else
             minOffset = MinOffset;
#endif

            // todo
            if (this is null)
            {
                var x = this.XAxis;

                if (x.IsEnabled && x.IsDrawLabelsEnabled)
                {
                    minOffset = Math.Max(minOffset, x.LabelRotatedWidth);
                }
            }

            legendTop += ExtraTopOffset;
            legendRight += ExtraRightOffset;
            legendBottom += ExtraBottomOffset;
            legendLeft += ExtraLeftOffset;

            float offsetLeft = Math.Max(minOffset, legendLeft);
            float offsetTop = Math.Max(minOffset, legendTop);
            float offsetRight = Math.Max(minOffset, legendRight);
            float offsetBottom = Math.Max(minOffset, Math.Max(RequiredBaseOffset, legendBottom));

            ViewPortHandler.RestrainViewPort(offsetLeft, offsetTop, offsetRight, offsetBottom);
        }
        /// <summary>
        /// Returns a recyclable MPPointF instance.
        /// Calculates the position around a center point, depending on the distance
        /// from the center, and the angle of the position around the center.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="dist"></param>
        /// <param name="angle">in degrees, converted to radians internally</param>
        /// <returns></returns>
        public Point GetPosition(Point center, float dist, float angle)
        {
            return GetPosition((float)center.X, (float)center.Y, dist, angle);
        }

        public Point GetPosition(float centerX, float centerY, float dist, float angle)
        {
            return new Point((float)(centerX + dist * Math.Sin(angle * ChartUtil.FDegToRad)),
            (float)(centerY + dist * Math.Sin(angle * ChartUtil.FDegToRad)));
        }

        /// <summary>
        /// Returns the distance of a certain point on the chart to the center of the
        /// chart
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public float DistanceToCenter(float x, float y)
        {
            var c = CenterOffsets;

            float xDist;
            if (x > c.X)
            {
                xDist = x - (float)c.X;
            }
            else
            {
                xDist = (float)c.X - x;
            }



            float yDist;
            if (y > c.Y)
            {
                yDist = y - (float)c.Y;
            }
            else
            {
                yDist = (float)c.Y - y;
            }

            // pythagoras
            return (float)Math.Sqrt(Math.Pow(xDist, 2.0) + Math.Pow(yDist, 2.0));
        }

        /// <summary>
        /// returns the angle relative to the chart center for the given point on the
        /// chart in degrees.The angle is always between 0 and 360°, 0° is NORTH,
        /// 90° is EAST, ..
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public float GetAngleForPoint(float x, float y)
        {
            var c = CenterOffsets;
            double tx = x - c.X, ty = y - c.Y;
            double length = Math.Sqrt(tx * tx + ty * ty);

            float angle = (float)(Math.Acos(ty / length) * ChartUtil.AngToDeg);

            if (x > c.X)
                angle = 360f - angle;

            // add 90° because chart starts EAST
            angle += 90f;

            // neutralize overflow
            if (angle > 360f)
                angle -= 360f;

            return angle;
        }

    }
}
