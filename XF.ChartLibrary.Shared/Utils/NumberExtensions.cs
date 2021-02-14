using System;

namespace XF.ChartLibrary.Utils
{
    public static class NumberExtensions
    {
        public static float RoundToNextSignificant(this double number)
        {
            if (double.IsInfinity(number) ||
                double.IsNaN(number) ||
                number == 0.0)
                return 0;
            float magnitude = (float)Math.Pow(10, 1 - (int)Math.Ceiling((float)Math.Log10(Math.Abs(number))));
            // Problem in Math.Round(2.5) somtimes giving 2 or 3 as result using Math.Celing
            long shifted = (long)Math.Round((number * magnitude) * 1.001);
            return shifted / magnitude;
        }

        public static double NextDouble(this double d)
        {
            if (d == double.PositiveInfinity)
                return d;
            // Get the long representation of value:
            d += 0.0d;
            return BitConverter.Int64BitsToDouble(BitConverter.DoubleToInt64Bits(d) +
                    ((d >= 0.0d) ? +1L : -1L));
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
