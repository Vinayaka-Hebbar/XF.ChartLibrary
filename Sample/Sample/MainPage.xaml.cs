using Xamarin.Forms;

namespace Sample
{
    public partial class MainPage 
    {
        public MainPage()
        {
            BindingContext = new ViewModels.MainViewModel();
            InitializeComponent();
        }

        async void OnItemTap(object sender, System.EventArgs e)
        {
            var obj = (BindableObject)sender;
            if (obj.BindingContext is Models.ChartType item)
            {
                await Navigation.PushAsync(item.Activator());
            }
        }
    }
}
