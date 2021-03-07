using SkiaSharp;
using System;
using System.Collections.Generic;
using XF.ChartLibrary.Interfaces.DataSets;

namespace XF.ChartLibrary.Renderer
{
    partial class PieChartRenderer
    {
        /// <summary>
        /// paint for the hole in the center of the pie chart and the transparent
        /// circle
        /// </summary>
        protected SKPaint holePaint;
        private SKPaint transparentCirclePaint;
        protected SKPaint ValueLinePaint;

        /// <summary>
        /// paint object used for drwing the slice-text
        /// </summary>
        private SKPaint entryLabelsPaint;
        /// <summary>
        /// Bitmap object used for drawing the paths (otherwise they are too long if
        /// rendered directly on the canvas)
        /// </summary>
        protected WeakReference<SKBitmap> DrawBitmap;

        /// <summary>
        /// protected on this canvas, the paths are rendered, it is initialized with the
        /// pathBitmap
        /// </summary>
        protected SKCanvas BitmapCanvas;

        public SKPaint TransparentCirclePaint
        {
            get => transparentCirclePaint;
            protected set => transparentCirclePaint = value;
        }

        public SKPaint HolePaint
        {
            get => holePaint;
            protected set => holePaint = value;
        }

        public SKPaint EntryLabelsPaint
        {
            get => entryLabelsPaint;
            protected set => entryLabelsPaint = value;
        }

        protected override void Initialize()
        {
            base.Initialize();
            holePaint = new SKPaint
            {
                IsAntialias = true,
                Color = SKColors.White,
                Style = SKPaintStyle.Fill
            };

            transparentCirclePaint = new SKPaint
            {
                IsAntialias = true,
                Color = SKColors.White.WithAlpha(105),
                Style = SKPaintStyle.Fill
            };

            ValuePaint.TextSize = 13f.DpToPixel();
            ValuePaint.Color = SKColors.White;
            ValuePaint.TextAlign = SKTextAlign.Center;

            entryLabelsPaint = new SKPaint
            {
                IsAntialias = true,
                Color = SKColors.White,
                TextAlign = SKTextAlign.Center,
                TextSize = 13f.DpToPixel()
            };

            ValueLinePaint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                IsAntialias = true
            };
        }

        public override void DrawData(SKCanvas c)
        {
            int width = (int)ViewPortHandler.ChartWidth;
            int height = (int)ViewPortHandler.ChartHeight;
            if (DrawBitmap == null || DrawBitmap.TryGetTarget(out SKBitmap drawBitmap) == false
                    || (drawBitmap.Width != width)
                    || (drawBitmap.Height != height))
            {

                if (width > 0 && height > 0)
                {
                    drawBitmap = new SKBitmap(width, height);
                    DrawBitmap = new WeakReference<SKBitmap>(drawBitmap);
                    BitmapCanvas = new SKCanvas(drawBitmap);
                }
                else
                {
                    return;
                }
            }

            drawBitmap.Erase(SKColors.Transparent);

            var lineData = Chart.Data;

            foreach (var set in lineData.DataSets)
            {
                if (set.IsVisible && set.EntryCount > 0)
                    DrawDataSet(c, set);
            }

            c.DrawBitmap(drawBitmap, 0, 0, RenderPaint);
        }

        private readonly SKPath PathBuffer = new SKPath();

        protected void DrawDataSet(SKCanvas c, IPieDataSet dataSet)
        {
            float angle = 0;
            float rotationAngle = Chart.RotationAngle;

            float phaseX = Animator.PhaseX;
            float phaseY = Animator.PhaseY;

            var circleBox = Chart.CircleBox;

            int entryCount = dataSet.EntryCount;
            float[] drawAngles = Chart.DrawAngles;
            var center = Chart.CenterCircleBox;
            float radius = Chart.Radius;
            bool drawInnerArc = Chart.DrawHoleEnabled && !Chart.DrawSlicesUnderHoleEnabled;
            float userInnerRadius = drawInnerArc
                    ? radius * (Chart.HoleRadius / 100.0f)
                    : 0.0f;
            float roundedRadius = (radius - (radius * Chart.HoleRadius / 100f)) / 2f;
            var roundedCircleBox = SKRect.Empty;
            bool drawRoundedSlices = drawInnerArc && Chart.DrawRoundedSlicesEnabled;

            int visibleAngleCount = 0;
            IDataSet<Data.PieEntry> set = dataSet;
            for (int j = 0; j < entryCount; j++)
            {
                // draw only if the value is greater than zero
                if ((Math.Abs(set[j].Y) > float.Epsilon))
                {
                    visibleAngleCount++;
                }
            }

            float sliceSpace = visibleAngleCount <= 1 ? 0.0f : GetSliceSpace(dataSet);

            for (int j = 0; j < entryCount; j++)
            {

                float sliceAngle = drawAngles[j];
                float innerRadius = userInnerRadius;

                var e = set[j];

                // draw only if the value is greater than zero
                if (!(Math.Abs(e.Y) > float.Epsilon))
                {
                    angle += sliceAngle * phaseX;
                    continue;
                }

                // Don't draw if it's highlighted, unless the chart uses rounded slices
                if (dataSet.IsHighlightEnabled && Chart.NeedsHighlight(j) && !drawRoundedSlices)
                {
                    angle += sliceAngle * phaseX;
                    continue;
                }

                bool accountForSliceSpacing = sliceSpace > 0.0f && sliceAngle <= 180.0f;

                RenderPaint.Color = dataSet.ColorAt(j);

                float sliceSpaceAngleOuter = visibleAngleCount == 1 ?
                        0.0f :
                        sliceSpace / (ChartUtil.FDegToRad * radius);
                float startAngleOuter = rotationAngle + (angle + sliceSpaceAngleOuter / 2.0f) * phaseY;
                float sweepAngleOuter = (sliceAngle - sliceSpaceAngleOuter) * phaseY;
                if (sweepAngleOuter < 0.0f)
                {
                    sweepAngleOuter = 0.0f;
                }

                PathBuffer.Reset();

                if (drawRoundedSlices)
                {
                    float x = center.X + (radius - roundedRadius) * (float)Math.Cos(startAngleOuter * ChartUtil.FDegToRad);
                    float y = center.Y + (radius - roundedRadius) * (float)Math.Sin(startAngleOuter * ChartUtil.FDegToRad);
                    roundedCircleBox = new SKRect(x - roundedRadius, y - roundedRadius, x + roundedRadius, y + roundedRadius);
                }

                float arcStartPointX = center.X + radius * (float)Math.Cos(startAngleOuter * ChartUtil.FDegToRad);
                float arcStartPointY = center.Y + radius * (float)Math.Sin(startAngleOuter * ChartUtil.FDegToRad);

                if (sweepAngleOuter >= 360.0f && sweepAngleOuter % 360f <= float.Epsilon)
                {
                    // Android is doing "mod 360"
                    PathBuffer.AddCircle(center.X, center.Y, radius, SKPathDirection.Clockwise);
                }
                else
                {

                    if (drawRoundedSlices)
                    {
                        PathBuffer.ArcTo(oval: roundedCircleBox, startAngleOuter + 180, -180, false);
                    }

                    PathBuffer.ArcTo(
                            circleBox,
                            startAngleOuter,
                            sweepAngleOuter, false);
                }

                // API < 21 does not receive floats in addArc, but a RectF
                var innerRect = new SKRect(
                          center.X - innerRadius,
                          center.Y - innerRadius,
                          center.X + innerRadius,
                          center.Y + innerRadius);

                if (drawInnerArc && (innerRadius > 0.0f || accountForSliceSpacing))
                {

                    if (accountForSliceSpacing)
                    {
                        float minSpacedRadius =
                                CalculateMinimumRadiusForSpacedSlice(
                                        center, radius,
                                        sliceAngle * phaseY,
                                        arcStartPointX, arcStartPointY,
                                        startAngleOuter,
                                        sweepAngleOuter);

                        if (minSpacedRadius < 0.0f)
                            minSpacedRadius = -minSpacedRadius;

                        innerRadius = Math.Max(innerRadius, minSpacedRadius);
                    }

                    float sliceSpaceAngleInner = visibleAngleCount == 1 || innerRadius == 0.0f ?
                            0.0f :
                            sliceSpace / (ChartUtil.FDegToRad * innerRadius);
                    float startAngleInner = rotationAngle + (angle + sliceSpaceAngleInner / 2.0f) * phaseY;
                    float sweepAngleInner = (sliceAngle - sliceSpaceAngleInner) * phaseY;
                    if (sweepAngleInner < 0.0f)
                    {
                        sweepAngleInner = 0.0f;
                    }
                    float endAngleInner = startAngleInner + sweepAngleInner;

                    if (sweepAngleOuter >= 360.0f && sweepAngleOuter % 360f <= float.Epsilon)
                    {
                        // Android is doing "mod 360"
                        PathBuffer.AddCircle(center.X, center.Y, innerRadius, SKPathDirection.CounterClockwise);
                    }
                    else
                    {

                        if (drawRoundedSlices)
                        {
                            float x = center.X + (radius - roundedRadius) * (float)Math.Cos(endAngleInner * ChartUtil.FDegToRad);
                            float y = center.Y + (radius - roundedRadius) * (float)Math.Sin(endAngleInner * ChartUtil.FDegToRad);
                            roundedCircleBox = new SKRect(x - roundedRadius, y - roundedRadius, x + roundedRadius, y + roundedRadius);
                            PathBuffer.ArcTo(roundedCircleBox, endAngleInner, 180, false);
                        }
                        else
                            PathBuffer.LineTo(
                                    center.X + innerRadius * (float)Math.Cos(endAngleInner * ChartUtil.FDegToRad),
                                    center.Y + innerRadius * (float)Math.Sin(endAngleInner * ChartUtil.FDegToRad));

                        PathBuffer.ArcTo(
                                innerRect,
                                endAngleInner,
                                -sweepAngleInner,
                                false);
                    }
                }
                else
                {

                    if (sweepAngleOuter % 360f > float.Epsilon)
                    {
                        if (accountForSliceSpacing)
                        {

                            float angleMiddle = startAngleOuter + sweepAngleOuter / 2.0f;

                            float sliceSpaceOffset =
                                    CalculateMinimumRadiusForSpacedSlice(
                                            center,
                                            radius,
                                            sliceAngle * phaseY,
                                            arcStartPointX,
                                            arcStartPointY,
                                            startAngleOuter,
                                            sweepAngleOuter);

                            float arcEndPointX = center.X +
                                    sliceSpaceOffset * (float)Math.Cos(angleMiddle * ChartUtil.FDegToRad);
                            float arcEndPointY = center.Y +
                                    sliceSpaceOffset * (float)Math.Sin(angleMiddle * ChartUtil.FDegToRad);

                            PathBuffer.LineTo(
                                    arcEndPointX,
                                    arcEndPointY);

                        }
                        else
                        {
                            PathBuffer.LineTo(
                                    center.X,
                                    center.Y);
                        }
                    }

                }

                PathBuffer.Close();

                BitmapCanvas.DrawPath(PathBuffer, RenderPaint);

                angle += sliceAngle * phaseX;
            }

        }

        /// <summary>
        ///  draws the description text in the center of the pie chart makes most
        /// sense when center-hole is enabled
        /// </summary>
        /// <param name="c"></param>
        partial void DrawCenterText(SKCanvas c);

        public override void DrawExtras(SKCanvas c)
        {
            DrawHole(c);
            if (DrawBitmap.TryGetTarget(out SKBitmap bitmap))
                c.DrawBitmap(bitmap, 0, 0, null);
            DrawCenterText(c);
        }

        public override void DrawHighlighted(SKCanvas c, IList<Highlight.Highlight> indices)
        {
            /* Skip entirely if using rounded circle slices, because it doesn't make sense to highlight
         * in this way.
         * TODO: add support for changing slice color with highlighting rather than only shifting the slice
         */

            bool drawInnerArc = Chart.DrawHoleEnabled && !Chart.DrawSlicesUnderHoleEnabled;
            if (drawInnerArc && Chart.DrawRoundedSlicesEnabled)
                return;

            float phaseX = Animator.PhaseX;
            float phaseY = Animator.PhaseY;

            float angle;
            float rotationAngle = Chart.RotationAngle;

            float[] drawAngles = Chart.DrawAngles;
            float[] absoluteAngles = Chart.AbsoluteAngles;
            var center = Chart.CenterCircleBox;
            float radius = Chart.Radius;
            float userInnerRadius = drawInnerArc
                    ? radius * (Chart.HoleRadius / 100.0f)
                    : 0.0f;

            for (int i = 0; i < indices.Count; i++)
            {

                // get the index to highlight
                int index = (int)indices[i].X;

                if (index >= drawAngles.Length)
                    continue;

                var set = Chart.Data[indices[i].DataSetIndex];

                if (!(set is IDataSet<Data.PieEntry> dataSet) || !set.IsHighlightEnabled)
                    continue;

                int entryCount = set.EntryCount;
                int visibleAngleCount = 0;
                for (int j = 0; j < entryCount; j++)
                {
                    // draw only if the value is greater than zero
                    if (Math.Abs(dataSet[j].Y) > float.Epsilon)
                    {
                        visibleAngleCount++;
                    }
                }

                if (index == 0)
                    angle = 0.0f;
                else
                    angle = absoluteAngles[index - 1] * phaseX;

                float sliceSpace = visibleAngleCount <= 1 ? 0.0f : set.SliceSpace;

                float sliceAngle = drawAngles[index];
                float innerRadius = userInnerRadius;

                float shift = set.SelectionShift;
                float highlightedRadius = radius + shift;
                var highlightedCircleBox = Chart.CircleBox.Inset(shift, shift);

                var accountForSliceSpacing = sliceSpace > 0.0f && sliceAngle <= 180.0f;

                var highlightColor = set.HighlightColor;
                if (highlightColor == null)
                {
                    RenderPaint.Color = set.ColorAt(index);
                }
                else
                {
                    RenderPaint.Color = highlightColor.Value;
                }

                float sliceSpaceAngleOuter = visibleAngleCount == 1 ?
                        0.0f :
                        sliceSpace / (ChartUtil.FDegToRad * radius);

                float sliceSpaceAngleShifted = visibleAngleCount == 1 ?
                        0.0f :
                        sliceSpace / (ChartUtil.FDegToRad * highlightedRadius);

                float startAngleOuter = rotationAngle + (angle + sliceSpaceAngleOuter / 2.0f) * phaseY;
                float sweepAngleOuter = (sliceAngle - sliceSpaceAngleOuter) * phaseY;
                if (sweepAngleOuter < 0.0f)
                {
                    sweepAngleOuter = 0.0f;
                }

                float startAngleShifted = rotationAngle + (angle + sliceSpaceAngleShifted / 2.0f) * phaseY;
                float sweepAngleShifted = (sliceAngle - sliceSpaceAngleShifted) * phaseY;
                if (sweepAngleShifted < 0.0f)
                {
                    sweepAngleShifted = 0.0f;
                }

                PathBuffer.Reset();

                if (sweepAngleOuter >= 360.0f && sweepAngleOuter % 360f <= float.Epsilon)
                {
                    // Android is doing "mod 360"
                    PathBuffer.AddCircle(center.X, center.Y, highlightedRadius, SKPathDirection.Clockwise);
                }
                else
                {

                    PathBuffer.MoveTo(
                            center.X + highlightedRadius * (float)Math.Cos(startAngleShifted * ChartUtil.FDegToRad),
                            center.Y + highlightedRadius * (float)Math.Sin(startAngleShifted * ChartUtil.FDegToRad));

                    PathBuffer.ArcTo(
                            highlightedCircleBox,
                            startAngleShifted,
                            sweepAngleShifted,
                            false
                    );
                }

                float sliceSpaceRadius = 0.0f;
                if (accountForSliceSpacing)
                {
                    sliceSpaceRadius =
                            CalculateMinimumRadiusForSpacedSlice(
                                    center, radius,
                                    sliceAngle * phaseY,
                                    center.X + radius * (float)Math.Cos(startAngleOuter * ChartUtil.FDegToRad),
                                    center.Y + radius * (float)Math.Sin(startAngleOuter * ChartUtil.FDegToRad),
                                    startAngleOuter,
                                    sweepAngleOuter);
                }

                // API < 21 does not receive floats in addArc, but a RectF
                var innerRect = new SKRect(
                        center.X - innerRadius,
                        center.Y - innerRadius,
                        center.X + innerRadius,
                        center.Y + innerRadius);

                if (drawInnerArc &&
                        (innerRadius > 0.0f || accountForSliceSpacing))
                {

                    if (accountForSliceSpacing)
                    {
                        float minSpacedRadius = sliceSpaceRadius;

                        if (minSpacedRadius < 0.0f)
                            minSpacedRadius = -minSpacedRadius;

                        innerRadius = Math.Max(innerRadius, minSpacedRadius);
                    }

                    float sliceSpaceAngleInner = visibleAngleCount == 1 || innerRadius == 0.0f ?
                            0.0f :
                            sliceSpace / (ChartUtil.FDegToRad * innerRadius);
                    float startAngleInner = rotationAngle + (angle + sliceSpaceAngleInner / 2.0f) * phaseY;
                    float sweepAngleInner = (sliceAngle - sliceSpaceAngleInner) * phaseY;
                    if (sweepAngleInner < 0.0f)
                    {
                        sweepAngleInner = 0.0f;
                    }
                    float endAngleInner = startAngleInner + sweepAngleInner;

                    if (sweepAngleOuter >= 360.0f && sweepAngleOuter % 360f <= float.Epsilon)
                    {
                        // Android is doing "mod 360"
                        PathBuffer.AddCircle(center.X, center.Y, innerRadius, SKPathDirection.CounterClockwise);
                    }
                    else
                    {

                        PathBuffer.LineTo(
                                center.X + innerRadius * (float)Math.Cos(endAngleInner * ChartUtil.FDegToRad),
                                center.Y + innerRadius * (float)Math.Sin(endAngleInner * ChartUtil.FDegToRad));

                        PathBuffer.ArcTo(
                                innerRect,
                                endAngleInner,
                                -sweepAngleInner,
                                false
                        );
                    }
                }
                else
                {

                    if (sweepAngleOuter % 360f > float.Epsilon)
                    {

                        if (accountForSliceSpacing)
                        {
                            float angleMiddle = startAngleOuter + sweepAngleOuter / 2.0f;

                            float arcEndPointX = center.X +
                                    sliceSpaceRadius * (float)Math.Cos(angleMiddle * ChartUtil.FDegToRad);
                            float arcEndPointY = center.Y +
                                    sliceSpaceRadius * (float)Math.Sin(angleMiddle * ChartUtil.FDegToRad);

                            PathBuffer.LineTo(
                                    arcEndPointX,
                                    arcEndPointY);

                        }
                        else
                        {

                            PathBuffer.LineTo(
                                    center.X,
                                    center.Y);
                        }

                    }

                }

                PathBuffer.Close();

                BitmapCanvas.DrawPath(PathBuffer, RenderPaint);
            }

        }

        public override void DrawValues(SKCanvas c)
        {
            var center = Chart.CenterCircleBox;

            // get whole the radius
            float radius = Chart.Radius;
            float rotationAngle = Chart.RotationAngle;
            float[] drawAngles = Chart.DrawAngles;
            float[] absoluteAngles = Chart.AbsoluteAngles;

            float phaseX = Animator.PhaseX;
            float phaseY = Animator.PhaseY;

            float roundedRadius = (radius - (radius * Chart.HoleRadius / 100f)) / 2f;
            float holeRadiusPercent = Chart.HoleRadius / 100.0f;
            float labelRadiusOffset = radius / 10f * 3.6f;

            if (Chart.DrawHoleEnabled)
            {
                labelRadiusOffset = (radius - (radius * holeRadiusPercent)) / 2f;

                if (!Chart.DrawSlicesUnderHoleEnabled && Chart.DrawRoundedSlicesEnabled)
                {
                    // Add curved circle slice and spacing to rotation angle, so that it sits nicely inside
                    rotationAngle += roundedRadius * 360 / (MathF.PI * 2 * radius);
                }
            }

            float labelRadius = radius - labelRadiusOffset;

            var data = Chart.Data;
            IList<IPieDataSet> dataSets = data.DataSets;

            float yValueSum = data.YValueSum;

            bool drawEntryLabels = Chart.DrawEntryLabels;

            float angle;
            int xIndex = 0;

            c.Save();

            float offset = 5.0f.DpToPixel();

            for (int i = 0; i < dataSets.Count; i++)
            {

                IPieDataSet dataSet = dataSets[i];

                bool drawValues = dataSet.IsDrawValuesEnabled;

                if (!drawValues && !drawEntryLabels)
                    continue;

                var xValuePosition = dataSet.XValuePosition;
                var yValuePosition = dataSet.YValuePosition;

                // apply the text-styling defined by the DataSet
                ApplyValueTextStyle(dataSet);

                float lineHeight = ValuePaint.MeasureText("Q")
                        + 4f.DpToPixel();

                var formatter = dataSet.ValueFormatter;

                int entryCount = dataSet.EntryCount;

                bool isUseValueColorForLineEnabled = dataSet.IsUseValueColorForLineEnabled;
                var valueLineColor = dataSet.ValueLineColor;

                ValueLinePaint.StrokeWidth = (dataSet.ValueLineWidth.DpToPixel());

                float sliceSpace = GetSliceSpace(dataSet);

                var iconsOffset = dataSet.IconsOffset;
                iconsOffset.X = iconsOffset.X.DpToPixel();
                iconsOffset.Y = iconsOffset.Y.DpToPixel();

                for (int j = 0; j < entryCount; j++)
                {

                    var entry = ((IDataSet<Data.PieEntry>)dataSet)[j];

                    if (xIndex == 0)
                        angle = 0.0f;
                    else
                        angle = absoluteAngles[xIndex - 1] * phaseX;

                    float sliceAngle = drawAngles[xIndex];
                    float sliceSpaceMiddleAngle = sliceSpace / (ChartUtil.FDegToRad * labelRadius);

                    // offset needed to center the drawn text in the slice
                    float angleOffset = (sliceAngle - sliceSpaceMiddleAngle / 2.0f) / 2.0f;

                    angle += angleOffset;

                    float transformedAngle = rotationAngle + angle * phaseY;

                    float value = Chart.UsePercenValuesEnabled ? entry.Y
                            / yValueSum * 100f : entry.Y;
                    String entryLabel = entry.Label;

                    float sliceXBase = (float)Math.Cos(transformedAngle * ChartUtil.FDegToRad);
                    float sliceYBase = (float)Math.Sin(transformedAngle * ChartUtil.FDegToRad);

                    bool drawXOutside = drawEntryLabels &&
                            xValuePosition == Data.PieDataSet.ValuePosition.OutsideSlice;
                    bool drawYOutside = drawValues &&
                            yValuePosition == Data.PieDataSet.ValuePosition.OutsideSlice;
                    bool drawXInside = drawEntryLabels &&
                            xValuePosition == Data.PieDataSet.ValuePosition.InsideSlice;
                    bool drawYInside = drawValues &&
                            yValuePosition == Data.PieDataSet.ValuePosition.InsideSlice;

                    if (drawXOutside || drawYOutside)
                    {

                        float valueLineLength1 = dataSet.ValueLinePart1Length;
                        float valueLineLength2 = dataSet.ValueLinePart2Length;
                        float valueLinePart1OffsetPercentage = dataSet.ValueLinePart1OffsetPercentage / 100.0f;

                        float pt2x, pt2y;
                        float labelPtx, labelPty;

                        float line1Radius;

                        if (Chart.DrawHoleEnabled)
                            line1Radius = (radius - (radius * holeRadiusPercent))
                                    * valueLinePart1OffsetPercentage
                                    + (radius * holeRadiusPercent);
                        else
                            line1Radius = radius * valueLinePart1OffsetPercentage;

                        float polyline2Width = dataSet.IsValueLineVariableLength
                                ? labelRadius * valueLineLength2 * (float)Math.Abs(Math.Sin(
                                transformedAngle * ChartUtil.FDegToRad))
                                : labelRadius * valueLineLength2;

                        float pt0x = line1Radius * sliceXBase + center.X;
                        float pt0y = line1Radius * sliceYBase + center.Y;

                        float pt1x = labelRadius * (1 + valueLineLength1) * sliceXBase + center.X;
                        float pt1y = labelRadius * (1 + valueLineLength1) * sliceYBase + center.Y;

                        if (transformedAngle % 360.0 >= 90.0 && transformedAngle % 360.0 <= 270.0)
                        {
                            pt2x = pt1x - polyline2Width;
                            pt2y = pt1y;

                            ValuePaint.TextAlign = SKTextAlign.Right;

                            if (drawXOutside)
                                entryLabelsPaint.TextAlign = SKTextAlign.Right;

                            labelPtx = pt2x - offset;
                            labelPty = pt2y;
                        }
                        else
                        {
                            pt2x = pt1x + polyline2Width;
                            pt2y = pt1y;
                            ValuePaint.TextAlign = SKTextAlign.Left;

                            if (drawXOutside)
                                entryLabelsPaint.TextAlign = SKTextAlign.Left;

                            labelPtx = pt2x + offset;
                            labelPty = pt2y;
                        }

                        var lineColor = SKColor.Empty;

                        if (isUseValueColorForLineEnabled)
                            lineColor = dataSet.ColorAt(j);
                        else if (valueLineColor != SKColor.Empty)
                            lineColor = valueLineColor;

                        if (lineColor != SKColor.Empty)
                        {
                            ValueLinePaint.Color = (lineColor);
                            c.DrawLine(pt0x, pt0y, pt1x, pt1y, ValueLinePaint);
                            c.DrawLine(pt1x, pt1y, pt2x, pt2y, ValueLinePaint);
                        }

                        // draw everything, depending on settings
                        if (drawXOutside && drawYOutside)
                        {

                            DrawValue(c,
                                    formatter,
                                    value,
                                    entry,
                                    0,
                                    labelPtx,
                                    labelPty,
                                    dataSet.ValueTextColorAt(j));

                            if (j < data.EntryCount && entryLabel != null)
                            {
                                c.DrawText(entryLabel, labelPtx, labelPty + lineHeight, entryLabelsPaint);
                            }

                        }
                        else if (drawXOutside)
                        {
                            if (j < data.EntryCount && entryLabel != null)
                            {
                                c.DrawText(entryLabel, labelPtx, labelPty + lineHeight / 2.0f, entryLabelsPaint);
                            }
                        }
                        else if (drawYOutside)
                        {

                            DrawValue(c, formatter, value, entry, 0, labelPtx, labelPty + lineHeight / 2.0f, dataSet
                                    .ValueTextColorAt(j));
                        }
                    }

                    if (drawXInside || drawYInside)
                    {
                        // calculate the text position
                        float x = labelRadius * sliceXBase + center.X;
                        float y = labelRadius * sliceYBase + center.Y;

                        ValuePaint.TextAlign = SKTextAlign.Center;

                        // draw everything, depending on settings
                        if (drawXInside && drawYInside)
                        {

                            DrawValue(c, formatter, value, entry, 0, x, y, dataSet.ValueTextColorAt(j));

                            if (j < data.EntryCount && entryLabel != null)
                            {
                                c.DrawText(entryLabel, x, y + lineHeight, entryLabelsPaint);
                            }

                        }
                        else if (drawXInside)
                        {
                            if (j < data.EntryCount && entryLabel != null)
                            {
                                c.DrawText(entryLabel, x, y + lineHeight / 2f, entryLabelsPaint);
                            }
                        }
                        else if (drawYInside)
                        {

                            DrawValue(c, formatter, value, entry, 0, x, y + lineHeight / 2f, dataSet.ValueTextColorAt(j));
                        }
                    }

                    if (entry.Icon != null && dataSet.IsDrawIconsEnabled)
                    {
                        float x = (labelRadius + iconsOffset.Y) * sliceXBase + center.X;
                        float y = (labelRadius + iconsOffset.Y) * sliceYBase + center.Y;
                        y += iconsOffset.X;

                        entry.Icon.Draw(
                                c,
                                (int)x,
                                (int)y);
                    }

                    xIndex++;
                }

            }
            c.Restore();
        }

        private readonly SKPath HoleCirclePath = new SKPath();


        /// <summary>
        /// draws the hole in the center of the chart and the transparent circle /
        /// hole
        /// </summary>
        /// <param name="c"></param>
        protected void DrawHole(SKCanvas c)
        {
            if (Chart.DrawHoleEnabled && BitmapCanvas != null)
            {

                float radius = Chart.Radius;
                float holeRadius = radius * (Chart.HoleRadius / 100);
                var center = Chart.CenterCircleBox;

                if (holePaint.Color.Alpha > 0)
                {
                    // draw the hole-circle
                    BitmapCanvas.DrawCircle(
                            center.X, center.Y,
                            holeRadius, holePaint);
                }

                // only draw the circle if it can be seen (not covered by the hole)
                if (transparentCirclePaint.Color.Alpha > 0 &&
                        Chart.TransparentCircleRadiusPercent > Chart.HoleRadius)
                {

                    // old color
                    var color = transparentCirclePaint.Color;
                    float secondHoleRadius = radius * (Chart.TransparentCircleRadiusPercent / 100);

                    transparentCirclePaint.Color = color.WithAlpha((byte)(color.Alpha * (Animator.PhaseX * Animator.PhaseY)));

                    // draw the transparent-circle
                    HoleCirclePath.Reset();
                    HoleCirclePath.AddCircle(center.X, center.Y, secondHoleRadius, SKPathDirection.Clockwise);
                    HoleCirclePath.AddCircle(center.X, center.Y, holeRadius, SKPathDirection.CounterClockwise);
                    BitmapCanvas.DrawPath(HoleCirclePath, transparentCirclePaint);

                    // reset alpha
                    transparentCirclePaint.Color = color;
                }
            }
        }
    }
}
