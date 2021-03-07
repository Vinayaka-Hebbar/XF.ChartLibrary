#nullable enable

using System.Collections.Generic;

#if NETSTANDARD || SKIASHARP
using Color = SkiaSharp.SKColor;
#elif __IOS__ || __TVOS
using Color = UIKit.UIColor;
#elif __ANDROID__
using Color = Android.Graphics.Color;
#endif

namespace XF.ChartLibrary.Data
{

    public class PieDataSet : DataSet<PieEntry>, Interfaces.DataSets.IPieDataSet
    {
        private Color? highlightColor;
        private bool isValueLineVariableLength = true;
        private float valueLinePart1OffsetPercentage = 75.0f;
        private float valueLinePart2Length = 0.4f;
        private float valueLinePart1Length = 0.4f;
        private ValuePosition xValuePosition;
        private ValuePosition yValuePosition;
        private float sliceSpace;
        private float selectionShift = 18f;
        private Color valueLineColor = Utils.ColorTemplate.Black;
        private bool isUseValueColorForLineEnabled;
        private float valueLineWidth = 1f;
        private bool automaticallyDisableSliceSpacing;

        public PieDataSet(IList<PieEntry> entries, string label) : base(entries, label)
        {
        }

        public ValuePosition XValuePosition
        {
            get => xValuePosition;
            set => xValuePosition = value;
        }

        public ValuePosition YValuePosition
        {
            get => yValuePosition;
            set => yValuePosition = value;
        }

        /// <summary>
        /// Sets the space that is left out between the piechart-slices in dp.
        /// Default: 0 --> no space, maximum 20f
        /// </summary>
        public float SliceSpace
        {
            get => sliceSpace;
            set
            {
                if (value > 20)
                    value = 20f;
                if (value < 0)
                    value = 0f;

#if PIXELSCALE

                sliceSpace = value.DpToPixel();
#else

                sliceSpace = value;
#endif
            }
        }


        public float SelectionShift
        {
            get => selectionShift;
            set
            {
#if PIXELSCALE
                selectionShift = value.DpToPixel();
#else
                selectionShift = value;
#endif
            }
        }

        /// <summary>
        /// When valuePosition is OutsideSlice, indicates line color
        /// </summary>
        public Color ValueLineColor
        {
            get => valueLineColor;
            set => valueLineColor = value;
        }

        public bool IsUseValueColorForLineEnabled
        {
            get => isUseValueColorForLineEnabled;
            set => isUseValueColorForLineEnabled = value;
        }

        /// <summary>
        /// When valuePosition is OutsideSlice, indicates line width
        /// </summary>
        public float ValueLineWidth
        {
            get => valueLineWidth;
            set => valueLineWidth = value;
        }

        /// <summary>
        /// When enabled, slice spacing will be 0.0 when the smallest value is going to be
        /// smaller than the slice spacing itself.
        /// </summary>
        public bool IsAutomaticallyDisableSliceSpacing
        {
            get => automaticallyDisableSliceSpacing;
            set => automaticallyDisableSliceSpacing = value;
        }

        public float ValueLinePart1OffsetPercentage
        {
            get => valueLinePart1OffsetPercentage;
            set => valueLinePart1OffsetPercentage = value;
        }

        /// <summary>
        /// When valuePosition is OutsideSlice, indicates length of first half of the line
        /// </summary>
        public float ValueLinePart1Length
        {
            get => valueLinePart1Length;
            set => valueLinePart1Length = value;
        }

        /// <summary>
        /// When valuePosition is OutsideSlice, indicates length of first half of the line
        /// </summary>
        public float ValueLinePart2Length
        {
            get => valueLinePart2Length;
            set => valueLinePart2Length = value;
        }

        /// <summary>
        /// When valuePosition is OutsideSlice, this allows variable line length
        /// </summary>
        public bool IsValueLineVariableLength
        {
            get => isValueLineVariableLength;
            set => isValueLineVariableLength = value;
        }

        /// <summary>
        /// Sets the color for the highlighted sector (null for using entry color) */
        /// </summary>
        public Color? HighlightColor
        {
            get => highlightColor;
            set => highlightColor = value;
        }

        protected override void CalcMinMax(PieEntry e)
        {
            if (e == null)
                return;
            CalcMinMaxY(e);
        }

        public enum ValuePosition
        {
            InsideSlice,
            OutsideSlice
        }
    }
}
