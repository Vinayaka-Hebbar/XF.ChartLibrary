using CoreAnimation;
using System;

namespace XF.ChartLibrary.Animation
{
    partial class Ticker
    {
        private long startTime;
        private long endTime;
        private CADisplayLink displayLink;

        partial void OnCancel()
        {
            if (displayLink != null)
            {
                displayLink.RemoveFromRunLoop(Foundation.NSRunLoop.Main, Foundation.NSRunLoopMode.Common);
                displayLink = null;
                OnStop();
            }
        }

        partial void OnStart()
        {
            startTime = Environment.TickCount;
            endTime = startTime + endTime;
            // Take care of the first frame if rendering is already scheduled...
            OnUpdate(0);
            displayLink = CADisplayLink.Create(UpdateAnimation);
            displayLink.AddToRunLoop(Foundation.NSRunLoop.Main, Foundation.NSRunLoopMode.Common);
        }

        void UpdateAnimation()
        {
            int current = Environment.TickCount;
            var elapsed = current - startTime;
            if (current >= endTime)
            {
                Cancel();
                return;
            }
            if (elapsed > duration)
            {
                elapsed = duration;
            }
            OnUpdate(elapsed);
        }
    }
}
