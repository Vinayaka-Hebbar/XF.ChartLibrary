using Android.Graphics;

namespace XF.ChartLibrary.Components
{
    public abstract partial class ComponentBase
    {
        private float textSize;

        protected ComponentBase(float textSize)
        {
            this.textSize = textSize;
        }

        protected ComponentBase()
        {
            textSize = 10f.DpToPixel();
        }

        public float TextSize
        {
            get => textSize;
            set
            {
                if (value > 24f)
                    value = 24f;
                if (value < 6f)
                    value = 6f;
                textSize = value.DpToPixel();
            }
        }

        public Color TextColor { get; set; } = Color.Black;

        public Typeface Typeface { get; set; }
    }
}
