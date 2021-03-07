using System;
using System.Collections.Generic;
using XF.ChartLibrary.Interfaces.DataSets;

namespace XF.ChartLibrary.Data
{
    public class PieData : ChartData<IPieDataSet, PieEntry>
    {
        public PieData()
        {
        }

        public PieData(params IPieDataSet[] sets) : base(sets)
        {

        }

        public PieData(IList<IPieDataSet> sets) : base(sets)
        {
        }

        /// <summary>
        /// Gets, Sets the PieDataSet this data object should represent.
        /// </summary>
        public IPieDataSet DataSet
        {
            get
            {
                return dataSets[0];
            }
            set
            {
                dataSets.Clear();
                dataSets.Add(value);
                NotifyDataChanged();
            }
        }

        /// <summary>
        /// The PieData object can only have one DataSet. Use getDataSet() method instead.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public override IPieDataSet this[int index]
        {
            get
            {
                return index == 0 ? dataSets[index] : null;
            }
        }

        public override IPieDataSet GetDataSetByLabel(string label, bool ignorecase)
        {
            return ignorecase ? string.Equals(label, dataSets[0].Label, StringComparison.OrdinalIgnoreCase) ? dataSets[0]
                    : null : string.Equals(label, dataSets[0].Label) ? dataSets[0] : null;
        }

        public override Entry GetEntryForHighlight(Highlight.Highlight highlight)
        {
            return ((IDataSet<PieEntry>)dataSets[0])[(int)highlight.X];
        }

        /// <summary>
        /// Returns the sum of all values in this PieData object.
        /// </summary>
        public float YValueSum
        {
            get
            {
                float sum = 0;

                IDataSet<PieEntry> dataSet = dataSets[0];
                for (int i = 0; i < dataSet.EntryCount; i++)
                    sum += dataSet[i].Y;
                return sum;
            }
        }
    }
}
