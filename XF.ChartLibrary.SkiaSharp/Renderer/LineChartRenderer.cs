using SkiaSharp;
using System;
using System.Collections.Generic;
using XF.ChartLibrary.Data;
using XF.ChartLibrary.Interfaces.DataSets;
using XF.ChartLibrary.Utils;

namespace XF.ChartLibrary.Renderer
{
    public partial class LineChartRenderer
    {
        /// <summary>
        /// Bitmap object used for drawing the paths (otherwise they are too long if
        /// rendered directly on the canvas)
        /// </summary>
        protected SKPaint CirclePaintInner;

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

        protected readonly SKPath CubicPath = new SKPath();
        protected readonly SKPath CubicFillPath = new SKPath();
        protected SKPath GenerateFilledPathBuffer = new SKPath();

        private SKPoint[] _lineBuffer = new SKPoint[4];

        protected override void Initialize()
        {
            base.Initialize();
            CirclePaintInner = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                Color = SKColors.White
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

            var lineData = ((Interfaces.DataProvider.ILineChartProvider)Chart).Data;

            foreach (var set in lineData.DataSets)
            {
                if (set.IsVisible)
                    DrawDataSet(c, set);
            }

            c.DrawBitmap(drawBitmap, 0, 0, RenderPaint);
        }

        protected void DrawDataSet(SKCanvas c, ILineDataSet dataSet)
        {
            if (dataSet.EntryCount < 1)
                return;

            RenderPaint.StrokeWidth = dataSet.LineWidth;
            RenderPaint.PathEffect = dataSet.DashPathEffect;

            switch (dataSet.Mode)
            {
                default:
                case LineDataSet.LineMode.Linear:
                case LineDataSet.LineMode.Stepped:
                    DrawLinear(c, dataSet);
                    break;

                case LineDataSet.LineMode.CubicBezier:
                    DrawCubicBezier(dataSet);
                    break;

                case LineDataSet.LineMode.HorizontalBezier:
                    DrawHorizontalBezier(dataSet);
                    break;
            }

            RenderPaint.PathEffect = null;
        }

        protected void DrawLinear(SKCanvas c, ILineDataSet lineDataSet)
        {
            int entryCount = lineDataSet.EntryCount;

            bool isDrawSteppedEnabled = lineDataSet.Mode == LineDataSet.LineMode.Stepped;
            int pointsPerEntryPair = isDrawSteppedEnabled ? 4 : 2;

            Transformer trans = Chart.GetTransformer(lineDataSet.AxisDependency);

            float phaseY = Animator.PhaseY;

            RenderPaint.Style = SKPaintStyle.Stroke;

            SKCanvas canvas;

            // if the data-set is dashed, draw on bitmap-canvas
            if (lineDataSet.IsDashedLineEnabled)
            {
                canvas = BitmapCanvas;
            }
            else
            {
                canvas = c;
            }

            XBounds.Set(Chart, lineDataSet, Animator);

            // if drawing filled is enabled
            if (lineDataSet.DrawFilled && entryCount > 0)
            {
                DrawLinearFill(c, lineDataSet, trans, XBounds);
            }

            // more than 1 color
            if (lineDataSet.Colors.Count > 1)
            {

                int numberOfFloats = pointsPerEntryPair;

                if (_lineBuffer.Length <= numberOfFloats)
                    _lineBuffer = new SKPoint[numberOfFloats];

                int max = XBounds.Min + XBounds.Range;
                IDataSet<Entry> dataSet = lineDataSet;
                for (int j = XBounds.Min; j < max; j++)
                {

                    Entry e = dataSet[j];
                    if (e == null)
                        continue;

                    _lineBuffer[0] = new SKPoint(e.X, e.Y * phaseY);

                    if (j < XBounds.Max)
                    {

                        e = dataSet[j + 1];

                        if (e == null)
                            break;

                        if (isDrawSteppedEnabled)
                        {
                            _lineBuffer[1] = new SKPoint(e.X, _lineBuffer[0].Y);
                            _lineBuffer[2] = _lineBuffer[1];
                            _lineBuffer[3] = new SKPoint(e.X, e.Y * phaseY);
                        }
                        else
                        {
                            _lineBuffer[1] = new SKPoint(e.X, e.Y * phaseY);
                        }

                    }
                    else
                    {
                        _lineBuffer[1] = _lineBuffer[0];
                    }

                    var pts = trans.PointValuesToPixel(_lineBuffer);
                    // Determine the start and end coordinates of the line, and make sure they differ.
                    var firstCoordinate = pts[0];
                    var lastCoordinate = pts[numberOfFloats - 1];
                    if (firstCoordinate == lastCoordinate)
                        continue;


                    if (!ViewPortHandler.IsInBoundsRight(firstCoordinate.X))
                        break;

                    // make sure the lines don't do shitty things outside
                    // bounds
                    if (!ViewPortHandler.IsInBoundsLeft(lastCoordinate.X) ||
                            !ViewPortHandler.IsInBoundsTop(Math.Max(firstCoordinate.Y, lastCoordinate.Y)) ||
                            !ViewPortHandler.IsInBoundsBottom(Math.Min(firstCoordinate.Y, lastCoordinate.Y)))
                        continue;

                    // get the color that is set for this line-segment
                    RenderPaint.Color = lineDataSet.ColorAt(j);

                    for (int i = 0; i < numberOfFloats; i += 2)
                    {
                        canvas.DrawLine(pts[i], pts[i + 1], RenderPaint);
                    }
                }

            }
            else
            { 
                // only one color per dataset
                if (_lineBuffer.Length < Math.Max(entryCount * pointsPerEntryPair, pointsPerEntryPair))
                    _lineBuffer = new SKPoint[Math.Max(entryCount * pointsPerEntryPair, pointsPerEntryPair) * 2];

                Entry e1, e2;
                IDataSet<Entry> dataSet = lineDataSet;
                e1 = dataSet[XBounds.Min];

                if (e1 != null)
                {

                    int j = 0;
                    for (int x = XBounds.Min; x <= XBounds.Range + XBounds.Min; x++)
                    {

                        e1 = dataSet[x == 0 ? 0 : (x - 1)];
                        e2 = dataSet[x];

                        if (e1 == null || e2 == null)
                            continue;

                        _lineBuffer[j++] = new SKPoint(e1.X, e1.Y * phaseY);

                        if (isDrawSteppedEnabled)
                        {
                            _lineBuffer[j++] = new SKPoint(e2.X, e1.Y * phaseY);
                            _lineBuffer[j++] = new SKPoint(e2.X, e1.Y * phaseY);
                        }

                        _lineBuffer[j++] = new SKPoint(e2.X, e2.Y * phaseY);
                    }

                    if (j > 0)
                    {
                        var pts = trans.PointValuesToPixel(_lineBuffer);

                        int size = Math.Max((XBounds.Range + 1) * pointsPerEntryPair, pointsPerEntryPair);

                        RenderPaint.Color = lineDataSet.Color;

                        for (int i = 0; i < size; i += 2)
                        {
                            canvas.DrawLine(pts[i], pts[i + 1], RenderPaint);
                        }
                    }
                }
            }

            RenderPaint.PathEffect = null;
        }

        protected void DrawLinearFill(SKCanvas c, ILineDataSet dataSet, Transformer trans, Bounds bounds)
        {
            SKPath filled = GenerateFilledPathBuffer;

            int startingIndex = bounds.Min;
            int endingIndex = bounds.Range + bounds.Min;
            int indexInterval = 128;
            int iterations = 0;


            int currentStartIndex;
            int currentEndIndex;
            // Doing this iteratively in order to avoid OutOfMemory errors that can happen on large bounds sets.
            do
            {
                currentStartIndex = startingIndex + (iterations * indexInterval);
                currentEndIndex = currentStartIndex + indexInterval;
                currentEndIndex = currentEndIndex > endingIndex ? endingIndex : currentEndIndex;

                if (currentStartIndex <= currentEndIndex)
                {
                    GenerateFilledPath(dataSet, currentStartIndex, currentEndIndex, filled);

                    trans.PathValueToPixel(filled);

                    var fill = dataSet.Fill;
                    if (fill != null)
                    {
                        fill.Draw(c, filled, RenderPaint, ViewPortHandler.ContentRect);
                    }
                    else
                    {
                        DrawFilledPath(c, filled, dataSet.FillColor, dataSet.FillAlpha);
                    }
                }

                iterations++;

            } while (currentStartIndex <= currentEndIndex);
        }

        void GenerateFilledPath(ILineDataSet lineDataSet, int startIndex, int endIndex, SKPath outputPath)
        {
            float fillMin = lineDataSet.FillFormatter.GetFillLinePosition(lineDataSet, Chart);
            float phaseY = Animator.PhaseY;
            bool isDrawSteppedEnabled = lineDataSet.Mode == LineDataSet.LineMode.Stepped;

            var filled = outputPath;
            filled.Reset();
            IDataSet<Entry> dataSet = lineDataSet;
            var entry = dataSet[startIndex];

            filled.MoveTo(entry.X, fillMin);
            filled.LineTo(entry.X, entry.Y * phaseY);

            // create a new path
            Entry currentEntry = null;
            Entry previousEntry = entry;
            for (int x = startIndex + 1; x <= endIndex; x++)
            {

                currentEntry = dataSet[x];

                if (isDrawSteppedEnabled)
                {
                    filled.LineTo(currentEntry.X, previousEntry.Y * phaseY);
                }

                filled.LineTo(currentEntry.X, currentEntry.Y * phaseY);

                previousEntry = currentEntry;
            }

            // close up
            if (currentEntry != null)
            {
                filled.LineTo(currentEntry.X, fillMin);
            }

            filled.Close();
        }

        protected void DrawCubicBezier(ILineDataSet lineDataSet)
        {
            float phaseY = Animator.PhaseY;

            Transformer trans = Chart.GetTransformer(lineDataSet.AxisDependency);

            XBounds.Set(Chart, lineDataSet, Animator);

            float intensity = lineDataSet.CubicIntensity;

            CubicPath.Reset();
            IDataSet<Entry> dataSet = lineDataSet;
            if (XBounds.Range >= 1)
            {
                // Take an extra point from the left, and an extra from the right.
                // That's because we need 4 points for a cubic bezier (cubic=4), otherwise we get lines moving and doing weird stuff on the edges of the chart.
                // So in the starting `prev` and `cur`, go -2, -1
                // And in the `lastIndex`, add +1

                int firstIndex = XBounds.Min + 1;

                Entry prevPrev;
                Entry prev = dataSet[Math.Max(firstIndex - 2, 0)];
                Entry cur = dataSet[Math.Max(firstIndex - 1, 0)];
                Entry next = cur;
                int nextIndex = -1;

                if (cur == null)
                    return;

                // let the spline start
                CubicPath.MoveTo(cur.X, cur.Y * phaseY);

                for (int j = XBounds.Min + 1; j <= XBounds.Range + XBounds.Min; j++)
                {
                    prevPrev = prev;
                    prev = cur;
                    cur = nextIndex == j ? next : dataSet[j];

                    nextIndex = j + 1 < lineDataSet.EntryCount ? j + 1 : j;
                    next = dataSet[nextIndex];

                    float prevDx = (cur.X - prevPrev.X) * intensity;
                    float prevDy = (cur.Y - prevPrev.Y) * intensity;
                    float curDx = (next.X - prev.X) * intensity;
                    float curDy = (next.Y - prev.Y) * intensity;

                    CubicPath.CubicTo(prev.X + prevDx, (prev.Y + prevDy) * phaseY,
                            cur.X - curDx,
                            (cur.Y - curDy) * phaseY, cur.X, cur.Y * phaseY);
                }
            }


            // if filled is enabled, close the path
            if (lineDataSet.DrawFilled)
            {
                CubicFillPath.Reset();
                CubicFillPath.AddPath(CubicPath);

                DrawCubicFill(BitmapCanvas, lineDataSet, CubicFillPath, trans, XBounds);
            }

            RenderPaint.Color = lineDataSet.Color;

            RenderPaint.Style = SKPaintStyle.Stroke;

            trans.PathValueToPixel(CubicPath);

            BitmapCanvas.DrawPath(CubicPath, RenderPaint);

            RenderPaint.PathEffect = null;
        }

        protected void DrawHorizontalBezier(ILineDataSet dataSet)
        {
            float phaseY = Animator.PhaseY;

            var trans = Chart.GetTransformer(dataSet.AxisDependency);

            XBounds.Set(Chart, dataSet, Animator);

            CubicPath.Reset();

            if (XBounds.Range >= 1)
            {
                var prev = ((IDataSet<Entry>)dataSet)[XBounds.Min];
                var cur = prev;

                // let the spline start
                CubicPath.MoveTo(cur.X, cur.Y * phaseY);

                for (int j = XBounds.Min + 1; j <= XBounds.Range + XBounds.Min; j++)
                {

                    prev = cur;
                    cur = ((IDataSet<Entry>)dataSet)[j];

                    float cpx = (prev.X)
                            + (cur.X - prev.X) / 2.0f;

                    CubicPath.CubicTo(
                            cpx, prev.Y * phaseY,
                            cpx, cur.Y * phaseY,
                            cur.X, cur.Y * phaseY);
                }
            }

            // if filled is enabled, close the path
            if (dataSet.DrawFilled)
            {

                CubicFillPath.Reset();
                CubicFillPath.AddPath(CubicPath);
                // create a new path, this is bad for performance
                DrawCubicFill(BitmapCanvas, dataSet, CubicFillPath, trans, XBounds);
            }

            RenderPaint.Color = dataSet.Color;

            RenderPaint.Style = SKPaintStyle.Stroke;

            trans.PathValueToPixel(CubicPath);

            BitmapCanvas.DrawPath(CubicPath, RenderPaint);

            RenderPaint.PathEffect = null;
        }

        protected void DrawCubicFill(SKCanvas c, ILineDataSet dataSet, SKPath spline, Transformer trans, Bounds bounds)
        {
            float fillMin = dataSet.FillFormatter
                    .GetFillLinePosition(dataSet, Chart);

            spline.LineTo(((IDataSet)dataSet)[bounds.Min + bounds.Range].X, fillMin);
            spline.LineTo(((IDataSet)dataSet)[bounds.Min].X, fillMin);
            spline.Close();

            trans.PathValueToPixel(spline);

            var fill = dataSet.Fill;
            if (fill != null)
            {
                fill.Draw(c, spline, RenderPaint, ViewPortHandler.ContentRect);
            }
            else
            {
                DrawFilledPath(c, spline, dataSet.FillColor, dataSet.FillAlpha);
            }
        }

        public override void DrawValues(SKCanvas c)
        {
            if (IsDrawingValuesAllowed(Chart))
            {
                var dataSets = ((Interfaces.DataProvider.ILineChartProvider)Chart).Data.DataSets;

                for (int i = 0; i < dataSets.Count; i++)
                {

                    ILineDataSet dataSet = dataSets[i];

                    if (!ShouldDrawValues(dataSet) || dataSet.EntryCount < 1)
                        continue;

                    // apply the text-styling defined by the DataSet
                    ApplyValueTextStyle(dataSet);

                    Transformer trans = Chart.GetTransformer(dataSet.AxisDependency);

                    // make sure the values do not interfear with the circles
                    int valOffset = (int)(dataSet.CircleRadius * 1.75f);

                    if (!dataSet.IsDrawCirclesEnabled)
                        valOffset /= 2;

                    XBounds.Set(Chart, dataSet, Animator);

                    var positions = trans.GenerateTransformedValuesLine(dataSet, Animator.PhaseX, Animator
                            .PhaseY, XBounds.Min, XBounds.Max);

                    var iconsOffset = dataSet.IconsOffset.DpToPixel();

                    for (int j = 0; j < positions.Length; j++)
                    {
                        var pos = positions[j];

                        if (!ViewPortHandler.IsInBoundsRight(pos.X))
                            break;

                        if (!ViewPortHandler.IsInBoundsLeft(pos.X) || !ViewPortHandler.IsInBoundsY(pos.Y))
                            continue;

                        Entry entry = ((IDataSet)dataSet)[j + XBounds.Min];

                        if (dataSet.IsDrawValuesEnabled)
                        {
                            DrawValue(c, dataSet.ValueFormatter, entry.Y, entry, i, pos.X,
                                    pos.Y - valOffset, dataSet.ValueTextColorAt(j));
                        }

                        if (entry.Icon != null && dataSet.IsDrawIconsEnabled)
                        {
                            c.DrawImage(
                                    entry.Icon,
                                    (int)(pos.X + iconsOffset.X),
                                    (int)(pos.Y + iconsOffset.Y));
                        }
                    }

                }
            }
        }

        public override void DrawExtras(SKCanvas c)
        {
            DrawCircles(c);
        }

        private readonly IDictionary<IDataSet, DataSetImageCache> imageCaches = new Dictionary<IDataSet, DataSetImageCache>();

        protected void DrawCircles(SKCanvas c)
        {
            RenderPaint.Style = SKPaintStyle.Fill;

            float phaseY = Animator.PhaseY;

            var dataSets = ((Interfaces.DataProvider.ILineChartProvider)Chart).Data.DataSets;

            for (int i = 0; i < dataSets.Count; i++)
            {

                ILineDataSet dataSet = dataSets[i];

                if (!dataSet.IsVisible || !dataSet.IsDrawCirclesEnabled ||
                        dataSet.EntryCount == 0)
                    continue;

                CirclePaintInner.Color = dataSet.CircleHoleColor;

                Transformer trans = Chart.GetTransformer(dataSet.AxisDependency);

                XBounds.Set(Chart, dataSet, Animator);

                float circleRadius = dataSet.CircleRadius;
                float circleHoleRadius = dataSet.CircleHoleRadius;
                var drawCircleHole = dataSet.IsDrawCircleHoleEnabled &&
                        circleHoleRadius < circleRadius &&
                        circleHoleRadius > 0.0f;
                var drawTransparentCircleHole = drawCircleHole &&
                        dataSet.CircleHoleColor == SKColors.Empty;

                if (imageCaches.TryGetValue(dataSet, out DataSetImageCache imageCache) == false)
                {
                    imageCache = new DataSetImageCache();
                    imageCaches.Add(dataSet, imageCache);
                }
                // only fill the cache with new bitmaps if a change is required
                if (imageCache.Init(dataSet))
                {
                    Fill(imageCache, dataSet, drawCircleHole, drawTransparentCircleHole);
                }

                int boundsRangeCount = XBounds.Range + XBounds.Min;

                for (int j = XBounds.Min; j <= boundsRangeCount; j++)
                {
                    Entry e = ((IDataSet)dataSet)[j];

                    if (e == null)
                        break;

                    var pt = trans.PointValueToPixel(e.X, e.Y * phaseY);

                    if (!ViewPortHandler.IsInBoundsRight(pt.X))
                        break;

                    if (!ViewPortHandler.IsInBoundsLeft(pt.X) ||
                            !ViewPortHandler.IsInBoundsY(pt.Y))
                        continue;

                    var circleBitmap = imageCache.GetBitmap(j);

                    if (circleBitmap != null)
                    {
                        c.DrawBitmap(circleBitmap, pt.X - circleRadius, pt.Y - circleRadius, null);
                    }
                }
            }
        }

        public override void DrawHighlighted(SKCanvas c, IList<Highlight.Highlight> indices)
        {
            var lineData = ((Interfaces.DataProvider.ILineChartProvider)Chart).Data;

            foreach (var high in indices)
            {

                ILineDataSet set = lineData[high.DataSetIndex];

                if (set == null || !set.IsHighlightEnabled)
                    continue;

                Entry e = ((IDataSet)set).EntryForXValue(high.X, high.Y);

                if (!IsInBoundsX(e, set))
                    continue;

                var pix = Chart.GetTransformer(set.AxisDependency).PointValueToPixel(e.X, e.Y * Animator
                        .PhaseY);

                high.SetDraw((float)pix.X, (float)pix.Y);

                // draw the lines
                DrawHighlightLines(c, (float)pix.X, (float)pix.Y, set);
            }
        }

        /// <summary>
        /// Fills the cache with bitmaps for the given dataset.
        /// </summary>
        protected void Fill(DataSetImageCache cache, ILineDataSet set, bool drawCircleHole, bool drawTransparentCircleHole)
        {
            int colorCount = set.CircleColorCount;
            float circleRadius = set.CircleRadius;
            float circleHoleRadius = set.CircleHoleRadius;
            var circleBitmaps = cache.CircleBitmaps;
            var circlePath = cache.CirclePathBuffer;
            for (int i = 0; i < colorCount; i++)
            {
                var circleBitmap = new SKBitmap((int)(circleRadius * 2.1), (int)(circleRadius * 2.1));

                var canvas = new SKCanvas(circleBitmap);
                circleBitmaps[i] = circleBitmap;
                RenderPaint.Color = set.GetCircleColor(i);

                if (drawTransparentCircleHole)
                {
                    // Begin path for circle with hole
                    circlePath.Reset();

                    circlePath.AddCircle(
                            circleRadius,
                            circleRadius,
                            circleRadius,
                            SKPathDirection.Clockwise);

                    // Cut hole in path
                    circlePath.AddCircle(
                            circleRadius,
                            circleRadius,
                            circleHoleRadius,
                            SKPathDirection.CounterClockwise);

                    // Fill in-between
                    canvas.DrawPath(circlePath, RenderPaint);
                }
                else
                {

                    canvas.DrawCircle(
                            circleRadius,
                            circleRadius,
                            circleRadius,
                            RenderPaint);

                    if (drawCircleHole)
                    {
                        canvas.DrawCircle(
                                circleRadius,
                                circleRadius,
                                circleHoleRadius,
                                CirclePaintInner);
                    }
                }
            }
        }

        protected class DataSetImageCache
        {
            public SKPath CirclePathBuffer = new SKPath();

            public SKBitmap[] CircleBitmaps;

            /// <summary>
            /// Sets up the cache, returns true if a change of cache was required.
            /// </summary>
            public bool Init(ILineDataSet set)
            {

                int size = set.CircleColorCount;
                var changeRequired = false;

                if (CircleBitmaps == null)
                {
                    CircleBitmaps = new SKBitmap[size];
                    changeRequired = true;
                }
                else if (CircleBitmaps.Length != size)
                {
                    CircleBitmaps = new SKBitmap[size];
                    changeRequired = true;
                }

                return changeRequired;
            }

            /// <summary>
            /// Returns the cached Bitmap at the given index.
            /// </summary>
            public SKBitmap GetBitmap(int index)
            {
                return CircleBitmaps[index % CircleBitmaps.Length];
            }
        }
    }
}
