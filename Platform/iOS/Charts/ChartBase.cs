using XF.ChartLibrary.Animation;

namespace XF.ChartLibrary.Charts
{
    public abstract partial class ChartBase<TData, TDataSet> : UIKit.UIView
    {
        public virtual void AnimatorStopped(Animator animator)
        {

        }

        public void AnimatorUpdated(Animator animator)
        {
            SetNeedsDisplay();
        }
    }
}
