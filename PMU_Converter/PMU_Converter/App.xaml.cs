using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PMU_Converter
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
            var viewModel = MainPage.BindingContext as MainViewModel;
            viewModel?.LoadState();
        }

        protected override void OnSleep()
        {
            var viewModel = MainPage.BindingContext as MainViewModel;
            viewModel?.SaveState();
        }

        protected override void OnResume()
        {
        }
    }
}
