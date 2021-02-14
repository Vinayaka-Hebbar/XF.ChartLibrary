using System;
using System.Collections.Generic;
using XF.ChartLibrary.Formatter;

namespace XF.ChartLibrary.Components
{
#if NETSTANDARD || SKIASHARP
    using DashPathEffect = SkiaSharp.SKPathEffect;
#elif __ANDROID__
    using DashPathEffect = Android.Graphics.DashPathEffect;
#endif
    public partial class AxisBase : ComponentBase
    {
        /// <summary>
        /// custom formatter that is used instead of the auto-formatter if set
        /// </summary>
        protected IAxisValueFormatter axisValueFormatter;

        private float gridLineWidth = 1f;

        private float axisLineWidth = 1f;

        private int axisMinLabels = 2;

        private int axisMaxLabels = 25;

        private bool drawGridLines = true;

        private bool drawAxisLine = true;

        private bool forceLabels = false;

        private bool granularityEnabled = false;

        internal float axisRange = 0f;

        internal float[] centeredEntries = Array.Empty<float>();

        internal IList<float> entries = Array.Empty<float>();

        /// <summary>
        /// the actual array of entries
        /// </summary>
        public IList<float> Entries => entries;

        /// <summary>
        /// axis label entries only used for centered labels
        /// </summary>
        public float[] CenteredEntries => centeredEntries;

        /// <summary>
        ///  the number of entries the legend contains
        /// </summary>
        public int EntryCount => entryCount;

        /// <summary>
        /// the number of decimal digits to use
        /// </summary>
        public int Decimals { get; set; }

        /// <summary>
        ///  the number of label entries the axis should have, default 6
        /// </summary>
        private int labelCount = 6;

        /// <summary>
        /// the minimum interval between axis values
        /// </summary>
        protected float granularity = 1.0f;


        /// <summary>
        /// flag that indicates if the line alongside the axis is drawn or not
        /// </summary>
        public bool DrawAxisLine
        {
            get => drawAxisLine;
            set => drawAxisLine = value;
        }

        /// <summary>
        /// flag that indicates of the labels of this axis should be drawn or not
        /// </summary>
        protected bool drawLabels = true;

        public bool CenterAxisLabels { get; set; } = false;

        /// <summary>
        /// array of limit lines that can be set for the axis
        /// </summary>
        public IList<LimitLine> LimitLines { get; }

        /// <summary>
        /// flag indicating the limit lines layer depth
        /// </summary>
        protected bool drawLimitLineBehindData = false;

        /// <summary>
        /// flag indicating the grid lines layer depth
        /// </summary>
        protected bool drawGridLinesBehindData = true;

        /// <summary>
        /// Extra spacing for `axisMinimum` to be added to automatically calculated `axisMinimum`
        /// </summary>
        protected float spaceMin = 0.0f;

        /// <summary>
        /// Extra spacing for `axisMaximum` to be added to automatically calculated `axisMaximum`
        /// </summary>
        protected float spaceMax = 0.0f;

        /// <summary>
        /// flag indicating that the axis-min value has been customized
        /// </summary>
        protected bool CustomAxisMin = false;

        /// <summary>
        /// flag indicating that the axis-max value has been customized
        /// </summary>
        protected bool CustomAxisMax = false;

        internal float axisMaximum = 0f;

        internal float axisMinimum = 0f;

        internal int entryCount;

        /// <summary>
        /// the total range of values this axis covers
        /// </summary>
        public float AxisRange
        {
            get => axisRange;
            set => axisRange = value;
        }

        /// <summary>
        /// The minumum number of labels on the axis
        /// </summary>
        public int AxisMinLabels
        {
            get => axisMinLabels;
            set
            {
                if (value > 0)
                    axisMinLabels = value;
            }
        }



#if __ANDROID__ || NETSTANDARD || SKIASHARP
        private DashPathEffect axisLineDashPathEffect;

        private DashPathEffect gridDashPathEffect;

        /// <summary>
        ///  Enables the grid line to be drawn in dashed mode, e.g.like this
        /// "- - - - - -". THIS ONLY WORKS IF HARDWARE-ACCELERATION IS TURNED OFF.
        /// Keep in mind that hardware acceleration boosts performance.
        /// </summary>
        public DashPathEffect GridDashedLine
        {
            get => gridDashPathEffect;
            set => gridDashPathEffect = value;
        }

        /// <summary>
        /// Disables the grid line to be drawn in dashed mode.
        /// </summary>
        public void DisableGridDashedLine()
        {
            gridDashPathEffect = null;
        }

        /// <summary>
        /// Returns true if the grid dashed-line effect is enabled, false if not.
        /// </summary>
        public bool IsGridDashedLineEnabled
        {
            get => gridDashPathEffect != null;
        }

        /// <summary>
        ///  Enables the axis line to be drawn in dashed mode, e.g.like this
        /// "- - - - - -". THIS ONLY WORKS IF HARDWARE-ACCELERATION IS TURNED OFF.
        /// Keep in mind that hardware acceleration boosts performance.
        /// </summary>
        public DashPathEffect AxisLineDashedLine
        {
            get => axisLineDashPathEffect;
            set => axisLineDashPathEffect = value;
        }

        /// <summary>
        /// Disables the axis line to be drawn in dashed mode.
        /// </summary>
        public void DisableAxisLineDashedLine()
        {
            axisLineDashPathEffect = null;
        }

        /// <summary>
        /// Returns true if the axis dashed-line effect is enabled, false if not.
        /// </summary>
        public bool IsAxisLineDashedLineEnabled
        {
            get => axisLineDashPathEffect != null;
        }

#endif

        /// <summary>
        /// The maximum number of labels on the axis
        /// </summary>
        public int AxisMaxLabels
        {
            get => axisMaxLabels;
            set
            {
                if (value > 0)
                    axisMaxLabels = value;
            }
        }

        public float Granularity
        {
            get => granularity;
            set
            {
                granularity = value;
                // set this to true if it was disabled, as it makes no sense to call this method with granularity disabled
                granularityEnabled = true;
            }
        }

        /// <summary>
        ///  When true, axis labels are controlled by the `granularity` property.
        /// When false, axis values could possibly be repeated.
        /// This could happen if two adjacent axis values are rounded to same value.
        /// If using granularity this could be avoided by having fewer axis values visible.
        /// </summary>
        public bool GranularityEnabled
        {
            get => granularityEnabled;
            set => granularityEnabled = value;
        }

        /// <summary>
        /// if true, the set number of y-labels will be forced
        /// </summary>
        public bool ForceLabels
        {
            get => forceLabels;
            set => forceLabels = value;
        }

        /// <summary>
        /// flag indicating if the grid lines for this axis should be drawn
        /// </summary>
        public bool DrawGridLines
        {
            get => drawGridLines;
            set => drawGridLines = value;
        }

        /// <summary>
        /// Returns true if drawing grid lines is enabled for this axis.
        /// </summary>
        public bool IsDrawGridLinesEnabled => drawGridLines;

        /// <summary>
        ///  Returns true if the line alongside the axis should be drawn.
        /// </summary>
        public bool IsDrawAxisLineEnabled => drawAxisLine;


        public bool IsCenterAxisLabelsEnabled => CenterAxisLabels && entryCount > 0;

        /// <summary>
        /// Gets or Sets the width of the border surrounding the chart in dp.
        /// </summary>
        public float AxisLineWidth
        {
            get => axisLineWidth;
            set
            {
#if __ANDROID__ || SKIASHARP
                axisLineWidth = value.DpToPixel();
#else
                axisLineWidth = value;
#endif
            }
        }


        /// <summary>
        /// Sets the width of the grid lines that are drawn away from each axis
        /// label.
        /// </summary>
        public float GridLineWidth
        {
            get => axisLineWidth;
            set
            {
#if __ANDROID__ || SKIASHARP
                gridLineWidth = value.DpToPixel();
#else
                gridLineWidth = value;
#endif
            }
        }


        /**
         * Set this to true to enable drawing the labels of this axis (this will not
         * affect drawing the grid lines or axis lines).
         *
         * @param enabled
         */
        public bool DrawLabels
        {
            set => drawLabels = value;
        }

        /// <summary>
        /// Returns true if drawing the labels is enabled for this axis.
        /// </summary>
        public bool IsDrawLabelsEnabled => drawLabels;

        /**
         * Sets the number of label entries for the y-axis max = 25, min = 2, default: 6, be aware
         * that this number is not fixed.
         *
         * @param count the number of y-axis labels that should be displayed
         */
        public int LabelCount
        {
            get => labelCount;
            set
            {
                if (value > AxisMaxLabels)
                    value = AxisMaxLabels;
                if (value < AxisMinLabels)
                    value = AxisMinLabels;

                labelCount = value;
                ForceLabels = false;
            }
        }

        /**
         * sets the number of label entries for the y-axis max = 25, min = 2, default: 6, be aware
         * that this number is not
         * fixed (if force == false) and can only be approximated.
         *
         * @param count the number of y-axis labels that should be displayed
         * @param force if enabled, the set label count will be forced, meaning that the exact
         *              specified count of labels will
         *              be drawn and evenly distributed alongside the axis - this might cause labels
         *              to have uneven values
         */
        public void SetLabelCount(int count, bool force)
        {
            LabelCount = count;
            ForceLabels = force;
        }

        /**
         * Returns true if focing the y-label count is enabled. Default: false
         *
         * @return
         */
        public bool IsForceLabelsEnabled => forceLabels;

        /**
         * Adds a new LimitLine to this axis.
         *
         * @param l
         */
        public void AddLimitLine(LimitLine l)
        {
            LimitLines.Add(l);

            if (LimitLines.Count > 6)
            {
                System.Diagnostics.Trace.TraceError("MPAndroiChart",
                        "Warning! You have more than 6 LimitLines on your axis, do you really want " +
                                "that?");
            }
        }

        /**
         * Removes the specified LimitLine from the axis.
         *
         * @param l
         */
        public void RemoveLimitLine(LimitLine l)
        {
            LimitLines.Remove(l);
        }

        /**
         * Removes all LimitLines from the axis.
         */
        public void RemoveAllLimitLines()
        {
            LimitLines.Clear();
        }

        /**
         * If this is set to true, the LimitLines are drawn behind the actual data,
         * otherwise on top. Default: false
         *
         * @param enabled
         */
        public bool DrawLimitLinesBehindData
        {
            get => drawLimitLineBehindData;
            set => drawLimitLineBehindData = value;
        }

        /// <summary>
        /// If this is set to false, the grid lines are draw on top of the actual data,
        /// otherwise behind.Default: true
        /// </summary>
        public bool DrawGridLinesBehindData
        {
            get => drawGridLinesBehindData;
            set => drawGridLinesBehindData = value;
        }

        public bool IsDrawGridLinesBehindDataEnabled => drawGridLinesBehindData;

        /**
         * Returns the longest formatted label (in terms of characters), this axis
         * contains.
         *
         * @return
         */
        public string GetLongestLabel()
        {
            string longest = string.Empty;

            for (int i = 0; i < Entries.Count; i++)
            {
                string text = GetFormattedLabel(i);

                if (text != null && longest.Length < text.Length)
                    longest = text;
            }

            return longest;
        }

        public string GetFormattedLabel(int index)
        {
            if (index < 0 || index >= Entries.Count)
                return string.Empty;
            else
                return ValueFormatter.GetFormattedValue(Entries[index], this);
        }

        /**
         * Returns the formatter used for formatting the axis labels.
         *
         * @return
         */
        public IAxisValueFormatter ValueFormatter
        {
            get
            {
                if (axisValueFormatter == null ||
                    (axisValueFormatter is DefaultAxisValueFormatter format &&
                            (format.Decimals != Decimals)))
                    axisValueFormatter = new DefaultAxisValueFormatter(Decimals);

                return axisValueFormatter;
            }
            set
            {

                if (value == null)
                    axisValueFormatter = new DefaultAxisValueFormatter(Decimals);
                else
                    axisValueFormatter = value;
            }
        }

        /**
         * ###### BELOW CODE RELATED TO CUSTOM AXIS VALUES ######
         */

        public float AxisMaximum
        {
            get => axisMaximum;
            set
            {
                CustomAxisMax = true;
                axisMaximum = value;
                axisRange = Math.Abs(value - axisMinimum);
            }
        }

        public float AxisMinimum
        {
            get => axisMinimum;
            set
            {
                CustomAxisMin = true;
                axisMinimum = value;
                axisRange = Math.Abs(axisMaximum - value);
            }
        }

        /**
         * By calling this method, any custom maximum value that has been previously set is reseted,
         * and the calculation is
         * done automatically.
         */
        public void ResetAxisMaximum()
        {
            CustomAxisMax = false;
        }

        /**
         * Returns true if the axis max value has been customized (and is not calculated automatically)
         *
         * @return
         */
        public bool IsAxisMaxCustom => CustomAxisMax;

        /**
         * By calling this method, any custom minimum value that has been previously set is reseted,
         * and the calculation is
         * done automatically.
         */
        public void ResetAxisMinimum()
        {
            CustomAxisMin = false;
        }

        /**
         * Returns true if the axis min value has been customized (and is not calculated automatically)
         *
         * @return
         */
        public bool IsAxisMinCustom => CustomAxisMin;


        /**
         * Calculates the minimum / maximum  and range values of the axis with the given
         * minimum and maximum values from the chart data.
         *
         * @param dataMin the min value according to chart data
         * @param dataMax the max value according to chart data
         */
        public virtual void Calculate(float dataMin, float dataMax)
        {
            // if custom, use value as is, else use data value
            float min = CustomAxisMin ? axisMinimum : (dataMin - spaceMin);
            float max = CustomAxisMax ? axisMaximum : (dataMax + spaceMax);

            // temporary range (before calculations)
            float range = Math.Abs(max - min);

            // in case all values are equal
            if (range == 0f)
            {
                max += 1f;
                min -= 1f;
            }

            axisMinimum = min;
            axisMaximum = max;

            // actual range
            axisRange = Math.Abs(max - min);
        }

        /**
         * Gets extra spacing for `axisMinimum` to be added to automatically calculated `axisMinimum`
         */
        public float SpaceMin
        {
            get => spaceMin;
            set => spaceMin = value;
        }


        /**
         * Gets extra spacing for `axisMaximum` to be added to automatically calculated `axisMaximum`
         */
        public float SpaceMax
        {
            get => spaceMax;
            set => spaceMax = value;
        }
    }
}
