namespace XF.ChartLibrary.Gestures
{
    public enum GestureMode
    {
        None, Drag, XZoom, YZoom, PinchZoom, Rotate, SingleTap, DoubleTap, LongPress, Fling
    }

    public enum PinchState
    {
        None,
        PinchZoom,
        XZoom,
        YZoom
    }

    public enum PanState
    {
        None, 
        Drag
    }
}
