using System;
using System.Collections.Generic;
using Range = XF.ChartLibrary.Highlight.Range;

namespace XF.ChartLibrary.Data
{
    public partial class BarEntry : Entry
    {
        /// <summary>
        /// the values the stacked barchart holds
        /// </summary>
        private IList<float> yVals;

        /// <summary>
        /// the ranges for the individual stack values - automatically calculated
        /// </summary>
        private IList<Range> ranges;

        /// <summary>
        /// the sum of all negative values this entry (if stacked) contains
        /// </summary>
        private float negativeSum;

        /// <summary>
        /// the sum of all positive values this entry (if stacked) contains
        /// </summary>
        private float positiveSum;

        /// <summary>
        /// Constructor for normal bars(not stacked).
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public BarEntry(float x, float y) : base(x, y)
        {
        }

        /// <summary>
        /// Constructor for normal bars(not stacked).
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="data">Spot for additional data this Entry represents.</param>
        public BarEntry(float x, float y, object data) : base(x, y, data)
        {
        }

        /// <summary>
        /// Constructor for stacked bar entries.One data object for whole stack
        /// </summary>
        /// <param name="x"></param>
        /// <param name="vals">the stack values, use at least 2</param>
        public BarEntry(float x, IList<float> vals) : base(x, CalcSum(vals))
        {
            yVals = vals;
            CalcPosNegSum();
            CalcRanges();
        }

        /// <summary>
        ///  Constructor for stacked bar entries.One data object for whole stack
        /// </summary>
        /// <param name="x"></param>
        /// <param name="vals"> the stack values, use at least 2</param>
        /// <param name="data"> Spot for additional data this Entry represents.</param>
        public BarEntry(float x, float[] vals, object data) :
            base(x, CalcSum(vals), data)
        {
            yVals = vals;
            CalcPosNegSum();
            CalcRanges();
        }

        public override Entry Clone()
        {
            return new BarEntry(X, Y, Data)
            {
                yVals = yVals
            };
        }

        /// <summary>
        /// Returns the stacked values this BarEntry represents, or null, if only a single value is represented (then, use
        /// getY()).
        /// </summary>
        public IList<float> YVals
        {
            get => yVals;
        }

        /// <summary>
        /// Set the array of values this BarEntry should represent.
        /// </summary>
        /// <param name="vals"></param>
        public void SetVals(float[] vals)
        {
            Y = CalcSum(vals);
            yVals = vals;
            CalcPosNegSum();
            CalcRanges();
        }

        /// <summary>
        /// Returns the ranges of the individual stack-entries. Will return null if this entry is not stacked.
        /// </summary>
        public IList<Range> Ranges
        {
            get => ranges;
        }

        /// <summary>
        /// Returns true if this BarEntry is stacked(has a values array), false if not.
        /// </summary>
        public bool IsStacked
        {
            get => yVals != null;
        }

        public float GetSumBelow(int stackIndex)
        {

            if (yVals == null)
                return 0;

            float remainder = 0f;
            int index = yVals.Count - 1;

            while (index > stackIndex && index >= 0)
            {
                remainder += yVals[index];
                index--;
            }

            return remainder;
        }

        /// <summary>
        /// Reuturns the sum of all positive values this entry (if stacked) contains.
        /// </summary>
        public float PositiveSum
        {
            get => positiveSum;
        }

        /// <summary>
        /// Returns the sum of all negative values this entry (if stacked) contains. (this is a positive number)
        /// </summary>
        public float NegativeSum
        {
            get => negativeSum;
        }

        void CalcPosNegSum()
        {
            if (yVals == null)
            {
                negativeSum = 0;
                positiveSum = 0;
                return;
            }

            float sumNeg = 0f;
            float sumPos = 0f;

            foreach (float f in yVals)
            {
                if (f <= 0f)
                    sumNeg += Math.Abs(f);
                else
                    sumPos += f;
            }

            negativeSum = sumNeg;
            positiveSum = sumPos;
        }

        /// <summary>
        /// Calculates the sum across all values of the given stack.
        /// </summary>
        /// <param name="vals"></param>
        /// <returns></returns>
        static float CalcSum(IList<float> vals)
        {
            if (vals == null)
                return 0f;

            float sum = 0f;

            foreach (float f in vals)
                sum += f;

            return sum;
        }

        protected void CalcRanges()
        {
            var values = yVals;

            if (values == null || values.Count == 0)
                return;

            ranges = new Range[values.Count];

            float negRemain = -this.negativeSum;
            float posRemain = 0f;

            for (int i = 0; i < ranges.Count; i++)
            {

                float value = values[i];

                if (value < 0)
                {
                    ranges[i] = new Range(negRemain, negRemain - value);
                    negRemain -= value;
                }
                else
                {
                    ranges[i] = new Range(posRemain, posRemain + value);
                    posRemain += value;
                }
            }
        }
    }
}
