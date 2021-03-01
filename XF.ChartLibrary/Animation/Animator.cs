using System;
using Xamarin.Forms;

namespace XF.ChartLibrary.Animation
{
    public partial class Animator 
    {
        private float durationX;
        private float durationY;

        private bool enabledX;
        private bool enabledY;

        private EasingFunction _easingX;
        private EasingFunction _easingY;

        public Action UpdateBlock { get; set; }

        public Action StopBlock { get; set; }

        private readonly Ticker ticker;

        public Animator()
        {
            ticker = new Ticker();
            ticker.Update += OnUpdate;
            ticker.Stop += OnStop;
        }

        void OnUpdate(float elapsed)
        {
            if (enabledX)
            {
                float duration = durationX;
                if (elapsed > duration)
                {
                    elapsed = duration;
                }

                PhaseX = _easingX == null ? elapsed / duration : _easingX.Invoke(elapsed / duration);
            }
            if (enabledY)
            {
                float duration = durationY;
                if (elapsed > duration)
                {
                    elapsed = duration;
                }
                PhaseY = _easingY == null ? elapsed / duration : _easingY.Invoke(elapsed / duration);
            }

            Delegate.AnimatorUpdated(this);
            UpdateBlock?.Invoke();
        }

        void OnStop()
        {
            enabledX = false;
            enabledY = false;

            // If we stopped an animation in the middle, we do not want to leave it like this
            if (PhaseX != 1.0f || PhaseY != 1.0f)
            {
                PhaseX = 1.0f;
                PhaseX = 1.0f;

                Delegate.AnimatorUpdated(this);
                UpdateBlock?.Invoke();
            }

            Delegate.AnimatorStopped(this);
            StopBlock?.Invoke();
        }

        public void Stop()
        {
            ticker.Cancel();
        }

        public void Animate(long xAxisDuration, long yAxisDuration, EasingFunction easingX, EasingFunction easingY)
        {
            ticker.Cancel();

            durationX = xAxisDuration;
            durationY = yAxisDuration;
            enabledX = xAxisDuration > 0.0;
            enabledY = yAxisDuration > 0.0;


            _easingX = easingX;
            _easingY = easingY;

            if (enabledX || enabledY)
            {
                ticker.Start(xAxisDuration > yAxisDuration ? xAxisDuration : yAxisDuration);
            }
        }

        public void AnimateX(long xAxisDuration, EasingFunction easing)
        {
            ticker.Cancel();

            durationX = xAxisDuration;
            enabledX = xAxisDuration > 0.0;


            _easingX = easing;

            if ((enabledX || enabledY))
            {
                ticker.Start(xAxisDuration);
            }
        }

        public void AnimateY(long yAxisDuration, EasingFunction easing)
        {
            ticker.Cancel();

            durationY = yAxisDuration;
            enabledY = yAxisDuration > 0.0;

            _easingY = easing;

            if (enabledX || enabledY)
            {
                ticker.Start(yAxisDuration);
            }
        }
    }
}
