﻿using System;
using System.ComponentModel;

namespace XF.ChartLibrary.Gestures
{
    public delegate void TapHandler(float x, float y);

    public delegate void PanHandler(PanEvent e, float distanceX, float distanceY);

    public delegate void PinchHandler(PinchEvent e, float x, float y);

    public delegate void DoubleTapHandler(float x, float y);

    public class PinchEvent
    {
        internal TouchState state;
        public TouchState State => state;
        public PinchState Mode;

        internal float xDist;
        internal float yDist;

#if __ANDROID__
        internal float Spacing; 
#endif

        public float Scale;

        /// <summary>
        /// Saved X dist
        /// </summary>
        public float XDist => xDist;

        /// <summary>
        /// Saved Y Dist
        /// </summary>
        public float YDist => yDist;

        public bool IsZooming => Mode != PinchState.None;

        internal void Reset()
        {
            Mode = PinchState.None;
            xDist = yDist = 0;
        }
    }

    public class PanEvent
    {
        internal TouchState state;
        public TouchState State => state;
        public PanState Mode;

        internal float x;

        internal float y;

        internal float velocityX;

        internal float velocityY;

        public float VelocityX => velocityX;

        public float VelocityY => velocityY;

        public float X => x;

        public float Y => y;

        internal void Reset()
        {
            Mode = PanState.None;
            velocityY = velocityX = 0;
        }
    }

    public enum TouchState { Started, Running, Completed }

    public partial class ChartGestureRecognizer : IChartGesture
    {
        public event TapHandler Tap;

        public event PanHandler Pan;

        public event DoubleTapHandler DoubleTap;

        public event PinchHandler Pinch;

        private event PropertyChangedEventHandler PropertyChanged;

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add
            {
                PropertyChanged += value;
            }

            remove
            {
                PropertyChanged -= value;
            }
        }

        public void OnTap(float x, float y)
        {
            Tap?.Invoke(x, y);
        }

        public void OnPan(PanEvent e, float distanceX, float distanceY)
        {
            Pan?.Invoke(e, distanceX, distanceY);
        }

        public void OnDoubleTap(float x, float y)
        {
            DoubleTap?.Invoke(x, y);
        }

        public void OnPinch(PinchEvent e, float x, float y)
        {
            Pinch?.Invoke(e, x, y);
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

#if NETSTANDARD
        public void Dispose()
        {
            Pinch = null;
            DoubleTap = null;
            Pan = null;
            Tap = null;
        } 
#endif
    }
}
