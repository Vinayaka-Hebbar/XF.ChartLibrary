
#if NETSTANDARD || SKIASHARP
using System;
using XF.ChartLibrary.Interfaces.DataSets;
using Matrix = SkiaSharp.SKMatrix;
using Point = SkiaSharp.SKPoint;
#elif __IOS__ || __TVOS__
using Point = CoreGraphics.CGPoint;
using Matrix = CoreGraphics.CGAffineTransform;
#elif __ANDROID__
using Point = Android.Graphics.PointF;
using Matrix = Android.Graphics.Matrix;
#endif
namespace XF.ChartLibrary.Utils
{
    /**
 * Transformer class that contains all matrices and is responsible for
 * transforming values into pixels on the screen and backwards.
 *
 * @author Philipp Jahoda
 */
    public partial class Transformer
    {

        /**
         * matrix to map the values to the screen pixels
         */
        protected Matrix MatrixValueToPx = MatrixUtil.CreateIdentity();

        /**
         * matrix for handling the different offsets of the chart
         */
        protected Matrix MatrixOffset = MatrixUtil.CreateIdentity();

        protected ViewPortHandler ViewPortHandler;

        public Transformer(ViewPortHandler viewPortHandler)
        {
            this.ViewPortHandler = viewPortHandler;
        }
    }
}
