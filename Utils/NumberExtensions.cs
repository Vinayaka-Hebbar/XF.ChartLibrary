using System;

namespace XF.ChartLibrary.Utils
{
    public static class NumberExtensions
    {
        public static float RoundToNextSignificant(float number)
        {
            if (double.IsInfinity(number) ||
                double.IsNaN(number) ||
                number == 0.0)
                return 0;

            float d = (float)MathF.Ceiling((float)Math.Log10(number < 0 ? -number : number));
            int pw = 1 - (int)d;
            float magnitude = (float)MathF.Pow(10, pw);
            long shifted = (long)MathF.Round(number * magnitude);
            return shifted / magnitude;
        }

        public static int Digits(this float self)
        {
            float i = RoundToNextSignificant(self);

            if (float.IsInfinity(i))
                return 0;

            return (int)MathF.Ceiling(-MathF.Log10(i)) + 2;
        }
    }
}
