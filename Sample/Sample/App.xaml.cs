using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Sample
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();

            AppDomain.CurrentDomain.UnhandledException += OnUnhandlexException;
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
