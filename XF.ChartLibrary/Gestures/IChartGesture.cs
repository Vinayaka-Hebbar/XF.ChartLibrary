namespace XF.ChartLibrary.Gestures
{
    public interface IChartGesture : Xamarin.Forms.IGestureRecognizer, System.IDisposable
    {
#if __ANDROID__
        void OnInitialize(Android.Views.View view);
        bool OnTouch(Android.Views.View view, Android.Views.MotionEvent e);
#elif __IOS__ || __TVOS__
        void OnInitialize(UIKit.UIView view);
#endif
    }
}
