
#if NETSTANDARD || SKIASHARP
using Point = SkiaSharp.SKPoint;
using Alignment = SkiaSharp.SKTextAlign;
#elif __IOS__ || __TVOS
using Alignment = UIKit.UITextAlignment;
using Point = CoreGraphics.CGPoint;
#elif __ANDROID__
using Point = Android.Graphics.PointF;
using Alignment = Android.Graphics.Paint.Align;
#endif

namespace XF.ChartLibrary.Components
{
    public partial class Description : ComponentBase
    {
        private Point position;

        /// <summary>
        /// the text used in the description
        /// </summary>
        public string Text { get; set; } = string.Empty;

        /// <summary>
        /// the custom position of the description text
        /// </summary>
        public Point Position => position;

        /// <summary>
        ///  the alignment of the description text
        /// </summary>
        public Alignment TextAlign { get; set; } = Alignment.Right;

        /// <summary>
        /// Sets a custom position for the description text in pixels on the screen.
        /// </summary>
        /// <param name="x">xcoordinate</param>
        /// <param name="y">ycoordinate</param>
        public void SetPosition(float x, float y)
        {
            if (position == null)
            {
                position = new Point(x, y);
            }
            else
            {
                position.X = x;
                position.Y = y;
            }
        }
    }
}
