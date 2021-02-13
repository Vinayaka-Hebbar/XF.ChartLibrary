namespace XF.ChartLibrary.Jobs
{
    public partial class ViewPortJob : Java.Lang.Object, Java.Lang.IRunnable
    {
        protected readonly float[] Points = new float[2];

        public void DoJob()
        {
            View.Post(this);
        }
    }
}
