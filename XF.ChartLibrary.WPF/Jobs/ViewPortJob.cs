using System.Windows;

namespace XF.ChartLibrary.Jobs
{
    public partial class ViewPortJob : DependencyObject
    {
        public void DoJob()
        {
            Dispatcher.Invoke(Run);
        }
    }
}
