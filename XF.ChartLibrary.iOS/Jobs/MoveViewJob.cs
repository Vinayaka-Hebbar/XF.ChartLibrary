namespace XF.ChartLibrary.Jobs
{
    partial class MoveViewJob
    {
        public override void Run()
        {
            ViewPortHandler.CenterViewPort(Transformer.PointValueToPixel(XValue, YValue), View);
            RecycleInstance(this);
        }
    }
}
