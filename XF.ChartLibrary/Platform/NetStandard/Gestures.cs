namespace XF.ChartLibrary.Gestures
{
    public class PanEvent
    {
        public PanMode Mode { get => throw null; set { } }

        public TouchState State => throw null;

        public float X => throw null;

        public float Y => throw null;

        public float VelocityX => throw null;

        public float VelocityY => throw null;
    }

    public class PinchEvent
    {
        public TouchState State => throw null;

        public PinchMode Mode { get => throw null; set { } }

        public float Scale;
        /// <summary>
        /// Saved X dist
        /// </summary>
        public float XDist => throw null;

        /// <summary>
        /// Saved Y Dist
        /// </summary>
        public float YDist => throw null;

        public bool IsZooming => Mode != PinchMode.None;
    }

    public struct TapEvent
    {
        public TouchState State => throw null;

        public float X => throw null;

        public float Y => throw null;
    }

    public class TouchEvent
    {
        public GestureMode Mode { get => throw null; set { } }

        public TouchState State => throw null;


        public float X => throw null;

        public float Y => throw null;
    }

    public class RotateEvent
    {
        public float Rotate => throw null;

        public RotateMode Mode { get => throw null; set { } }

        public float Velocity => throw null;

    }
}
