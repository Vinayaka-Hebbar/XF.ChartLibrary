using SkiaSharp;
using Xamarin.Forms;
using XF.ChartLibrary.Charts;
using XF.ChartLibrary.Components;

namespace Sample.Custom
{
    public class MarkerViewText : MarkerView
    {
        private readonly FormattedString text;

        public MarkerViewText()
        {
            text = new FormattedString()
            {
                Spans =
                {
                    new Xamarin.Forms.Span
                    {
                        Text = "Sample"
                    },
                    new Xamarin.Forms.Span
                    {
                        Text = " Text",
                        FontAttributes = FontAttributes.Bold,
                        TextColor = Color.Red
                    }
                }
            };
        }

        public override void Draw(SKCanvas canvas, SKPoint pos, IChartBase chart)
        {
            new TextLayout().Draw(canvas, text, SKRect.Create(pos.X, pos.Y, 250, 50));
            base.Draw(canvas, pos, chart);
        }
    }
}
