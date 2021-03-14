using System.Collections.Generic;
using XF.ChartLibrary.Data;
using XF.ChartLibrary.Interfaces.DataProvider;

namespace XF.ChartLibrary.Highlight
{
    public class HorizontalBarHighlighter : BarHighlighter
    {
        public HorizontalBarHighlighter(IBarDataProvider chart) : base(chart)
        {
        }

        public override Highlight GetHighlight(float x, float y)
        {
			var barData = Chart.BarData;

			var pos = GetValsForTouch(y, x);

			Highlight high = GetHighlightForX((float)pos.Y, y, x);
			if (high == null)
				return null;

			var set = barData[high.DataSetIndex];
			if (set.IsStacked)
			{

				return GetStackedHighlight(high,
						set,
						(float)pos.Y,
						(float)pos.X);
			}

			return high;
		}


        protected override IList<Highlight> BuildHighlights(Interfaces.DataSets.IDataSet set, int dataSetIndex, float xVal, Data.DataSetRounding rounding)
        {
			//noinspection unchecked
			var entries = set.EntriesForXValue(xVal);
			if (entries.Count == 0)
			{
				// Try to find closest x-value and take all entries for that x-value
					 Entry closest = set.EntryForXValue(xVal, float.NaN, rounding);
				if (closest != null)
				{
					//noinspection unchecked
					entries = set.EntriesForXValue(closest.X);
				}
			}

            int count = entries.Count;
            if (count == 0)
				return System.Array.Empty<Highlight>();

			Highlight[] highlights = new Highlight[count];


            for (int i = 0; i < count; i++)
			{
                Entry e = entries[i];
                var pixels = Chart.GetTransformer(
						set.AxisDependency).PointValueToPixel(e.Y, e.X);

				highlights[i] = new Highlight(
						e.X, e.Y,
						(float)pixels.X, (float)pixels.Y,
						dataSetIndex, set.AxisDependency);
			}

			return highlights;
		}

        protected override float GetDistance(float x1, float y1, float x2, float y2)
        {
			return System.Math.Abs(y1 - y2);
		}
    }
}
