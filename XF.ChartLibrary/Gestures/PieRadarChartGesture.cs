namespace XF.ChartLibrary.Gestures
{
    public delegate void TouchHandler(TouchEvent e);

    public partial class PieRadarChartGesture : ChartGestureBase
    {
        public event TapHandler Tap;

        public event TouchHandler Touch;

        public void OnTap(TapEvent e)
        {
            Tap?.Invoke(e);
        }

        public void OnTouch(TouchEvent e)
        {
            Touch?.Invoke(e);
        }
    }
}
