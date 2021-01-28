using CoreGraphics;

namespace XF.ChartLibrary.Utils
{
    public partial class MatrixUtil
    {
        public static CGAffineTransform CreateIdentity()
        {
            return CGAffineTransform.MakeIdentity();
        }

        public static CGAffineTransform PostTranslateScale(this CGAffineTransform _, float scaleX, float scaleY, float tx, float ty)
        {
            var matrix = CGAffineTransform.MakeIdentity();
            matrix.Scale(scaleX, scaleY);
            matrix.Translate(tx, ty);
            return matrix;
        }

        public static CGAffineTransform Reset(this CGAffineTransform _) => CGAffineTransform.MakeIdentity();

        public static CGAffineTransform PostTranslate(this CGAffineTransform self, float tx, float ty)
        {
            self.Translate(tx, ty);
            return self;
        }

        public static CGAffineTransform Translate(this CGAffineTransform self, float tx, float ty)
        {
            self.Translate(tx, ty);
            return self;
        }

        public static CGAffineTransform PostScale(this CGAffineTransform self, float scaleX, float scaleY)
        {
            self.Scale(scaleX, scaleY);
            return self;
        }
    }
}
