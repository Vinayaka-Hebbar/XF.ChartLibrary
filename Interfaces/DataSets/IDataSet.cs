using System.Collections.Generic;
using XF.ChartLibrary.Components;
using XF.ChartLibrary.Data;
using XF.ChartLibrary.Formatter;

#if __IOS__ || __TVOS
using Color = UIKit.UIColor;
using Font = UIKit.UIFont;
using DashPathEffect = XF.ChartLibrary.Utils.DashPathEffect;
#elif __ANDROID__
using Color = Android.Graphics.Color;
using Font = Android.Graphics.Typeface;
     using DashPathEffect = Android.Graphics.DashPathEffect;
#elif NETSTANDARD
using Color = SkiaSharp.SKColor;
using Font = SkiaSharp.SKTypeface;
using DashPathEffect = SkiaSharp.SKPathEffect;
#endif


namespace XF.ChartLibrary.Interfaces.DataSets
{
    public interface IDataSetBase
    {
        /// Use this method to tell the data set that the underlying data has changed
        void NotifyDataSetChanged();

        /// Calculates the minimum and maximum x and y values (xMin, xMax, yMin, yMax).
        void CalcMinMax();

        /// Calculates the min and max y-values from the Entry closest to the given fromX to the Entry closest to the given toX value.
        /// This is only needed for the autoScaleMinMax feature.
        void CalcMinMaxY(float fromX, float toX);

        /// The minimum y-value this DataSet holds
        float YMin { get; }

        /// The maximum y-value this DataSet holds
        float YMax { get; }

        /// The minimum x-value this DataSet holds
        float XMin { get; }

        /// The maximum x-value this DataSet holds
        float XMax { get; }

        /// The number of y-values this DataSet represents
        int EntryCount { get; }

        /// - Parameters:
        ///   - xValue: x-value of the entry to search for
        ///   - closestToY: If there are multiple y-values for the specified x-value,
        ///   - rounding: Rounding method if exact value was not found
        /// - Returns: The array-index of the specified entry.
        /// If the no Entry at the specified x-value is found, this method returns the index of the Entry at the closest x-value according to the rounding.
        int EntryIndex(float xValue, float yValue, DataSetRounding rounding);

        /// Removes the Entry object at the given index in the values array from the DataSet.
        ///
        /// *optional feature, can return `false` ifnot implemented*
        ///
        /// - Parameters:
        ///   - index: the index of the entry to remove
        /// - Returns: `true` if the entry was removed successfully, `false` ifthe entry does not exist or if this feature is not supported
        bool RemoveEntry(int index);

        /// Removes the Entry object closest to the given x-value from the DataSet.
        ///
        /// *optional feature, can return `false` ifnot implemented*
        ///
        /// - Parameters:
        ///   - x: the x-value to remove
        /// - Returns: `true` if the entry was removed successfully, `false` ifthe entry does not exist or if this feature is not supported
        bool RemoveEntry(float x);

        /// Removes the first Entry (at index 0) of this DataSet from the entries array.
        ///
        /// *optional feature, can return `false` ifnot implemented*
        ///
        /// - Returns: `true` if the entry was removed successfully, `false` ifthe entry does not exist or if this feature is not supported
        bool RemoveFirst();

        /// Removes the last Entry (at index 0) of this DataSet from the entries array.
        ///
        /// *optional feature, can return `false` ifnot implemented*
        ///
        /// - Returns: `true` if the entry was removed successfully, `false` ifthe entry does not exist or if this feature is not supported
        bool RemoveLast();

        /// Removes all values from this DataSet and does all necessary recalculations.
        ///
        /// *optional feature, could throw if not implemented*
        void Clear();

        // MARK: - Styling functions and accessors

        /// The label string that describes the DataSet.
        string Label { get; }

        /// The axis this DataSet should be plotted against.
        YAxisDependency AxisDependency { get; }

        /// List representing all colors that are used for drawing the actual values for this DataSet
        IList<Color> ValueColors { get; }

        /// All the colors that are used for this DataSet.
        /// Colors are reused as soon as the number of Entries the DataSet represents is higher than the size of the colors array.
        IList<Color> Colors { get; }

        /// - Returns: The color at the given index of the DataSet's color array.
        /// This prevents out-of-bounds by performing a modulus on the color index, so colours will repeat themselves.
        Color ColorAt(int index);

        void ResetColors();

        Color Color { get; set; }

        /// `true` if value highlighting is enabled for this dataset
        bool IsHighlightEnabled { get; set; }

        /// Custom formatter that is used instead of the auto-formatter if set
        IValueFormatter ValueFormatter { get; set; }

        bool NeedsFormatter { get; }

        /// Sets/get a single color for value text.
        /// Setting the color clears the colors array and adds a single color.
        /// Getting will return the first color in the array.
        Color ValueTextColor
        { get; set; }

        /// - Returns: The color at the specified index that is used for drawing the values inside the chart. Uses modulus internally.
        Color ValueTextColorAt(int index);

        /// the font for the value-text labels
        Font ValueFont { get; set; }

#if !__IOS__ || !__TVOS__
        float ValueTextSize { get; set; }
#endif

        /// The rotation angle (in degrees) for value-text labels
        float ValueLabelAngle { get; set; }

        /// The form to draw for this dataset in the legend.
        ///
        /// Return `.Default` to use the default legend form.
        Form Form { get; }

        /// The form size to draw for this dataset in the legend.
        ///
        /// Return `NaN` to use the default legend form size.
        float FormSize { get; }

        /// The line width for drawing the form of this dataset in the legend
        ///
        /// Return `NaN` to use the default legend form line width.
        float FormLineWidth { get; }

        /// Line dash configuration for legend shapes that consist of lines.
        public DashPathEffect FormLineDashEffect { get; set; }

        /// `true` if y-value drawing is enabled, `false` ifnot
        bool IsDrawValuesEnabled { get; set; }

        /// Returns true if y-icon drawing is enabled, false if not
        bool IsDrawIconsEnabled { get; set; }

        /// Offset of icons drawn on the chart.
        ///
        /// For all charts except Pie and Radar it will be ordinary (x offset, y offset).
        ///
        /// For Pie and Radar chart it will be (y offset, distance from center offset); so if you want icon to be rendered under value, you should increase X component of CGPoint, and if you want icon to be rendered closet to center, you should decrease height component of CGPoint.
        System.Drawing.Point IconsOffset { get; set; }

        ///  Get Set the visibility of this DataSet. If not visible, the DataSet will not be drawn to the chart upon refreshing it.
        bool IsVisible { get; set; }
    }

    public interface IDataSet<TEntry> : IDataSetBase where TEntry : Entry
    {
        /// - Throws: out of bounds
        /// if `i` is out of bounds, it may throw an out-of-bounds exception
        /// - Returns: The entry object found at the given index (not x-value!)
        TEntry this[int i] { get; }
        /// - Parameters:
        ///   - xValue: the x-value
        ///   - closestToY: If there are multiple y-values for the specified x-value,
        ///   - rounding: determine whether to round up/down/closest if there is no Entry matching the provided x-value
        /// - Returns: The first Entry object found at the given x-value with binary search.
        /// If the no Entry at the specified x-value is found, this method returns the Entry at the closest x-value according to the rounding.
        /// nil if no Entry object at that x-value.
        TEntry EntryForXValue(
            float xValue,
            float yValue,
            DataSetRounding rounding);

        /// - Parameters:
        ///   - xValue: the x-value
        ///   - closestToY: If there are multiple y-values for the specified x-value,
        /// - Returns: The first Entry object found at the given x-value with binary search.
        /// If the no Entry at the specified x-value is found, this method returns the Entry at the closest x-value.
        /// nil if no Entry object at that x-value.
        TEntry EntryForXValue(
         float xValue,
            float yValue);

        /// - Returns: All Entry objects found at the given x-value with binary search.
        /// An empty array if no Entry object at that x-value.
        IList<TEntry> EntriesForXValue(float xValue);

        /// Adds an Entry to the DataSet dynamically.
        ///
        /// *optional feature, can return `false` ifnot implemented*
        ///
        /// Entries are added to the end of the list.
        ///
        /// - Parameters:
        ///   - e: the entry to add
        /// - Returns: `true` if the entry was added successfully, `false` ifthis feature is not supported
        bool AddEntry(TEntry e);

        /// Adds an Entry to the DataSet dynamically.
        /// Entries are added to their appropriate index in the values array respective to their x-position.
        /// This will also recalculate the current minimum and maximum values of the DataSet and the value-sum.
        ///
        /// *optional feature, can return `false` ifnot implemented*
        ///
        /// Entries are added to the end of the list.
        ///
        /// - Parameters:
        ///   - e: the entry to add
        /// - Returns: `true` if the entry was added successfully, `false` ifthis feature is not supported
        bool AddEntryOrdered(TEntry e);

        // MARK: - Data functions and accessors
        /// - Parameters:
        ///   - e: the entry to search for
        /// - Returns: The array-index of the specified entry
        int EntryIndex(TEntry e);

        /// Removes an Entry from the DataSet dynamically.
        ///
        /// *optional feature, can return `false` ifnot implemented*
        ///
        /// - Parameters:
        ///   - entry: the entry to remove
        /// - Returns: `true` if the entry was removed successfully, `false` ifthe entry does not exist or if this feature is not supported
        bool RemoveEntry(TEntry e);

        /// Checks if this DataSet contains the specified Entry.
        ///
        /// - Returns: `true` if contains the entry, `false` ifnot.
        bool Contains(TEntry e);
    }

    public interface IDataSet : IDataSetBase
    {
        /// - Throws: out of bounds
        /// if `i` is out of bounds, it may throw an out-of-bounds exception
        /// - Returns: The entry object found at the given index (not x-value!)
        Entry this[int i] { get; }
        /// - Parameters:
        ///   - xValue: the x-value
        ///   - closestToY: If there are multiple y-values for the specified x-value,
        ///   - rounding: determine whether to round up/down/closest if there is no Entry matching the provided x-value
        /// - Returns: The first Entry object found at the given x-value with binary search.
        /// If the no Entry at the specified x-value is found, this method returns the Entry at the closest x-value according to the rounding.
        /// nil if no Entry object at that x-value.
        Entry EntryForXValue(
            float xValue,
            float yValue,
            DataSetRounding rounding);

        /// - Parameters:
        ///   - xValue: the x-value
        ///   - closestToY: If there are multiple y-values for the specified x-value,
        /// - Returns: The first Entry object found at the given x-value with binary search.
        /// If the no Entry at the specified x-value is found, this method returns the Entry at the closest x-value.
        /// nil if no Entry object at that x-value.
        Entry EntryForXValue(
         float xValue,
            float yValue);

        /// - Returns: All Entry objects found at the given x-value with binary search.
        /// An empty array if no Entry object at that x-value.
        IList<Entry> EntriesForXValue(double xValue);

        // MARK: - Data functions and accessors
        /// - Parameters:
        ///   - e: the entry to search for
        /// - Returns: The array-index of the specified entry
        int EntryIndex(Entry e);

        /// Checks if this DataSet contains the specified Entry.
        ///
        /// - Returns: `true` if contains the entry, `false` ifnot.
        bool Contains(Entry e);
    }
}
