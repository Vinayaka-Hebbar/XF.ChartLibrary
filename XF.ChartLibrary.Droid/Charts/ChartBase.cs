using Android.Animation;
using Android.Content;
using Android.Views;
using XF.ChartLibrary.Components;

namespace XF.ChartLibrary.Charts
{
    public abstract partial class ChartBase<TData, TDataSet> : View, Android.Animation.ValueAnimator.IAnimatorUpdateListener
    {
        protected ChartBase(Context context) : base(context)
        {
            XAxis = new XAxis();
        }

        public TData Data
        {
            get => data;
            set
            {
                data = value;
                offsetsCalculated = false;
                if (value == null)
                    return;
                SetUpDefaultFormatter(value.YMin, value.YMax);
                foreach (TDataSet set in value.DataSets)
                {
                    if (set.NeedsFormatter || set.ValueFormatter == DefaultValueFormatter)
                        set.ValueFormatter = DefaultValueFormatter;
                }
                NotifyDataSetChanged();
            }
        }

        public void OnAnimationUpdate(ValueAnimator animation)
        {
            PostInvalidate();
        }
    }
}
