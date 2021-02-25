using System.ComponentModel;
using Xamarin.Forms;
using XF.ChartLibrary.Example.ViewModels;

namespace XF.ChartLibrary.Example.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}