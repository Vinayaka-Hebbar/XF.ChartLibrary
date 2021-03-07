using System;
using System.Collections.Generic;
using System.Text;
using XF.ChartLibrary.Formatter;

#if NETSTANDARD || SKIASHARP
using Color = SkiaSharp.SKColor;
using DashPathEffect = SkiaSharp.SKPathEffect;
using ColorList = SkiaSharp.SKColors;
#elif __IOS__ || __TVOS
    using Color = UIKit.UIColor;
    using ColorList = UIKit.UIColor;
    using DashPathEffect = XF.ChartLibrary.Utils.DashPathEffect;
#elif __ANDROID__
    using Color = Android.Graphics.Color;
    using ColorList = Android.Graphics.Color;
    using DashPathEffect = Android.Graphics.DashPathEffect;
#endif

namespace XF.ChartLibrary.Data
{
    public class LineDataSet : LineRadarDataSet<Entry>, Interfaces.DataSets.ILineDataSet
    {
        private IList<Color> circleColors;

        private IFillFormatter fillFormatter;

        private bool drawCircles = true;

        private bool drawCircleHole = true;

        private DashPathEffect dashPathEffect;

        private float circleRadius;

        private float circleHoleRadius;

        /// <summary>
        /// Drawing mode for this line dataset
        /// </summary>
        public LineMode Mode { get; set; } = LineMode.Linear;

        /// <summary>
        /// List representing all colors that are used for the circles
        /// </summary>
        public IList<Color> CircleColors
        {
            get => circleColors;
            set
            {
                if (value == null)
                {
                    value = new List<Color>()
                    {
                        Utils.ColorTemplate.FromRGB(140, 234, 255)
                    };
                }
                circleColors = value;
            }
        }

        /// <summary>
        /// the color of the inner circles
        /// </summary>
        public Color CircleHoleColor { get; set; } = ColorList.White;

        /// <summary>
        /// the radius of the circle-shaped value indicators
        /// </summary>
        public float CircleRadius
        {
            get => circleRadius;
            set
            {
#if PIXELSCALE
                circleRadius = value.DpToPixel();
#else
                circleRadius = value; 
#endif
            }
        }
        /// <summary>
        ///  the hole radius of the circle-shaped value indicators
        /// </summary>
        public float CircleHoleRadius
        {
            get => circleHoleRadius;
            set
            {
#if PIXELSCALE
                circleHoleRadius = value.DpToPixel();
#else
                circleHoleRadius = value; 
#endif
            }
        }
        /// <summary>
        /// sets the intensity of the cubic lines
        /// </summary>
        public float CubicIntensity { get; set; } = 0.2f;

        /// <summary>
        /// if true, drawing circles is enabled
        /// </summary>
        public bool IsDrawCirclesEnabled
        {
            get => drawCircles;
            set => drawCircles = value;
        }

        public bool IsDrawCircleHoleEnabled
        {
            get => drawCircleHole;
            set => drawCircleHole = value;
        }

        public DashPathEffect DashPathEffect
        {
            get => dashPathEffect;
            set => dashPathEffect = value;
        }

        /// <summary>
        /// formatter for customizing the position of the fill-line
        /// </summary>
        public IFillFormatter FillFormatter
        {
            get
            {
                return fillFormatter ?? DefaultFillFormatter.Instance;
            }
            set
            {
                fillFormatter = value;
            }
        }

        public int CircleColorCount => circleColors.Count;

        public bool IsDashedLineEnabled => dashPathEffect != null;

        public LineDataSet(IList<Entry> yVals, string label) : base(yVals, label)
        {
            circleColors = new List<Color>()
                    {
                        Utils.ColorTemplate.DefaultColor
                    };
#if __IOS__ || __TVOS__
            circleRadius = 6f;
            circleHoleRadius = 4f;
#else
            circleRadius = 4f.DpToPixel();
            circleHoleRadius = 2.5f.DpToPixel();
#endif
        }

        public enum LineMode
        {
            Linear,
            Stepped,
            CubicBezier,
            HorizontalBezier
        }

        public Color GetCircleColor(int index)
        {
            return circleColors[index];
        }
    }
}
