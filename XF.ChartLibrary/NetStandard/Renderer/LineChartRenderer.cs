using SkiaSharp;
using System;
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
                    DrawBitmap = new System.WeakReference<SKBitmap>(drawBitmap);
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
                if (set.IsVisible)
                    DrawDataSet(c, set);
            }

            c.DrawBitmap(drawBitmap, 0, 0, RenderPaint);
        }

        protected void DrawDataSet(SKCanvas c, Data.LineDataSet dataSet)
        {
            if (dataSet.EntryCount < 1)
                return;

            RenderPaint.StrokeWidth = dataSet.LineWidth;
            RenderPaint.PathEffect = dataSet.DashPathEffect;

            switch (dataSet.Mode)
            {
                default:
                case Data.LineDataSet.LineMode.Linear:
                case Data.LineDataSet.LineMode.Stepped:
                    DrawLinear(c, dataSet);
                    break;

                case Data.LineDataSet.LineMode.CubicBezier:
                    DrawCubicBezier(dataSet);
                    break;

                case Data.LineDataSet.LineMode.HorizontalBezier:
                    DrawHorizontalBezier(dataSet);
                    break;
            }

            RenderPaint.PathEffect = null;
        }

        protected void DrawLinear(SKCanvas c, LineDataSet dataSet)
        {
            int entryCount = dataSet.EntryCount;

            bool isDrawSteppedEnabled = dataSet.Mode == LineDataSet.LineMode.Stepped;
            int pointsPerEntryPair = isDrawSteppedEnabled ? 4 : 2;

            Transformer trans = Chart.GetTransformer(dataSet.AxisDependency);

            float phaseY = Animator.PhaseY;

            RenderPaint.Style = SKPaintStyle.Stroke;

            SKCanvas canvas;

            // if the data-set is dashed, draw on bitmap-canvas
            if (dataSet.IsDashedLineEnabled)
            {
                canvas = BitmapCanvas;
            }
            else
            {
                canvas = c;
            }

            XBounds.Set(Chart, dataSet, Animator);

            // if drawing filled is enabled
            if (dataSet.DrawFilled && entryCount > 0)
            {
                DrawLinearFill(c, dataSet, trans, XBounds);
            }

            // more than 1 color
            if (dataSet.Colors.Count > 1)
            {

                int numberOfFloats = pointsPerEntryPair;

                if (_lineBuffer.Length <= numberOfFloats)
                    _lineBuffer = new SKPoint[numberOfFloats];

                int max = XBounds.Min + XBounds.Range;

                for (int j = XBounds.Min; j < max; j++)
                {

                    Entry e = dataSet[j];
                    if (e == null) continue;

                    _lineBuffer[0] = new SKPoint(e.X, e.Y * phaseY);

                    if (j < XBounds.Max)
                    {

                        e = dataSet[j + 1];

                        if (e == null) break;

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
                    RenderPaint.Color = dataSet.ColorAt(j);

                    for (int i = 0; i < numberOfFloats; i += 2)
                    {
                        canvas.DrawLine(pts[i], pts[i + 1], RenderPaint);
                    }
                }

            }
            else
            { // only one color per dataset

                if (_lineBuffer.Length < Math.Max((entryCount) * pointsPerEntryPair, pointsPerEntryPair))
                    _lineBuffer = new SKPoint[Math.Max((entryCount) * pointsPerEntryPair, pointsPerEntryPair) * 2];

                Entry e1, e2;

                e1 = dataSet[XBounds.Min];

                if (e1 != null)
                {

                    int j = 0;
                    for (int x = XBounds.Min; x <= XBounds.Range + XBounds.Min; x++)
                    {

                        e1 = dataSet[x == 0 ? 0 : (x - 1)];
                        e2 = dataSet[x];

                        if (e1 == null || e2 == null) continue;

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

                        int size = Math.Max((XBounds.Range + 1) * pointsPerEntryPair, pointsPerEntryPair) * 2;

                        RenderPaint.Color = dataSet.Color;

                        for (int i = 0; i < size; i += 2)
                        {
                            canvas.DrawLine(pts[i], pts[i + 1], RenderPaint);
                        }
                    }
                }
            }

            RenderPaint.PathEffect = null;
        }

        protected void DrawLinearFill(SKCanvas c, LineDataSet dataSet, Transformer trans, Bounds bounds)
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

                        DrawFilledPath(c, filled, fill, dataSet.FillAlpha);
                    }
                    else
                    {

                        DrawFilledPath(c, filled, dataSet.FillColor, dataSet.FillAlpha);
                    }
                }

                iterations++;

            } while (currentStartIndex <= currentEndIndex);
        }

        void GenerateFilledPath(LineDataSet dataSet, int startIndex, int endIndex, SKPath outputPath)
        {
            float fillMin = dataSet.FillFormatter.GetFillLinePosition(dataSet, Chart);
            float phaseY = Animator.PhaseY;
            bool isDrawSteppedEnabled = dataSet.Mode == LineDataSet.LineMode.Stepped;

            var filled = outputPath;
            filled.Reset();

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

        protected void DrawCubicBezier(LineDataSet dataSet)
        {
            float phaseY = Animator.PhaseY;

            Transformer trans = Chart.GetTransformer(dataSet.AxisDependency);

            XBounds.Set(Chart, dataSet, Animator);

            float intensity = dataSet.CubicIntensity;

            CubicPath.Reset();

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

                if (cur == null) return;

                // let the spline start
                CubicPath.MoveTo(cur.X, cur.Y * phaseY);

                for (int j = XBounds.Min + 1; j <= XBounds.Range + XBounds.Min; j++)
                {
                    prevPrev = prev;
                    prev = cur;
                    cur = nextIndex == j ? next : dataSet[j];

                    nextIndex = j + 1 < dataSet.EntryCount ? j + 1 : j;
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
            if (dataSet.DrawFilled)
            {

                CubicFillPath.Reset();
                CubicFillPath.AddPath(CubicPath);

                DrawCubicFill(BitmapCanvas, dataSet, CubicFillPath, trans, XBounds);
            }

            RenderPaint.Color = dataSet.Color;

            RenderPaint.Style = SKPaintStyle.Stroke;

            trans.PathValueToPixel(CubicPath);

            BitmapCanvas.DrawPath(CubicPath, RenderPaint);

            RenderPaint.PathEffect = null;
        }

        protected void DrawHorizontalBezier(Data.LineDataSet dataSet)
        {
            float phaseY = Animator.PhaseY;

            var trans = Chart.GetTransformer(dataSet.AxisDependency);

            XBounds.Set(Chart, dataSet, Animator);

            CubicPath.Reset();

            if (XBounds.Range >= 1)
            {

                var prev = dataSet[XBounds.Min];
                var cur = prev;

                // let the spline start
                CubicPath.MoveTo(cur.X, cur.Y * phaseY);

                for (int j = XBounds.Min + 1; j <= XBounds.Range + XBounds.Min; j++)
                {

                    prev = cur;
                    cur = dataSet[j];

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

                DrawFilledPath(c, spline, fill, dataSet.FillAlpha);
            }
            else
            {

                DrawFilledPath(c, spline, dataSet.FillColor, dataSet.FillAlpha);
            }
        }
    }
}
