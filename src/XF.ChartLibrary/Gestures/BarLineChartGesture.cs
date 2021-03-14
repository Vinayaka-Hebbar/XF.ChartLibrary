namespace XF.ChartLibrary.Gestures
{
    public delegate void TapHandler(TapEvent e);

    public delegate void PanHandler(PanEvent e, float distanceX, float distanceY);

    public delegate void PinchHandler(PinchEvent e, float x, float y);

    public delegate void DoubleTapHandler(float x, float y);

    public enum TouchState { Begin, Changed, Ended }

    public partial class BarLineChartGesture : ChartGestureBase
    {
        private TapHandler tap;
        public TapHandler Tap
        {
            get => tap;
            set => tap = value;
        }

        private PanHandler pan;
        public PanHandler Pan
        {
            get => pan;
            set => pan = value;
        }

        private DoubleTapHandler doubleTap;
        public DoubleTapHandler DoubleTap
        {
            get => doubleTap;
            set => doubleTap = value;
        }

        private PinchHandler pinch;
        public PinchHandler Pinch
        {
            get => pinch;
            set => pinch = value;
        }

        public void OnTap(TapEvent e)
        {
            tap?.Invoke(e);
        }

        public void OnPan(PanEvent e, float distanceX, float distanceY)
        {
            pan?.Invoke(e, distanceX, distanceY);
        }

        public void OnDoubleTap(float x, float y)
        {
            doubleTap?.Invoke(x, y);
        }

        public void OnPinch(PinchEvent e, float x, float y)
        {
            pinch?.Invoke(e, x, y);
        }
    }
}
