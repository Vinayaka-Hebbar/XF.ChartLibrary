using SkiaSharp;

namespace XF.ChartLibrary.Utils
{
    public  static partial class MatrixUtil
    {
        public static SKMatrix CreateIdentity()
        {
            return SKMatrix.CreateIdentity();
        }

        public static SKMatrix PostTranslateScale(this SKMatrix _, float scaleX, float scaleY, float tx, float ty)
        {
            return SKMatrix.CreateScaleTranslation(scaleX, scaleY, tx, ty);
        }

        public static SKMatrix Reset(this SKMatrix _) => SKMatrix.CreateIdentity();

        public static SKMatrix PostTranslate(this SKMatrix self, float tx, float ty)
        {
            return self.PostConcat(SKMatrix.CreateTranslation(tx,ty));
        }

        public static SKMatrix Translate(this SKMatrix self, float tx, float ty)
        {
            return SKMatrix.CreateTranslation(tx, ty);
        }

        public static SKMatrix PostScale(this SKMatrix self, float scaleX, float scaleY)
        {
            return self.PostConcat(SKMatrix.CreateScale(scaleX, scaleY));
        }
    }
}
