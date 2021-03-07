using Xamarin.Forms.Xaml;
using XF.ChartLibrary.Data;
using XF.ChartLibrary.Formatter;
using XF.ChartLibrary.Highlight;

namespace Sample
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MarkerViewXY
    {
        private readonly IAxisValueFormatter xValueFormatter;

        public MarkerViewXY()
        {
            InitializeComponent();
        }

        public MarkerViewXY(IAxisValueFormatter xValueFormatter)
        {
            this.xValueFormatter = xValueFormatter;
            InitializeComponent();
        }

        public override void RefreshContent(Entry e, Highlight highlight)
        {
            if(xValueFormatter == null)
            {
                Field.Text = $"x: {e.X}, y: {e.Y}";
            }
            else
            {
                Field.Text = $"x: {xValueFormatter.GetFormattedValue(e.X, null)}, y: {e.Y}";
            }
            base.RefreshContent(e, highlight);
        }
    }
}