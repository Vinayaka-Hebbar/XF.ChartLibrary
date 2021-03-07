using SkiaSharp;
using Xamarin.Forms;

namespace XF.ChartLibrary.Components
{
    public class Span
    {
        private SKColor background;
        private string text;
        private SKColor foreground;
        private float textSize;
        private float lineHeight;
        private string fontName;
        private FontAttributes attributes;

        public Span()
        {
        }

        public Span(string text)
        {
            this.text = text;
        }

        public Span(Span other)
        {
            text = other.Text;
            textSize = other.TextSize;
            foreground = other.Foreground;
            lineHeight = other.LineHeight;
            attributes = other.Attributes;
            background = other.Background;
        }

        public string Text
        {
            get => text;
            set => text = value;
        }

        public SKColor Background
        {
            get => background;
            set => background = value;
        }

        public SKColor Foreground
        {
            get => foreground;
            set => foreground = value;
        }

        public float TextSize
        {
            get => textSize;
            set => textSize = value;
        }

        public float LineHeight
        {
            get => lineHeight;
            set => lineHeight = value;
        }

        public string FontName
        {
            get => fontName;
            set => fontName = value;
        }

        public int Line { get; set; }

        public SKRect Bounds { get; set; }

        public SKRect LayoutFrame { get; set; }

        public SKTypeface Typeface { get; set; }

        public FontAttributes Attributes
        {
            get => attributes;
            set => attributes = value;
        }
    }
}
