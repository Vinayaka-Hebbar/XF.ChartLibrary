namespace XF.ChartLibrary.Gestures
{
    public interface IChartGesture : Xamarin.Forms.IGestureRecognizer
#if __ANDROID__
       , Android.Views.View.IOnTouchListener
#endif
    {
        bool TouchEnabled { get; set; }

        void Clear();
#if __ANDROID__
        void OnInitialize(Android.Views.View view);
#elif __IOS__ || __TVOS__
        void OnInitialize(UIKit.UIView view, System.nfloat scale);
        void SetScale(System.nfloat scale);
        void Attach(UIKit.UIView view);
        void Detach(UIKit.UIView view);
        System.nfloat ScaleFactor {get;}
#endif
    }
}
