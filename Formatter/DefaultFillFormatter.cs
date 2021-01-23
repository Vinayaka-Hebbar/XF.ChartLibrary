﻿using XF.ChartLibrary.Data;
using XF.ChartLibrary.Interfaces.DataProvider;

namespace XF.ChartLibrary.Formatter
{
    public class DefaultFillFormatter : IFillFormatter
    {
        public static readonly DefaultFillFormatter Instance = new DefaultFillFormatter();

        public double GetFillLinePosition(ILineDataSet dataSet, ILineChartDataProvider dataProvider)
        {
            var chartMaxY = dataProvider.YChartMax;
            var chartMinY = dataProvider.YChartMin;

            LineData data = dataProvider.Data;

            double fillMin;
            if (dataSet.YMax > 0 && dataSet.YMin < 0)
            {
                fillMin = 0f;
            }
            else
            {

                double max, min;

                if (data.YMax > 0)
                    max = 0f;
                else
                    max = chartMaxY;
                if (data.YMin < 0)
                    min = 0f;
                else
                    min = chartMinY;

                fillMin = dataSet.YMin >= 0 ? min : max;
            }

            return fillMin;
        }
    }
}
