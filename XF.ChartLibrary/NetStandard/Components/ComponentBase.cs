using SkiaSharp;

namespace XF.ChartLibrary.Components
{
    public partial class ComponentBase
    {
        private float textSize;

        public float TextSize
        {
            get => textSize;
            set => textSize = value;
        }

        public SKColor TextColor { get; set; } = SKColors.Black;

        public SKTypeface Typeface { get; set; }
    }
}
