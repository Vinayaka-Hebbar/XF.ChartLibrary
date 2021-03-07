using SkiaSharp;

namespace XF.ChartLibrary.Components
{
    public partial class ComponentBase
    {
        private float textSize;

        protected ComponentBase()
        {
            textSize = 10f.DpToPixel();
            xOffset = yOffset = 5f.DpToPixel();
        }

        public float TextSize
        {
            get => textSize;
            set
            {
                value = value.DpToPixel();
                if (value > 24f)
                    value = 24f;
                if (value < 6f)
                    value = 6f;
                textSize = value;
            }
        }

        public SKColor TextColor { get; set; } = SKColors.Black;

        public SKTypeface Typeface { get; set; }
    }
}
