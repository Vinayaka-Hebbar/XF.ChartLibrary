using System;
using XF.ChartLibrary.Utils;
using XF.ChartLibrary.Charts;

namespace XF.ChartLibrary.Jobs
{
    public abstract partial class ViewPortJob : IPoolable
    {
        protected float[] Points = new float[2];

        private int ownerId;
        public readonly ViewPortHandler ViewPortHandler;
        public float XValue { get; protected set; }
        public float YValue { get; protected set; }
        public Transformer Transformer { get; }
        public IChartBase View { get; }

        public int CurrentOwnerId
        {
            get => ownerId;
            set => ownerId = value;
        }

        public ViewPortJob(
           ViewPortHandler viewPortHandler,
          float xValue,
          float yValue,
          Transformer transformer,
          IChartBase view)
        {
            ViewPortHandler = viewPortHandler;
            XValue = xValue;
            YValue = yValue;
            Transformer = transformer;
            View = view;
            ownerId = ObjectPool.NoOwner;
            Initialize();
        }

        protected virtual void Initialize()
        {
        }

        public virtual void Run()
        {
            throw new NotImplementedException("`Run()` must be overridden by subclasses");
        }

        public abstract IPoolable Instantiate();
    }
}
