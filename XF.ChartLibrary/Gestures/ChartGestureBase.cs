using System.ComponentModel;

namespace XF.ChartLibrary.Gestures
{
    public abstract partial class ChartGestureBase : IChartGesture
    {
        protected bool Enabled;

        public bool TouchEnabled
        {
            get => Enabled;
            set
            {
                TouchEnabled = value;
                OnPropertyChanged(nameof(TouchEnabled));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Clear previous state when to view
        /// </summary>
        public virtual void Clear()
        {

        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
