using SkiaSharp;
using System.Collections.Generic;

namespace XF.ChartLibrary.Utils
{
    public partial class Transformer
    {
        public SKMatrix ValueToPixelMatrix => MatrixValueToPx
                .PostConcat(ViewPortHandler.TouchMatrix)
                .PostConcat(MatrixOffset);

        protected SKMatrix PixelToValueMatrixBuffer = new SKMatrix();

        /// <summary>
        /// Transform a rectangle with all matrices with potential animation phases.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="phaseY"></param>
        public SKRect RectToPixelPhase(SKRect r, float phaseY)
        {
            // multiply the height of the rect with the phase
            r.Top *= phaseY;
            r.Bottom *= phaseY;

            r = MatrixValueToPx.MapRect(r);
            r = ViewPortHandler.touchMatrix.MapRect(r);
            return MatrixOffset.MapRect(r);
        }

        /// <summary>
        /// Transform a rectangle with all matrices.
        /// </summary>
        /// <param name="r"></param>
        public SKRect RectValueToPixel(SKRect r)
        {
            r = MatrixValueToPx.MapRect(r);
            r = ViewPortHandler.touchMatrix.MapRect(r);
            return MatrixOffset.MapRect(r);
        }

        /// <summary>
        /// transforms multiple rects with all matrices
        /// </summary>
        /// <param name="rects"></param>
        public void RectValuesToPixel(IList<SKRect> rects)
        {
            var m = ValueToPixelMatrix;
            for (int i = 0; i < rects.Count; i++)
                rects[i] = m.MapRect(rects[i]);
        }

        /// <summary>
        /// transform a path with all the given matrices VERY IMPORTANT: keep order
        /// to value-touch-offset
        /// </summary>
        public void PathValueToPixel(SKPath path)
        {
            path.Transform(MatrixValueToPx);
            path.Transform(ViewPortHandler.TouchMatrix);
            path.Transform(MatrixOffset);
        }

        public SKPoint[] PointValuesToPixel(SKPoint[] pts)
        {
            pts = MatrixValueToPx.MapPoints(pts);
            pts = ViewPortHandler.TouchMatrix.MapPoints(pts);
            return MatrixOffset.MapPoints(pts);
        }

        public SKPoint PointValueToPixel(float x, float y)
        {
            return MatrixOffset
                .MapPoint(ViewPortHandler.TouchMatrix.MapPoint(MatrixValueToPx.MapPoint(x, y)));
        }

        public SKPoint[] PixelsToValue(params SKPoint[] points)
        {
            // invert all matrixes to convert back to the original value
            MatrixOffset.TryInvert(out SKMatrix tmp);
            points = tmp.MapPoints(points);

            ViewPortHandler.TouchMatrix.TryInvert(out tmp);
            points = tmp.MapPoints(points);

            MatrixValueToPx.TryInvert(out tmp);
            return tmp.MapPoints(points);
        }

        public SKPoint PixelsToValue(float x, float y)
        {
            // invert all matrixes to convert back to the original value
            MatrixOffset.TryInvert(out SKMatrix tmp);
            var point = tmp.MapPoint(x, y);

            ViewPortHandler.touchMatrix.TryInvert(out tmp);
            point = tmp.MapPoint(point);

            MatrixValueToPx.TryInvert(out tmp);
            return tmp.MapPoint(point);
        }

        public SKPoint ValueByTouchPoint(float x, float y)
        {
            return PixelsToValue(x, y);
        }

        protected SKPoint[] valuePointsForGenerateTransformedValuesLine = new SKPoint[1];

        /// <summary>
        /// Transforms an List of Entry into a float array containing the x and
        /// y values transformed with all matrices for the CANDLESTICKCHART.
        /// </summary>
        public SKPoint[] GenerateTransformedValuesLine(Interfaces.DataSets.ILineDataSet data,
                                                 float phaseX, float phaseY,
                                                 int min, int max)
        {
            Interfaces.DataSets.IDataSet dataSet = data;
            int count = ((int)((max - min) * phaseX) + 1);

            if (valuePointsForGenerateTransformedValuesLine.Length != count)
            {
                valuePointsForGenerateTransformedValuesLine = new SKPoint[count];
            }
            var valuePoints = valuePointsForGenerateTransformedValuesLine;

            for (int j = 0; j < count; j++)
            {

                var e = dataSet[j + min];

                if (e != null)
                {
                    valuePoints[j] = new SKPoint(e.X, e.Y * phaseY);
                }
            }

            return ValueToPixelMatrix.MapPoints(valuePoints);
        }

        /// <summary>
        ///  Prepares the matrix that transforms values to pixels. Calculates the
        ///scale factors from the charts size and offsets.
        /// </summary>
        public void PrepareMatrixValuePx(float xChartMin, float deltaX, float deltaY, float yChartMin)
        {
            float scaleX = (float)(ViewPortHandler.ContentWidth / deltaX);
            float scaleY = (float)(ViewPortHandler.ContentHeight / deltaY);

            if (float.IsInfinity(scaleX))
            {
                scaleX = 0;
            }
            if (float.IsInfinity(scaleY))
            {
                scaleY = 0;
            }

            // setup all matrices
            MatrixValueToPx = SKMatrix.Identity
                .PostConcat(SKMatrix.CreateTranslation(-xChartMin, -yChartMin))
                .PostConcat(SKMatrix.CreateScale(scaleX, -scaleY));
        }

        /// <summary>
        /// Prepares the matrix that contains all offsets.
        /// </summary>
        public void PrepareMatrixOffset(bool inverted)
        {
            if (!inverted)
                MatrixOffset = SKMatrix.Identity.PostConcat(SKMatrix.CreateTranslation(ViewPortHandler.OffsetLeft,
                        ViewPortHandler.ChartHeight - ViewPortHandler.OffsetBottom));
            else
            {
                MatrixOffset = SKMatrix
                    .CreateTranslation(ViewPortHandler.OffsetLeft, -ViewPortHandler.OffsetTop)
                    .PostConcat(SKMatrix.CreateScale(1f, -1f));
            }
        }
    }
}
