using System;
using System.ComponentModel;

namespace XF.ChartLibrary.Gestures
{
    public delegate void TapHandler(TapEvent e);

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

    public class TapEvent
    {
        internal float x;

        internal float y;

        public float X => x;

        public float Y => y;
    }

    public enum TouchState { Started, Running, Completed }

    public partial class ChartGestureRecognizer : Xamarin.Forms.IGestureRecognizer
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

        protected void OnTap(TapEvent e)
        {
            Tap?.Invoke(e);
        }

        protected void OnPan(PanEvent e, float distanceX, float distanceY)
        {
            Pan?.Invoke(e, distanceX, distanceY);
        }

        protected void OnDoubleTap(float x, float y)
        {
            DoubleTap?.Invoke(x, y);
        }

        protected void OnPinch(PinchEvent e, float x, float y)
        {
            Pinch?.Invoke(e, x, y);
        }
    }
}
