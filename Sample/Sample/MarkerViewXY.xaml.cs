using Xamarin.Forms.Xaml;
using XF.ChartLibrary.Data;
using XF.ChartLibrary.Highlight;

namespace Sample
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MarkerViewXY
    {
        public MarkerViewXY()
        {
            InitializeComponent();
        }

        public override void RefreshContent(Entry e, Highlight highlight)
        {
            Field.Text = $"x: {e.X}, y: {e.Y}";
            base.RefreshContent(e, highlight);
        }
    }
}