using System;

namespace XF.ChartLibrary.Animation
{
    public delegate void AnimationUpdate(float elapsed);

    public partial class Ticker
    {
        private long duration;

        public long Duration
        {
            get => duration;
        }

        public Ticker()
        {

        }

        public event AnimationUpdate Update;

        public event Action Stop;

        public void OnUpdate(float elapsed)
        {
            Update?.Invoke(elapsed);
        }

        public void Cancel()
        {
            OnCancel();
            duration = 0;
        }

        public void OnStop()
        {
            Stop?.Invoke();
        }

        public void Start(long duration)
        {
            OnCancel();
            this.duration = duration;
            OnStart();
        }

        partial void OnStart();

        partial void OnCancel();
    }
}
