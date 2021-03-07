using System;
using XF.ChartLibrary.Data;
using XF.ChartLibrary.Interfaces;
using XF.ChartLibrary.Interfaces.DataProvider;
using XF.ChartLibrary.Interfaces.DataSets;

namespace XF.ChartLibrary.Charts
{
    public partial class LineChart : BarLineChartBase<LineData, ILineDataSet>, ILineChartDataProvider
    {
        public override void Initialize()
        {
            base.Initialize();
            renderer = new Renderer.LineChartRenderer(this, Animator, ViewPortHandler);
        }

        IChartData IBarLineScatterCandleBubbleProvider.Data => data;
    }
}
