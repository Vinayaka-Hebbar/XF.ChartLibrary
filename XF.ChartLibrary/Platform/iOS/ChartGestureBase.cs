using System;
using UIKit;

namespace XF.ChartLibrary.Gestures
{
    partial class ChartGestureBase : Foundation.NSObject
    {
        protected internal nfloat Scale;

        public nfloat ScaleFactor => Scale;

        protected internal UIView View;

        public virtual void OnInitialize(UIView view, nfloat scale)
        {
            View = view;
            Scale = scale;
            Attach(view);
        }

        public abstract void Attach(UIView view);

        public abstract void Detach(UIView view);

        public void SetScale(nfloat scale) => Scale = scale;

    }
}
