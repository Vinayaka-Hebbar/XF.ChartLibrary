﻿using CoreAnimation;
using System;
using System.Collections.Generic;
using System.Text;
#if __IOS__ || __TVOS__
using UIKit;
using NSUIDisplayLink = CoreAnimation.CADisplayLink;
#endif

namespace XF.ChartLibrary.Animation
{
    public partial class Animator
    {
        private long startTimeX;

        private long startTimeY;

        private NSUIDisplayLink displayLink;

        private long durationX;
        private long durationY;

        private double endTimeX;
        private double endTimeY;
        private double endTime;

        private bool enabledX;
        private bool enabledY;

        private EasingFunction _easingX;
        private EasingFunction _easingY;

        public Action UpdateBlock { get; set; }

        public Action StopBlock { get; set; }

        public void Stop()
        {
            if (displayLink != null)
            {
                displayLink.RemoveFromRunLoop(Foundation.NSRunLoop.Main, Foundation.NSRunLoopMode.Common);
                displayLink = null;

                enabledX = false;
                enabledY = false;

                // If we stopped an animation in the middle, we do not want to leave it like this
                if (PhaseX != 1.0 || PhaseY != 1.0)
                {
                    PhaseX = 1.0f;
                    PhaseX = 1.0f;

                    Delegate.AnimatorUpdated(this);
                    UpdateBlock?.Invoke();
                }

                Delegate.AnimatorStopped(this);
                StopBlock?.Invoke();
            }
        }

        void UpdateAnimationPhases(long currentTime)
        {
            if (enabledX)
            {
                var elapsedTime = currentTime - startTimeX;
                var duration = durationX;
                var elapsed = elapsedTime;
                if (elapsed > duration)
                {
                    elapsed = duration;
                }

                PhaseX = _easingX == null ? elapsed / duration : _easingX.Invoke(elapsed / duration);
            }

            if (enabledY)
            {
                var elapsedTime = currentTime - startTimeY;
                var duration = durationY;
                var elapsed = elapsedTime;
                if (elapsed > duration)
                {
                    elapsed = duration;
                }

                PhaseY = _easingY == null ? elapsed / duration : _easingY.Invoke(elapsed / duration);
            }
        }

        private void AnimationLoop()
        {
            var currentTime = System.Diagnostics.Stopwatch.GetTimestamp();

            UpdateAnimationPhases(currentTime);

            Delegate.AnimatorUpdated(this);
            UpdateBlock?.Invoke();


            if (currentTime >= endTime)
            {
                Stop();
            }

        }

        public void Animate(long xAxisDuration, long yAxisDuration, EasingFunction easingX, EasingFunction easingY)
        {
            Stop();

            startTimeX = System.Diagnostics.Stopwatch.GetTimestamp();
            startTimeY = startTimeX;
            durationX = xAxisDuration;
            durationY = yAxisDuration;
            endTimeX = startTimeX + xAxisDuration;
            endTimeY = startTimeY + yAxisDuration;
            endTime = endTimeX > endTimeY ? endTimeX : endTimeY;
            enabledX = xAxisDuration > 0.0;
            enabledY = yAxisDuration > 0.0;


            _easingX = easingX;
            _easingY = easingY;

            // Take care of the first frame if rendering is already scheduled...
            UpdateAnimationPhases(startTimeX);


            if (enabledX || enabledY)
            {
                displayLink = NSUIDisplayLink.Create(AnimationLoop);
                displayLink.AddToRunLoop(Foundation.NSRunLoop.Main, Foundation.NSRunLoopMode.Common);
            }
        }

        public void AnimateX(long xAxisDuration, EasingFunction easing)
        {
            startTimeX = System.Diagnostics.Stopwatch.GetTimestamp();
            durationX = xAxisDuration;
            endTimeX = startTimeX + xAxisDuration;
            endTime = endTimeX > endTimeY ? endTimeX : endTimeY;
            enabledX = xAxisDuration > 0.0;


            _easingX = easing;

            // Take care of the first frame if rendering is already scheduled...
            UpdateAnimationPhases(startTimeX);


            if ((enabledX || enabledY) &&
                displayLink == null)
            {
                displayLink = NSUIDisplayLink.Create(AnimationLoop);
                displayLink.AddToRunLoop(Foundation.NSRunLoop.Main, Foundation.NSRunLoopMode.Common);
            }
        }

        public void AnimateY(long yAxisDuration, EasingFunction easing)
        {
            startTimeY = System.Diagnostics.Stopwatch.GetTimestamp();
            durationY = yAxisDuration;
            endTimeY = startTimeY + yAxisDuration;
            endTime = endTimeX > endTimeY ? endTimeX : endTimeY;
            enabledY = yAxisDuration > 0.0;


            _easingY = easing;

            // Take care of the first frame if rendering is already scheduled...
            UpdateAnimationPhases(startTimeY);


            if ((enabledX || enabledY) &&
                    displayLink == null)
            {
                displayLink = NSUIDisplayLink.Create(AnimationLoop);
                displayLink.AddToRunLoop(Foundation.NSRunLoop.Main, Foundation.NSRunLoopMode.Common);
            }
        }
    }
}
