using SkiaSharp;
using System.Collections.Generic;
using XF.ChartLibrary.Data;
using XF.ChartLibrary.Interfaces.DataSets;
using XF.ChartLibrary.Utils;

namespace XF.ChartLibrary.Renderer
{
    partial class RadarChartRenderer
    {
        /**
     * paint for drawing the web
     */
        private SKPaint webPaint;
        private SKPaint highlightCirclePaint;

        public SKPaint WebPaint
        {
            get => webPaint;
            protected set => webPaint = value;
        }

        public SKPaint HighlightCirclePaint
        {
            get => highlightCirclePaint;
            protected set => highlightCirclePaint = value;
        }

        public override void DrawData(SKCanvas c)
        {
            var radarData = Chart.Data;

            int mostEntries = radarData.GetMaxEntryCountSet().EntryCount;

            foreach (IRadarDataSet set in radarData.DataSets)
            {

                if (set.IsVisible)
                {
                    DrawDataSet(c, set, mostEntries);
                }
            }
        }

        protected SKPath DrawDataSetSurfacePathBuffer = new SKPath();

        protected void DrawDataSet(SKCanvas c, IRadarDataSet dataSet, int mostEntries)
        {
            float phaseX = Animator.PhaseX;
            float phaseY = Animator.PhaseY;

            float sliceangle = Chart.SliceAngle;

            // calculate the factor that is needed for transforming the value to
            // pixels
            float factor = Chart.Factor;

            var center = Chart.CenterOffsets;
            var surface = DrawDataSetSurfacePathBuffer;
            surface.Reset();

            var hasMovedToPoint = false;

            for (int j = 0; j < dataSet.EntryCount; j++)
            {

                RenderPaint.Color = dataSet.ColorAt(j);

                var e = ((IDataSet<RadarEntry>)dataSet)[j];

                var pOut = ChartUtil.GetPosition(
                        center,
                        (e.Y - Chart.YChartMin) * factor * phaseY,
                        sliceangle * j * phaseX + Chart.RotationAngle);

                if (float.IsNaN(pOut.X))
                    continue;

                if (!hasMovedToPoint)
                {
                    surface.MoveTo(pOut.X, pOut.Y);
                    hasMovedToPoint = true;
                }
                else
                    surface.LineTo(pOut.X, pOut.Y);
            }

            if (dataSet.EntryCount > mostEntries)
            {
                // if this is not the largest set, draw a line to the center before closing
                surface.LineTo(center.X, center.Y);
            }

            surface.Close();

            if (dataSet.DrawFilled)
            {

                var drawable = dataSet.Fill;
                if (drawable != null)
                {
                    drawable.Draw(c, surface, RenderPaint, ViewPortHandler.ContentRect);
                }
                else
                {

                    DrawFilledPath(c, surface, dataSet.FillColor, dataSet.FillAlpha);
                }
            }

            RenderPaint.StrokeWidth = dataSet.LineWidth;
            RenderPaint.Style = SKPaintStyle.Stroke;

            // draw the line (only if filled is disabled or alpha is below 255)
            if (!dataSet.DrawFilled || dataSet.FillAlpha < byte.MaxValue)
                c.DrawPath(surface, RenderPaint);
        }

        public override void DrawExtras(SKCanvas c)
        {
            DrawWeb(c);
        }

        protected void DrawWeb(SKCanvas c)
        {
            float sliceangle = Chart.SliceAngle;

            // calculate the factor that is needed for transforming the value to
            // pixels
            float factor = Chart.Factor;
            float rotationangle = Chart.RotationAngle;

            var center = Chart.CenterOffsets;

            // draw the web lines that come from the center
            webPaint.StrokeWidth = Chart.WebLineWidth;
            webPaint.Color = Chart.GetWebColor();

            int xIncrements = 1 + Chart.SkipWebLineCount;
            int maxEntryCount = Chart.Data.GetMaxEntryCountSet().EntryCount;

            for (int i = 0; i < maxEntryCount; i += xIncrements)
            {

                var p = ChartUtil.GetPosition(
                        center,
                        Chart.YRange * factor,
                        sliceangle * i + rotationangle);

                c.DrawLine(center.X, center.Y, p.X, p.Y, webPaint);
            }

            // draw the inner-web
            webPaint.StrokeWidth = Chart.WebLineWidthInner;
            webPaint.Color = Chart.GetWebColorInner();

            int labelCount = Chart.YAxis.entryCount;

            int entryCount = Chart.Data.EntryCount;

            for (int j = 0; j < labelCount; j++)
            {

                for (int i = 0; i < entryCount; i++)
                {

                    float r = (Chart.YAxis.entries[j] - Chart.YChartMin) * factor;

                    var p1out = ChartUtil.GetPosition(center, r, sliceangle * i + rotationangle);
                    var p2out = ChartUtil.GetPosition(center, r, sliceangle * (i + 1) + rotationangle);

                    c.DrawLine(p1out.X, p1out.Y, p2out.X, p2out.Y, webPaint);
                }
            }
        }

        public override void DrawValues(SKCanvas c)
        {
            float phaseX = Animator.PhaseX;
            float phaseY = Animator.PhaseY;

            float sliceangle = Chart.SliceAngle;

            // calculate the factor that is needed for transforming the value to
            // pixels
            float factor = Chart.Factor;

            var center = Chart.CenterOffsets;

            float yoffset = ChartUtil.DpToPixel(5f);

            for (int i = 0; i < Chart.Data.DataSetCount; i++)
            {
                IRadarDataSet dataSet = Chart.Data[i];


                if (!ShouldDrawValues(dataSet))
                    continue;

                // apply the text-styling defined by the DataSet
                ApplyValueTextStyle(dataSet);

                var iconsOffset = dataSet.IconsOffset;
#if PIXELSCALE
                iconsOffset.X = ChartUtil.DpToPixel(iconsOffset.X);
                iconsOffset.Y = ChartUtil.DpToPixel(iconsOffset.Y);
#endif

                for (int j = 0; j < dataSet.EntryCount; j++)
                {

                    RadarEntry entry = ((IDataSet<RadarEntry>)dataSet)[j];

                    var pOut = ChartUtil.GetPosition(
                             center,
                             (entry.Y - Chart.YChartMin) * factor * phaseY,
                             sliceangle * j * phaseX + Chart.RotationAngle);

                    if (dataSet.IsDrawValuesEnabled)
                    {
                        DrawValue(c,
                                dataSet.ValueFormatter,
                                entry.Y,
                                entry,
                                i,
                                pOut.X,
                                pOut.Y - yoffset,
                                dataSet.ValueTextColorAt(j));
                    }

                    if (entry.Icon != null && dataSet.IsDrawIconsEnabled)
                    {
                        var icon = entry.Icon;

                        var pIcon = ChartUtil.GetPosition(
                                 center,
                                 entry.Y * factor * phaseY + iconsOffset.Y,
                                 sliceangle * j * phaseX + Chart.RotationAngle);

                        //noinspection SuspiciousNameCombination
                        pIcon.Y += iconsOffset.X;

                        icon.Draw(c, pIcon.X, pIcon.Y);
                    }
                }

            }

        }

        public override void DrawHighlighted(SKCanvas c, IList<Highlight.Highlight> indices)
        {
            float sliceangle = Chart.SliceAngle;

            // calculate the factor that is needed for transforming the value to
            // pixels
            float factor = Chart.Factor;

            var center = Chart.CenterOffsets;

            RadarData radarData = Chart.Data;

            foreach (var high in indices)
            {

                IRadarDataSet set = radarData[high.DataSetIndex];

                if (set == null || !set.IsHighlightEnabled)
                    continue;

                RadarEntry e = ((IDataSet<RadarEntry>)set)[(int)high.X];

                if (!IsInBoundsX(e, set))
                    continue;

                float y = e.Y - Chart.YChartMin;

               var pOut = ChartUtil.GetPosition(center,
                        y * factor * Animator.PhaseY,
                        sliceangle * high.X * Animator.PhaseX + Chart.RotationAngle);

                high.SetDraw(pOut.X, pOut.Y);

                // draw the lines
                DrawHighlightLines(c, pOut.X, pOut.Y, set);

                if (set.DrawHighlightCircleEnabled)
                {

                    if (!float.IsNaN(pOut.X) && !float.IsNaN(pOut.Y))
                    {
                        var strokeColor = set.HighlightCircleStrokeColor;
                        if (strokeColor == null)
                        {
                            strokeColor = set.ColorAt(0);
                        }

                        DrawHighlightCircle(c,
                                pOut,
                                set.HighlightCircleInnerRadius,
                                set.HighlightCircleOuterRadius,
                                set.HighlightCircleFillColor,
                                strokeColor.Value.WithAlpha(set.HighlightCircleStrokeAlpha),
                                set.HighlightCircleStrokeWidth);
                    }
                }
            }

        }

        protected SKPath DrawHighlightCirclePathBuffer = new SKPath();
        public void DrawHighlightCircle(SKCanvas c,
                                        SKPoint point,
                                        float innerRadius,
                                        float outerRadius,
                                        SKColor? fillColor,
                                        SKColor? strokeColor,
                                        float strokeWidth)
        {
            c.Save();

            outerRadius = ChartUtil.DpToPixel(outerRadius);
            innerRadius = ChartUtil.DpToPixel(innerRadius);

            if (fillColor != null)
            {
                var p = DrawHighlightCirclePathBuffer;
                p.Reset();
                p.AddCircle(point.X, point.Y, outerRadius, SKPathDirection.Clockwise);
                if (innerRadius > 0.0f)
                {
                    p.AddCircle(point.X, point.Y, innerRadius, SKPathDirection.CounterClockwise);
                }
                highlightCirclePaint.Color = fillColor.Value;
                highlightCirclePaint.Style = SKPaintStyle.Fill;
                c.DrawPath(p, highlightCirclePaint);
            }

            if (strokeColor != null)
            {
                highlightCirclePaint.Color = strokeColor.Value;
                highlightCirclePaint.Style = SKPaintStyle.Stroke;
                highlightCirclePaint.StrokeWidth = ChartUtil.DpToPixel(strokeWidth);
                c.DrawCircle(point.X, point.Y, outerRadius, highlightCirclePaint);
            }

            c.Restore();
        }
    }
}
