using SkiaSharp;
using System;

namespace XF.ChartLibrary.Charts
{
    partial class PieChart
    {
        public override void OnPaintSurface(SKSurface surface, SKImageInfo e)
        {
            base.OnPaintSurface(surface, e);
            if (data == null)
                return;

            SKCanvas c = surface.Canvas;
            c.Clear(SKColors.Transparent);
            Renderer.DrawData(c);

            if (ValuesToHighlight)
                Renderer.DrawHighlighted(c, IndicesToHighlight);

            Renderer.DrawExtras(c);

            Renderer.DrawValues(c);

            LegendRenderer.RenderLegend(c);

            DrawDescription(c);

            DrawMarkers(c);
        }

        /// <summary>
        /// Sets the color the transparent-circle should have.
        /// </summary>
        /// <param name="color"></param>
        public void SetTransparentCircleColor(SKColor color)
        {
            var p = ((Renderer.PieChartRenderer)renderer).TransparentCirclePaint;
            p.Color = color.WithAlpha(p.Color.Alpha);
        }
        /// <summary>
        /// Sets the amount of transparency the transparent circle should have 0 = fully transparent,
        /// 255 = fully opaque.
        /// Default value is 100.
        /// </summary>
        /// <param name="alpha">0-255</param>
        public void SetTransparentCircleAlpha(byte alpha)
        {
            var p = ((Renderer.PieChartRenderer)renderer).TransparentCirclePaint;
            p.Color = p.Color.WithAlpha(alpha);
        }

        /// <summary>
        /// Sets the color for the hole that is drawn in the center of the PieChart
        /// (if enabled).
        /// </summary>
        /// <param name="color"></param>
        public void SetHoleColor(SKColor color)
        {
            ((Renderer.PieChartRenderer)renderer).HolePaint.Color = color;
        }

        /// <summary>
        ///  Sets the color the entry labels are drawn with.
        /// </summary>
        /// <param name="color"></param>
        public void SetEntryLabelColor(SKColor color)
        {
            ((Renderer.PieChartRenderer)renderer).EntryLabelsPaint.Color = color;
        }

        /// <summary>
        /// Sets a custom Typeface for the drawing of the entry labels.
        /// </summary>
        /// <param name="tf"></param>
        public void SetEntryLabelTypeface(SKTypeface tf)
        {
            ((Renderer.PieChartRenderer)renderer).EntryLabelsPaint.Typeface = tf;
        }

        /// <summary>
        /// Sets the size of the entry labels in dp. Default: 13
        /// </summary>
        /// <param name="size"></param>
        public void SetEntryLabelTextSize(float size)
        {
#if PIXELSCALE
            ((Renderer.PieChartRenderer)renderer).EntryLabelsPaint.TextSize = size.DpToPixel();
#else
            ((Renderer.PieChartRenderer)renderer).EntryLabelsPaint.TextSize = size;
#endif
        }

        protected override SKPoint GetMarkerPosition(Highlight.Highlight value)
        {
            var center = CenterCircleBox;
            float r = Radius;

            float off = r / 10f * 3.6f;

            if (DrawHoleEnabled)
            {
                off = (r - (r / 100f * HoleRadius)) / 2f;
            }

            r -= off; // offset to keep things inside the chart

            float rotationAngle = RotationAngle;

            int entryIndex = (int)value.X;

            // offset needed to center the drawn text in the slice
            float offset = drawAngles[entryIndex] / 2;

            // calculate the text position
            float x = (float)(r
                    * Math.Cos((rotationAngle + absoluteAngles[entryIndex] - offset)
                    * Animator.PhaseY * ChartUtil.FDegToRad) + center.X);
            float y = (float)(r
                    * Math.Sin((rotationAngle + absoluteAngles[entryIndex] - offset)
                    * Animator.PhaseY * ChartUtil.FDegToRad) + center.Y);

            return new SKPoint(x, y);
        }
    }
}
