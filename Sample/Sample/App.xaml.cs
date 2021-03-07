using System;
using Xamarin.Forms;

namespace Sample
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            RequestedThemeChanged += OnThemeChanged;
            NavigationPage root = new NavigationPage(new MainPage());
            root.Pushed += OnPushed;
            root.Popped += OnPoped;
            MainPage = root;
            AppDomain.CurrentDomain.UnhandledException += OnUnhandlexException;
            
        }

        private void OnPoped(object sender, NavigationEventArgs e)
        {
            if (e.Page is Pages.BasePage p)
            {
                RequestedThemeChanged -= p.ThemeChanged;
            }
        }

        void OnPushed(object sender, NavigationEventArgs e)
        {
            if(e.Page is Pages.BasePage p)
            {
                RequestedThemeChanged += p.ThemeChanged;
            }
        }

        private void OnThemeChanged(object sender, AppThemeChangedEventArgs e)
        {
            UserAppTheme = e.RequestedTheme;
        }

        private void OnUnhandlexException(object sender, UnhandledExceptionEventArgs e)
        {
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
