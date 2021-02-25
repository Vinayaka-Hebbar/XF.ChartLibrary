using System.Collections.Generic;
using XF.ChartLibrary.Components;
using XF.ChartLibrary.Formatter;
using XF.ChartLibrary.Interfaces.DataSets;

#if NETSTANDARD || SKIASHARP
using Color = SkiaSharp.SKColor;
using Point = SkiaSharp.SKPoint;
using Font = SkiaSharp.SKTypeface;
using DashPathEffect = SkiaSharp.SKPathEffect;
#elif __IOS__ || __TVOS
using Color = UIKit.UIColor;
using Font = UIKit.UIFont;
using Point = CoreGraphics.CGPoint;
using DashPathEffect = XF.ChartLibrary.Utils.DashPathEffect;
#elif __ANDROID__
using Color = Android.Graphics.Color;
using Point = Android.Graphics.PointF;
using Font = Android.Graphics.Typeface;
using DashPathEffect = Android.Graphics.DashPathEffect;
#endif

namespace XF.ChartLibrary.Data
{
    public enum DataSetRounding
    {
        Up = 0,
        Down = 1,
        Closest = 2,
    }

    public abstract partial class DataSetBase<TEntry> : IDataSet<TEntry> where TEntry : Entry
    {
        private Form form = Form.Default;
        private float formSize = float.NaN;
        private float formLineWidth = float.NaN;

        private IList<Color> colors;

        private IList<Color> valueColors;

        private IValueFormatter valueFormatter;

        public abstract TEntry this[int i] { get; }

        internal float yMax = -float.MaxValue;

        internal float yMin = float.MaxValue;

        internal float xMax = -float.MaxValue;

        internal float xMin = float.MaxValue;

        public float XMin { get => xMin; }

        public float XMax { get => xMax; }

        public float YMin { get => yMin; }

        public float YMax { get => yMax; }

        public abstract int EntryCount { get; }

        public string Label { get; set; }

        public YAxisDependency AxisDependency { get; set; }

        public IList<Color> ValueColors
        {
            get => valueColors ?? Utils.ColorTemplate.DefaultValueColors;

            set => valueColors = value;
        }

        public IList<Color> Colors
        {
            get => colors ?? Utils.ColorTemplate.DefaultColors;
            set => colors = value;
        }

        public Color Color
        {
            get
            {
                if (colors == null || colors.Count == 0)
                {
                    return Utils.ColorTemplate.DefaultColor;
                }
                return colors[0];
            }
            set
            {
                if (colors == null)
                {
                    colors = new List<Color>();
                }
                else
                {
                    colors.Clear();
                }

                colors.Add(value);
            }
        }

        public bool IsHighlightEnabled { get; set; } = true;

        public IValueFormatter ValueFormatter
        {
            get
            {
                if (valueFormatter == null)
                    return DefaultValueFormatter.Instance;
                return valueFormatter;
            }

            set => valueFormatter = value;
        }

        public bool NeedsFormatter => valueFormatter != null;

        public Color ValueTextColor
        {
            get
            {
                if (valueColors == null || valueColors.Count == 0)
                {
                    return Utils.ColorTemplate.Black;
                }
                return valueColors[0];
            }
            set
            {
                if (valueColors == null)
                {
                    valueColors = new List<Color>();
                }
                else
                {
                    valueColors.Clear();
                }

                valueColors.Add(value);
            }
        }


#if !__IOS__ || !__TVOS__

        public Font ValueTypeface { get; set; }

        private float valueTextSize;

        protected DataSetBase(string label)
        {
            Label = label;

#if !__IOS__ || !__TVOS__
            valueTextSize = 12f.DpToPixel();
#endif
        }

        public float ValueTextSize
        {
            get => valueTextSize;
            set
            {
#if __ANDROID__ || SKIASHARP
                valueTextSize = value.DpToPixel();
#else
                valueTextSize = value;
#endif
            }
        }
#else
        public Font ValueFont {get;set;}
#endif
        public float ValueLabelAngle { get; set; }

        public Form Form
        {
            get => form;
            set => form = value;
        }

        public float FormSize
        {
            get => formSize;
            set => formSize = value;
        }

        public float FormLineWidth
        {
            get => formLineWidth;
            set => formLineWidth = value;
        }

        public DashPathEffect FormLineDashEffect { get; set; }

        public bool IsDrawValuesEnabled { get; set; } = true;

        public bool IsDrawIconsEnabled { get; set; }

        public Point IconsOffset { get; set; }

        public bool IsVisible { get; set; } = true;

        public abstract bool AddEntry(TEntry e);

        public abstract bool AddEntryOrdered(TEntry e);

        public abstract void CalcMinMax();

        public abstract void CalcMinMaxY(float fromX, float toX);

        public abstract void Clear();

        public Color ColorAt(int index)
        {
            if (colors == null)
                return default;
            return colors[index % colors.Count];
        }

        public abstract IList<TEntry> EntriesForXValue(float xValue);

        public abstract TEntry EntryForXValue(float xValue, float yValue, DataSetRounding rounding);

        public virtual TEntry EntryForXValue(float xValue, float yValue)
        {
            return EntryForXValue(xValue, yValue, DataSetRounding.Closest);
        }

        public abstract int EntryIndex(float xValue, float yValue, DataSetRounding rounding);

        public void NotifyDataSetChanged()
        {
            CalcMinMax();
        }

        public abstract bool RemoveEntry(int index);

        public bool RemoveEntry(float x)
        {
            var e = EntryForXValue(x, float.NaN);
            if (e == null)
                return false;
            return RemoveEntry(e);
        }

        public bool RemoveFirst()
        {
            return EntryCount > 0 && RemoveEntry(this[0]);
        }

        public bool RemoveLast()
        {
            int count = EntryCount;
            return count > 0 && RemoveEntry(this[count - 1]);
        }

        public void ResetColors()
        {
            if (colors == null)
            {
                colors = new List<Color>();
            }
            else
            {
                colors.Clear();
            }
        }

        public Color ValueTextColorAt(int index)
        {
            if (valueColors == null)
                return Utils.ColorTemplate.DefaultValueTextColor;
            return valueColors[index % valueColors.Count];
        }

        public abstract int EntryIndex(TEntry e);

        public virtual bool RemoveEntry(TEntry e)
        {
            int index = EntryIndex(e);
            if (index > -1)
            {
                return RemoveEntry(index);
            }
            return false;
        }

        public abstract bool Contains(TEntry e);

    }
}
