using Android.Graphics;

namespace XF.ChartLibrary.Utils
{
    public static partial class MatrixUtil
    {
        public static Matrix CreateIdentity()
        {
            return new Matrix();
        }

        public static Matrix PostTranslateScale(this Matrix self, float scaleX, float scaleY, float tx, float ty)
        {
            self.Reset();
            self.PostTranslate(tx, ty);
            self.PostScale(scaleX, -scaleY);
            return self;
        }

        public static Matrix Reset(this Matrix self)
        {
            self.Reset();
            return self;
        }

        public static Matrix PostTranslate(this Matrix self, float tx, float ty)
        {
            self.Reset();
            self.PostTranslate(tx, ty);
            return self;
        }

        public static Matrix Translate(this Matrix self, float tx, float ty)
        {
            self.Reset();
            self.SetTranslate(tx, ty);
            return self;
        }


        public static Matrix PostScale(this Matrix self, float scaleX, float scaleY)
        {
            self.Reset();
            self.PostScale(scaleX, scaleY);
            return self;
        }
    }
}
