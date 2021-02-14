using System;
using XF.ChartLibrary.Charts;

#if NETSTANDARD || SKIASHARP
using Point = SkiaSharp.SKPoint;
using Matrix = SkiaSharp.SKMatrix;
#elif __IOS__ || __TVOS__
using Point = CoreGraphics.CGPoint;
using Matrix = CoreGraphics.CGAffineTransform;
#elif __ANDROID__
using Point = Android.Graphics.PointF;
using Matrix = Android.Graphics.Matrix;
#endif
namespace XF.ChartLibrary.Utils
{
    public partial class ViewPortHandler
    {
        private float chartWidth = 0.0f;

        private float chartHeight = 0.0f;

        private float minScaleY = 1f;

        private float maxScaleY = float.MaxValue;

        private float minScaleX = 1f;

        internal float maxScaleX = float.MaxValue;

        private float scaleX = 1f;

        private float scaleY = 1f;

        private float transX;

        private float transY;

        private float transOffsetX;

        private float transOffsetY;

        public ViewPortHandler()
        {
        }

        public void SetChartDimens(float width, float height)
        {
            float offsetLeft = this.OffsetLeft;
            float offsetTop = this.OffsetTop;
            float offsetRight = this.OffsetRight;
            float offsetBottom = this.OffsetBottom;

            chartHeight = height;
            chartWidth = width;

            RestrainViewPort(offsetLeft, offsetTop, offsetRight, offsetBottom);
        }

        public bool HasChartDimens
        {
            get
            {
                return chartHeight > 0 && chartWidth > 0;
            }
        }

        /// <summary>
        /// `true` if both drag offsets (x and y) are zero or smaller.
        /// </summary>
        public bool HasNoDragOffset
        {
            get
            {
                return transOffsetX <= 0.0 && transOffsetY <= 0.0;
            }
        }

        public float OffsetLeft
        {
            get => (float)contentRect.Left;
        }

        public float OffsetRight
        {
            get => chartWidth - (float)contentRect.Right;
        }

        public float OffsetTop
        {
            get => (float)contentRect.Top;
        }

        public float OffsetBottom
        {
            get => chartHeight - (float)contentRect.Bottom;
        }

        public float ContentTop
        {
            get => (float)contentRect.Top;
        }

        public float ContentLeft
        {
            get => (float)contentRect.Left;
        }

        public float ContentRight => (float)contentRect.Right;

        public float ContentBottom => (float)contentRect.Bottom;

        public float ContentWidth => (float)contentRect.Width;

        public float ContentHeight => (float)contentRect.Height;

        public float ChartHeight => chartHeight;

        public float ChartWidth => chartWidth;

        public float SmallestContentExtension => Math.Min((float)contentRect.Width, (float)contentRect.Height);

        public bool IsInBoundsX(float x)
        {
            return IsInBoundsLeft(x) && IsInBoundsRight(x);
        }

        public bool IsInBoundsY(float y)
        {
            return IsInBoundsTop(y) && IsInBoundsBottom(y);
        }

        public bool IsInBounds(float x, float y)
        {
            return IsInBoundsX(x) && IsInBoundsY(y);
        }

        public bool IsInBoundsLeft(float x)
        {
            return contentRect.Left <= x + 1;
        }

        public bool IsInBoundsRight(float x)
        {
            x = (float)((int)(x * 100.0f)) / 100.0f;
            return contentRect.Right >= x - 1;
        }

        public bool IsInBoundsTop(float y)
        {
            return contentRect.Top <= y;
        }

        public bool IsInBoundsBottom(float y)
        {
            y = (float)((int)(y * 100.0f)) / 100.0f;
            return contentRect.Bottom >= y;
        }


        public float ScaleX => scaleX;

        public float ScaleY => scaleY;

        public float TransX => transX;

        public float TransY => transY;

        /// <summary>
        /// if the chart is fully zoomed out, return true
        /// </summary>
        public bool IsFullyZoomedOut => IsFullyZoomedOutX && IsFullyZoomedOutY;

        /// <summary>
        /// Returns true if the chart is fully zoomed out on it's y-axis (vertical).
        /// </summary>
        public bool IsFullyZoomedOutY => !(scaleY > minScaleY || minScaleY > 1f);

        /// <summary>
        /// Returns true if the chart is fully zoomed out on it's x-axis
        /// (horizontal).
        /// </summary>
        public bool IsFullyZoomedOutX => !(scaleX > minScaleX || minScaleX > 1f);

        public bool CanZoomOutMoreX => scaleX > minScaleX;

        public bool CanZoomInMoreX => scaleX < maxScaleX;

        public bool CanZoomOutMoreY => scaleY > minScaleY;

        public bool CanZoomInMoreY => scaleY < maxScaleY;

        public Matrix ZoomIn(float x, float y)
        {
            return Zoom(1.4f, 1.4f, x, y);
        }

        /// <summary>
        /// Zooms out by 0.7f, x and y are the coordinates (in pixels) of the zoom
        /// center.
        /// </summary>
        public Matrix ZoomOut(float x, float y)
        {
            return Zoom(0.7f, 0.7f, x, y);
        }

        /// <summary>
        /// Zooms out to original size.
        /// </summary>
        public void ResetZoom()
        {
            Zoom(1.0f, 1.0f, 0.0f, 0.0f);
        }

        /// <summary>
        /// Sets the minimum scale factor 
        /// </summary>
        public void SetMinimumScaleX(float xScale)
        {
            if (xScale < 1f)
                xScale = 1f;

            minScaleX = xScale;

            touchMatrix = LimitTransAndScale(touchMatrix, contentRect);
        }

        /// <summary>
        /// Sets the maximum scale factor for the x-axis
        /// </summary>
        public void SetMaximumScaleX(float xScale)
        {

            if (xScale == 0.0f)
                xScale = float.MaxValue;

            maxScaleX = xScale;

            touchMatrix = LimitTransAndScale(touchMatrix, contentRect);
        }

        /// <summary>
        /// Sets the minimum and maximum scale factors for the x-axis
        /// </summary>
        public void SetMinMaxScaleX(float minScaleX, float maxScaleX)
        {

            if (minScaleX < 1f)
                minScaleX = 1f;

            if (maxScaleX == 0.0f)
                maxScaleX = float.MaxValue;

            this.minScaleX = minScaleX;
            this.maxScaleX = maxScaleX;

            touchMatrix = LimitTransAndScale(touchMatrix, contentRect);
        }

        /// <summary>
        /// Sets the minimum scale factor for the y-axis
        /// </summary>
        public void SetMinimumScaleY(float yScale)
        {

            if (yScale < 1f)
                yScale = 1f;

            minScaleY = yScale;

            touchMatrix = LimitTransAndScale(touchMatrix, contentRect);
        }

        /// <summary>
        /// Sets the maximum scale factor for the y-axis
        /// </summary>
        public void SetMaximumScaleY(float yScale)
        {

            if (yScale == 0.0f)
                yScale = float.MaxValue;

            maxScaleY = yScale;

            touchMatrix = LimitTransAndScale(touchMatrix, contentRect);
        }

        /// <summary>
        /// Sets the minimum and maximum scale factors for the y-axis
        /// </summary>
        public void SetMinMaxScaleY(float minScaleY, float maxScaleY)
        {

            if (minScaleY < 1f)
                minScaleY = 1f;

            if (maxScaleY == 0.0f)
                maxScaleY = float.MaxValue;

            this.minScaleY = minScaleY;
            this.maxScaleY = maxScaleY;

            touchMatrix = LimitTransAndScale(touchMatrix, contentRect);
        }
    }
}
