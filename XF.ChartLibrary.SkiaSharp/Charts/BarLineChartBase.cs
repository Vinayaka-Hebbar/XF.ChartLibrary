using SkiaSharp;
using System;
using XF.ChartLibrary.Components;

namespace XF.ChartLibrary.Charts
{
    partial class BarLineChartBase<TData, TDataSet>
    {
        /// <summary>
        /// paint object for the (by default) lightgrey background of the grid
        /// </summary>
        protected SKPaint mGridBackgroundPaint;

        protected SKPaint mBorderPaint;

        public bool DragXEnabled
        {
            get => (bool)GetValue(DragXEnabledProperty);
            set => SetValue(DragXEnabledProperty, value);
        }

        public bool DragYEnabled
        {
            get => (bool)GetValue(DragYEnabledProperty);
            set => SetValue(DragYEnabledProperty, value);
        }

        public bool HighlightPerDragEnabled
        {
            get => (bool)GetValue(HighlightPerDragEnabledProperty);
            set => SetValue(HighlightPerDragEnabledProperty, value);
        }


        /// <summary>
        /// the object representing the labels on the left y-axis
        /// </summary>
        public YAxis AxisLeft
        {
            get => (YAxis)GetValue(AxisLeftProperty);
            set => SetValue(AxisLeftProperty, value);
        }

        /// <summary>
        /// the object representing the labels on the right y-axis
        /// </summary>
        public YAxis AxisRight
        {
            get => (YAxis)GetValue(AxisRightProperty);
            set => SetValue(AxisRightProperty, value);
        }

        public override int MaxVisibleCount
        {
            get => (int)GetValue(MaxVisibleCountProperty);
            set => SetValue(MaxVisibleCountProperty, value);
        }

        /// <summary>
        /// Draws the grid background
        /// </summary>
        protected void DrawGridBackground(SKCanvas c)
        {
            if (IsDrawGridBackground)
            {
                // draw the grid background
                c.DrawRect(ViewPortHandler.ContentRect, mGridBackgroundPaint);
            }

            if (mDrawBorders)
            {
                c.DrawRect(ViewPortHandler.ContentRect, mBorderPaint);
            }
        }


        protected override void CalculateOffsets()
        {
            if (!customViewPortEnabled)
            {
                var offset = CalculateLegendOffsets();

                var offsetLeft = offset.Left;
                var offsetTop = offset.Top;
                var offsetRight = offset.Right;
                var offsetBottom = offset.Bottom;

                // offsets for y-labels
                if (AxisLeft.NeedsOffset)
                {
                    offsetLeft += AxisLeft.GetRequiredWidthSpace(mAxisRendererLeft
                            .AxisLabelPaint);
                }

                if (AxisRight.NeedsOffset)
                {
                    offsetRight += AxisRight.GetRequiredWidthSpace(mAxisRendererRight.AxisLabelPaint);
                }

                if (XAxis.IsEnabled && XAxis.IsDrawLabelsEnabled)
                {

                    float xLabelHeight = XAxis.LabelRotatedHeight + XAxis.YOffset;

                    // offsets for x-labels
                    if (XAxis.Position == XAxis.XAxisPosition.Bottom)
                    {

                        offsetBottom += xLabelHeight;

                    }
                    else if (XAxis.Position == XAxis.XAxisPosition.Top)
                    {

                        offsetTop += xLabelHeight;

                    }
                    else if (XAxis.Position == XAxis.XAxisPosition.BottomInside)
                    {

                        offsetBottom += xLabelHeight;
                        offsetTop += xLabelHeight;
                    }
                }

                offsetTop += ExtraTopOffset;
                offsetRight += ExtraRightOffset;
                offsetBottom += ExtraBottomOffset;
                offsetLeft += ExtraLeftOffset;

                float minOffset = MinOffset.DpToPixel();

                ViewPortHandler.RestrainViewPort(
                        Math.Max(minOffset, offsetLeft),
                        Math.Max(minOffset, offsetTop),
                        Math.Max(minOffset, offsetRight),
                        Math.Max(minOffset, offsetBottom));
            }

            PrepareOffsetMatrix();
            PrepareValuePxMatrix();
        }

        public override void OnPaintSurface(SKSurface surface, SKImageInfo info)
        {
            if (data == null)
            {
                base.OnPaintSurface(surface, info);
                return;
            }

            var canvas = surface.Canvas;
            canvas.Clear(SKColors.Transparent);
            // execute all drawing commands
            DrawGridBackground(canvas);

            if (mAutoScaleMinMaxEnabled)
            {
                AutoScale();
            }

            if (AxisLeft.IsEnabled)
                mAxisRendererLeft.ComputeAxis(AxisLeft.axisMinimum, AxisLeft.axisMaximum, AxisLeft.Inverted);

            if (AxisRight.IsEnabled)
                mAxisRendererRight.ComputeAxis(AxisRight.axisMinimum, AxisRight.axisMaximum, AxisRight.Inverted);

            if (XAxis.IsEnabled)
                mXAxisRenderer.ComputeAxis(XAxis.axisMinimum, XAxis.axisMaximum, false);

            mXAxisRenderer.RenderAxisLine(canvas);
            mAxisRendererLeft.RenderAxisLine(canvas);
            mAxisRendererRight.RenderAxisLine(canvas);

            if (XAxis.IsDrawGridLinesBehindDataEnabled)
                mXAxisRenderer.RenderGridLines(canvas);

            if (AxisLeft.IsDrawGridLinesBehindDataEnabled)
                mAxisRendererLeft.RenderGridLines(canvas);

            if (AxisRight.IsDrawGridLinesBehindDataEnabled)
                mAxisRendererRight.RenderGridLines(canvas);

            if (XAxis.IsEnabled && XAxis.DrawLimitLinesBehindData)
                mXAxisRenderer.RenderLimitLines(canvas);

            if (AxisLeft.IsEnabled && AxisLeft.DrawLimitLinesBehindData)
                mAxisRendererLeft.RenderLimitLines(canvas);

            if (AxisRight.IsEnabled && AxisRight.DrawLimitLinesBehindData)
                mAxisRendererRight.RenderLimitLines(canvas);

            int clipRestoreCount = canvas.Save();

            if (mClipDataToContent)
            {
                // make sure the data cannot be drawn outside the content-rect
                canvas.ClipRect(ViewPortHandler.ContentRect);
            }

            Renderer.DrawData(canvas);

            if (!XAxis.IsDrawGridLinesBehindDataEnabled)
                mXAxisRenderer.RenderGridLines(canvas);

            if (!AxisLeft.IsDrawGridLinesBehindDataEnabled)
                mAxisRendererLeft.RenderGridLines(canvas);

            if (!AxisRight.IsDrawGridLinesBehindDataEnabled)
                mAxisRendererRight.RenderGridLines(canvas);

            // if highlighting is enabled
            if (ValuesToHighlight)
                Renderer.DrawHighlighted(canvas, indicesToHighlight);

            // Removes clipping rectangle
            canvas.RestoreToCount(clipRestoreCount);

            Renderer.DrawExtras(canvas);

            if (XAxis.IsEnabled && !XAxis.DrawLimitLinesBehindData)
                mXAxisRenderer.RenderLimitLines(canvas);

            if (AxisLeft.IsEnabled && !AxisLeft.DrawLimitLinesBehindData)
                mAxisRendererLeft.RenderLimitLines(canvas);

            if (AxisRight.IsEnabled && !AxisRight.DrawLimitLinesBehindData)
                mAxisRendererRight.RenderLimitLines(canvas);

            mXAxisRenderer.RenderAxisLabels(canvas);
            mAxisRendererLeft.RenderAxisLabels(canvas);
            mAxisRendererRight.RenderAxisLabels(canvas);

            if (mClipValuesToContent)
            {
                clipRestoreCount = canvas.Save();
                canvas.ClipRect(ViewPortHandler.ContentRect);

                Renderer.DrawValues(canvas);

                canvas.RestoreToCount(clipRestoreCount);
            }
            else
            {
                Renderer.DrawValues(canvas);
            }

            LegendRenderer.RenderLegend(canvas);

            DrawDescription(canvas);

            DrawMarkers(canvas);
        }
    }
}
