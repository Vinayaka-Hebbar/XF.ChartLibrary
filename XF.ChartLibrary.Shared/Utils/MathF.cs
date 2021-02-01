#if NETFRAMEWORK
namespace System
{
    public static class MathF
    {
        public const float PI = 3.14159265f;

        public static float Sin(float value) => (float)System.Math.Sin(value);

        public static float Asin(float value) => (float)System.Math.Asin(value);

        public static float Cos(float value) => (float)System.Math.Cos(value);

        public static float Ceiling(float value) => (float)System.Math.Ceiling(value);

        public static float Floor(float value) => (float)System.Math.Floor(value);

        public static float Pow(float x, float y) => (float)System.Math.Pow(x,y);

        public static float Sqrt(float value) => (float)System.Math.Sqrt(value);

        public static float Log10(float value) => (float)System.Math.Log10(value);

        public static float Round(float value) => (float)System.Math.Round(value);
    }
} 
#endif
