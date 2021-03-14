namespace XF.ChartLibrary.Gestures
{
    public delegate void TouchHandler(TouchEvent e);

    public partial class PieRadarChartGesture : ChartGestureBase
    {
        private TapHandler tap;
        public TapHandler Tap
        {
            get => tap;
            set => tap = value;
        }

        private TouchHandler touch;
        public TouchHandler Touch
        {
            get => touch;
            set => touch = value;
        }

        public void OnTap(TapEvent e)
        {
            tap?.Invoke(e);
        }

        public void OnTouch(TouchEvent e)
        {
            touch?.Invoke(e);
        }
    }
}
