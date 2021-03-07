namespace XF.ChartLibrary.Jobs
{
    public partial class MoveViewJob
    {
        public override void Run()
        {
            Points[0] = XValue;
            Points[1] = YValue;

            Transformer.PointValueToPixel(Points);
            ViewPortHandler.CenterViewPort(Points, View);

            RecycleInstance(this);
        }
    }
}
