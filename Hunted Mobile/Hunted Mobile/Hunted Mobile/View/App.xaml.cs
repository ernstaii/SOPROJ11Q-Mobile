using Hunted_Mobile.View;
using Hunted_Mobile.ViewModel;

using Xamarin.Forms;

namespace Hunted_Mobile {
    public partial class App : Application {
        public App(AppViewModel viewModel) {
            InitializeComponent();

            BindingContext = viewModel;

            MainPage = new NavigationPage(new MainPage(viewModel));
        }

        protected override void OnStart() {
        }

        protected override void OnSleep() {
        }

        protected override void OnResume() {
        }
    }
}
