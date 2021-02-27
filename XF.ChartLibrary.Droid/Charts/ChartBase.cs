using Android.Animation;
using Android.Content;
using Android.Views;
using XF.ChartLibrary.Components;

namespace XF.ChartLibrary.Charts
{
    public abstract partial class ChartBase<TData, TDataSet> : View, ValueAnimator.IAnimatorUpdateListener
    {
        private IMarker marker;

        protected ChartBase(Context context) : base(context)
        {
            XAxis = new XAxis();
        }

        public Legend Legend { get; protected set; }

        public XAxis XAxis { get; protected set; }

        public IMarker Marker
        {
            get => marker;
            set => marker = value;
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
