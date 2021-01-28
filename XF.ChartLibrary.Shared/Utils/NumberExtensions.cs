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

            float d = (float)MathF.Ceiling((float)Math.Log10(number < 0 ? -number : number));
            int pw = 1 - (int)d;
            float magnitude = (float)MathF.Pow(10, pw);
            long shifted = (long)Math.Round(number * magnitude);
            return shifted / magnitude;
        }

        public static double NextDouble(this double value)
        {

            // Get the long representation of value:
            var longRep = BitConverter.DoubleToInt64Bits(value);

            long nextLong;
            if (longRep >= 0) // number is positive, so increment to go "up"
                nextLong = longRep + 1L;
            else if (longRep == long.MinValue) // number is -0
                nextLong = 1L;
            else  // number is negative, so decrement to go "up"
                nextLong = longRep - 1L;

            return BitConverter.Int64BitsToDouble(nextLong);
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
