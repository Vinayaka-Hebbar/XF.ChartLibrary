using XF.ChartLibrary.Utils;

namespace XF.ChartLibrary.Jobs
{
    public partial class MoveViewJob : ViewPortJob
    {
        private readonly static ObjectPool<MoveViewJob> pool;

        static MoveViewJob()
        {
            pool = ObjectPool<MoveViewJob>.Create(2, new MoveViewJob(null, 0, 0, null, null));
            pool.ReplenishPercentage = 0.5f;
        }

        public static MoveViewJob GetInstance(ViewPortHandler viewPortHandler, float xValue, float yValue, Transformer trans, Charts.IChartBase v)
        {
            MoveViewJob result = pool.Get();
            result.ViewPortHandler = viewPortHandler;
            result.XValue = xValue;
            result.YValue = yValue;
            result.Transformer = trans;
            result.View = v;
            return result;
        }

        public static void RecycleInstance(MoveViewJob instance)
        {
            pool.Recycle(instance);
        }

        public MoveViewJob(ViewPortHandler viewPortHandler, float xValue, float yValue, Transformer trans, Charts.IChartBase v) : base(viewPortHandler, xValue, yValue, trans, v)
        {
        }

        public override IPoolable Instantiate()
        {
            return new MoveViewJob(ViewPortHandler, XValue, YValue, Transformer, View);
        }
    }
}
