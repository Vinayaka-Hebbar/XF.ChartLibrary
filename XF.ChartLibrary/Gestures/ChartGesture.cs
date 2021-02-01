using System.ComponentModel;

namespace XF.ChartLibrary.Gestures
{
    public delegate void TapHandler(TapEvent e);

    public delegate void PanHandler(Xamarin.Forms.GestureStatus state, float x, float y);

    public delegate void DoubleTapHandler(float x, float y);

    public partial class ChartGesture : Xamarin.Forms.IGestureRecognizer
    {
        public event TapHandler Tap;

        public event PanHandler Pan;

        public event DoubleTapHandler DoubleTap;

        public void OnTap(TapEvent e)
        {
            Tap?.Invoke(e);
        }

        public void OnPan(Xamarin.Forms.GestureStatus state, float x, float y)
        {
            Pan?.Invoke(state, x, y);
        }

        public void OnDoubleTap(float x, float y)
        {
            DoubleTap?.Invoke(x, y);
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class TapEvent
    {
        public long Id { get; set; }

        public TapAction Action { get; set; }

        public float X { get; set; }

        public float Y { get; set; }

        public int PointerCount { get; set; }

        public float XDist { get; set; }

        public float YDist { get; set; }
    }

    public class PanEvent
    {
        public Xamarin.Forms.GestureState State { get; set; }

        public float X { get; }

        public float Y { get; }
    }

    public enum TapAction
    {
        Entered,
        Pressed,
        Moved,
        Released,
        Cancelled,
        Exited,
        WheelChanged,
    }
}
