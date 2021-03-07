namespace XF.ChartLibrary.Charts
{
    partial class LineChart
    {
        public override void OnUnbind()
        {
            if (renderer is Renderer.LineChartRenderer l)
            {
                l.ReleaseBitmap();
            }
        }
    }
}
