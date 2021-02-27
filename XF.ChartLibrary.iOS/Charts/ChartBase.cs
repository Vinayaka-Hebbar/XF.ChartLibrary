using XF.ChartLibrary.Animation;
using XF.ChartLibrary.Components;

namespace XF.ChartLibrary.Charts
{
    public abstract partial class ChartBase<TData, TDataSet> : UIKit.UIView
    {
        private IMarker marker;

        public XAxis XAxis { get; protected set; }

        public Legend Legend { get; protected set; }

        protected ChartBase()
        {
            XAxis = new XAxis();
        }

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

        public virtual void AnimatorStopped(Animator animator)
        {

        }

        public void AnimatorUpdated(Animator animator)
        {
            SetNeedsDisplay();
        }
    }
}
