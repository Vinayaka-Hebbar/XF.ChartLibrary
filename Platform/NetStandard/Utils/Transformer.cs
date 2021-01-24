using SkiaSharp;

namespace XF.ChartLibrary.Utils
{
    public partial class Transformer
    {
        protected SKMatrix PixelToValueMatrixBuffer = new SKMatrix();

        /// <summary>
        /// transform a path with all the given matrices VERY IMPORTANT: keep order
        /// to value-touch-offset
        /// </summary>
        public void PathValueToPixel(SKPath path)
        {
            path.Transform(MatrixValueToPx);
            path.Transform(ViewPortHandler.MatrixTouch);
            path.Transform(MatrixOffset);
        }

        public SKPoint[] PointValuesToPixel(SKPoint[] pts)
        {
            pts = MatrixValueToPx.MapPoints(pts);
            pts = ViewPortHandler.MatrixTouch.MapPoints(pts);
            return MatrixOffset.MapPoints(pts);
        }

        public SKPoint PointValueToPixel(float x, float y)
        {
            return MatrixOffset
                .MapPoint(ViewPortHandler.MatrixTouch.MapPoint(MatrixValueToPx.MapPoint(x, y)));
        }

        public SKPoint[] PixelsToValue(params SKPoint[] points)
        {
            var tmp = PixelToValueMatrixBuffer;
            tmp.Reset();

            // invert all matrixes to convert back to the original value
            MatrixOffset.TryInvert(out tmp);
            points = tmp.MapPoints(points);

            ViewPortHandler.MatrixTouch.TryInvert(out tmp);
            points = tmp.MapPoints(points);

            MatrixValueToPx.TryInvert(out tmp);
            return tmp.MapPoints(points);
        }

        public SKPoint PixelsToValue(float x, float y)
        {
            // invert all matrixes to convert back to the original value
            MatrixOffset.TryInvert(out SKMatrix tmp);
            var point = tmp.MapPoint(x, y);

            ViewPortHandler.MatrixTouch.TryInvert(out tmp);
            point = tmp.MapPoint(point);

            MatrixValueToPx.TryInvert(out tmp);
            return tmp.MapPoint(point);
        }

        public SKPoint ValueByTouchPoint(float x, float y)
        {
            return PixelsToValue(x, y);
        }
    }
}
