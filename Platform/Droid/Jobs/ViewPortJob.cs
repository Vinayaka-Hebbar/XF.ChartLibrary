using System;
using System.Collections.Generic;
using System.Text;

namespace XF.ChartLibrary.Jobs
{
    public partial class ViewPortJob : Java.Lang.Object, Java.Lang.IRunnable
    {
        public void DoJob()
        {
            View.Post(this);
        }
    }
}
