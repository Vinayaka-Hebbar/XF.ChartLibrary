using SkiaSharp;

namespace XF.ChartLibrary.Data
{
    partial class BarEntry
    {
        public BarEntry(float x, float y, SKImage icon) : base(x, y)
        {
            Icon = icon;
        }

        public BarEntry(float x, float y, SKImage icon, object data) : base(x, y)
        {
            Icon = icon;
            Data = data;
        }

        /// <summary>
        ///  Constructor for stacked bar entries.One data object for whole stack
        /// </summary>
        /// <param name="x"></param>
        /// <param name="vals">the stack values, use at least 2</param>
        /// <param name="icon">icon image</param>
        public BarEntry(float x, float[] vals, SKImage icon) : base(x, CalcSum(vals), icon)
        {
            yVals = vals;
            CalcPosNegSum();
            CalcRanges();
        }


        /// <summary>
        /// Constructor for stacked bar entries.One data object for whole stack
        /// </summary>
        /// <param name="x"></param>
        /// <param name="vals">the stack values, use at least 2</param>
        /// <param name="icon">icon image</param>
        /// <param name="data">Spot for additional data this Entry represents.</param>
        public BarEntry(float x, float[] vals, SKImage icon, object data) : base(x, CalcSum(vals), icon, data)
        {
            yVals = vals;
            CalcPosNegSum();
            CalcRanges();
        }
    }
}
