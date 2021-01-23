using Android.Content;
using Android.Views;

namespace XF.ChartLibrary.Charts
{
    public abstract partial class ChartBase<TData, TDataSet> : View
    {
        protected ChartBase(Context context) : base(context)
        {
        }

    }
}
