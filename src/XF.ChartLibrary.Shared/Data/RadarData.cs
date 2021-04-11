using System.Collections.Generic;
using XF.ChartLibrary.Interfaces.DataSets;

namespace XF.ChartLibrary.Data
{
    public class RadarData : ChartData<IRadarDataSet, RadarEntry>
    {
        public RadarData(IList<IRadarDataSet> sets) : base(sets)
        {
        }

        /// <summary>
        /// the labels that should be drawn around the RadarChart at the end of each web line.
        /// </summary>
        public IList<string> Labels { get; set; }

        public RadarData SetLabels(params string[] labels)
        {
            Labels = labels;
            return this;
        }
    }
}
