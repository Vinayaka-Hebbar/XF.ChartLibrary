using System.Collections.Generic;
using System.Drawing;
using XF.ChartLibrary.Components;
using XF.ChartLibrary.Formatter;
using XF.ChartLibrary.Interfaces.DataSets;

#if __IOS__ || __TVOS
using Color = UIKit.UIColor;
using Font = UIKit.UIFont;
#elif __ANDROID__
using Color = Android.Graphics.Color;
    using Font = Android.Graphics.Typeface;
#elif NETSTANDARD
using Color = SkiaSharp.SKColor;
using Font = SkiaSharp.SKTypeface;
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

        public string Label { get; }

        public YAxisDependency AxisDependency { get; set; }

        public IList<Color> ValueColors
        {
            get => valueColors;
            set => valueColors = value;
        }

        public IList<Color> Colors
        {
            get => colors;
            set => colors = value;
        }

        public Color Color
        {
            get
            {
                if (colors == null || colors.Count == 0)
                    return default;
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
                    return default;
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

        public Font ValueFont { get; set; }

#if !__IOS__ || !__TVOS__

        private float valueTextSize;

        protected DataSetBase(string label)
        {
            Label = label;
        }

        public float ValueTextSize
        {
            get => valueTextSize;
            set => valueTextSize = value;
        }
#endif
        public float ValueLabelAngle { get; set; }

        public Form Form { get; }

        public float FormSize { get; }

        public float FormLineWidth { get; }

        public float FormLineDashPhase { get; }

        public IList<float> FormLineDashLengths { get; }

        public bool IsDrawValuesEnabled { get; set; } = true;

        public bool IsDrawIconsEnabled { get; set; }

        public Point IconsOffset { get; set; }

        public bool IsVisible { get; set; }

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
                return default;
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
