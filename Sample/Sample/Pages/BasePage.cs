using Xamarin.Forms;

namespace Sample.Pages
{
    public class BasePage : ContentPage
    {
        internal void ThemeChanged(object sender, AppThemeChangedEventArgs e)
        {
            OnThemeChanged(e.RequestedTheme);
        }

        protected virtual void OnThemeChanged(OSAppTheme theme)
        {
        }
    }
}
