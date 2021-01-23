using System;
using XF.ChartLibrary.Components;

namespace XF.ChartLibrary.Highlight
{
    /// <summary>
    /// Contains information needed to determine the highlighted value.
    /// </summary>
    public class Highlight : IEquatable<Highlight>
    {
        /**
     * the x-value of the highlighted value
     */
        public float X { get; set; }

        /**
         * the y-value of the highlighted value
         */
        public float Y { get; }

        /**
         * the x-pixel of the highlight
         */
        public float XPx { get; }

        /**
         * the y-pixel of the highlight
         */
        public float YPx { get; }

        /**
         * the index of the data object - in case it refers to more than one
         */
        private int DataIndex { get; }

        /**
         * the index of the dataset the highlighted value is in
         */
        public int DataSetIndex { get; }

        /**
         * index which value of a stacked bar entry is highlighted, default -1
         */
        public int StackIndex { get; }

        /**
         * the axis the highlighted value belongs to
         */
        public YAxisDependency Axis { get; }

        /**
         * the x-position (pixels) on which this highlight object was last drawn
         */
        public float DrawX { get; set; }

        /**
         * the y-position (pixels) on which this highlight object was last drawn
         */
        public float DrawY { get; set; }

        public Highlight(float x, float y, int dataSetIndex, int dataIndex)
        {
            X = x;
            Y = y;
            DataSetIndex = dataSetIndex;
            DataIndex = dataIndex;
        }

        public Highlight(float x, float y, int dataSetIndex)
        {
            X = x;
            Y = y;
            DataSetIndex = dataSetIndex;
            DataIndex = -1;
        }

        public Highlight(float x, int dataSetIndex, int stackIndex) : this(x, float.NaN, dataSetIndex)
        {
            StackIndex = stackIndex;
        }

        /**
         * constructor
         *
         * @param x            the x-value of the highlighted value
         * @param y            the y-value of the highlighted value
         * @param dataSetIndex the index of the DataSet the highlighted value belongs to
         */
        public Highlight(float x, float y, float xPx, float yPx, int dataSetIndex, YAxisDependency axis)
        {
            X = x;
            Y = y;
            XPx = xPx;
            YPx = yPx;
            DataSetIndex = dataSetIndex;
            Axis = axis;
        }

        /**
         * Constructor, only used for stacked-barchart.
         *
         * @param x            the index of the highlighted value on the x-axis
         * @param y            the y-value of the highlighted value
         * @param dataSetIndex the index of the DataSet the highlighted value belongs to
         * @param stackIndex   references which value of a stacked-bar entry has been
         *                     selected
         */
        public Highlight(float x, float y, float xPx, float yPx, int dataSetIndex, int stackIndex, YAxisDependency axis) : this(x, y, xPx, yPx, dataSetIndex, axis)
        {
            StackIndex = stackIndex;
        }

        public bool IsStacked => StackIndex >= 0;

        /**
         * Sets the x- and y-position (pixels) where this highlight was last drawn.
         *
         * @param x
         * @param y
         */
        public void SetDraw(float x, float y)
        {
            DrawX = x;
            DrawY = y;
        }


        public override string ToString()
        {
            return "Highlight, x: " + X + ", y: " + Y + ", dataSetIndex: " + DataSetIndex
                    + ", stackIndex (only stacked barentry): " + StackIndex;
        }

        public bool Equals(Highlight h)
        {
            if (h == null)
                return false;
            else
            {
                if (DataSetIndex == h.DataSetIndex && X == h.X
                        && StackIndex == h.StackIndex && DataIndex == h.DataIndex)
                    return true;
                else
                    return false;
            }
        }
    }
}
