using Xamarin.Forms;

namespace XF.ChartLibrary.Jobs
{
    public partial class ViewPortJob : BindableObject
    {
        public void DoJob()
        {
            Dispatcher.BeginInvokeOnMainThread(Run);
        }
    }
}
