namespace XF.ChartLibrary.Gestures
{
    public delegate void TapHandler(TapEvent e);

    public delegate void PanHandler(PanEvent e, float distanceX, float distanceY);

    public delegate void PinchHandler(PinchEvent e, float x, float y);

    public delegate void DoubleTapHandler(float x, float y);

    public enum TouchState { Begin, Changed, Ended }

    public partial class BarLineChartGesture : ChartGestureBase
    {
        public event TapHandler Tap;

        public event PanHandler Pan;

        public event DoubleTapHandler DoubleTap;

        public event PinchHandler Pinch;

        public void OnTap(TapEvent e)
        {
            Tap?.Invoke(e);
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
    }
}
