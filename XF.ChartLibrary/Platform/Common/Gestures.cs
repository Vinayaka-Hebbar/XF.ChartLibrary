namespace XF.ChartLibrary.Gestures
{

    public class PinchEvent
    {
        internal TouchState state;
        public TouchState State => state;

        internal PinchMode mode;
        public PinchMode Mode
        {
            get => mode;
            set => mode = value;
        }

        internal float yDist;

#if __ANDROID__
        internal float Spacing; 
#endif

        public float Scale;

        internal float xDist;
        /// <summary>
        /// Saved X dist
        /// </summary>
        public float XDist => xDist;

        internal void Reset()
        {
            Mode = PinchMode.None;
            xDist = yDist = 0;
        }

        /// <summary>
        /// Saved Y Dist
        /// </summary>
        public float YDist => yDist;

        public bool IsZooming => Mode != PinchMode.None;
    }

    public class PanEvent
    {
        internal PanMode mode;
        public PanMode Mode
        {
            get => mode;
            set => mode = value;
        }

        internal TouchState state;
        public TouchState State => state;

        internal float x;
        public float X => x;

        internal float y;
        public float Y => y;

        internal float velocityX;

        internal float velocityY;

        public float VelocityX => velocityX;

        public float VelocityY => velocityY;

        internal void Reset()
        {
            Mode = PanMode.None;
            velocityY = velocityX = 0;
        }
    }

    public struct TapEvent
    {
        internal TouchState state;
        public TouchState State => state;

        internal float x;
        public float X => x;

        internal float y;
        public float Y => y;
    }


    public class TouchEvent
    {
        internal float x;
        internal float y;

        internal GestureMode mode;
        public GestureMode Mode
        {
            get => mode;
            set => mode = value;
        }

        internal TouchState state;

        public TouchState State => state;

        public float X => x;
        public float Y => y;
    }



    public class RotateEvent
    {
        internal float rotate;

        public float Rotate
        {
            get { return rotate; }
        }

        internal RotateMode mode;
        public RotateMode Mode
        {
            get => mode;
            set => mode = value;
        }

        internal float velocity;

        public float Velocity => velocity;

        public void Reset()
        {
            rotate = velocity = 0f;
        }

    }
}
