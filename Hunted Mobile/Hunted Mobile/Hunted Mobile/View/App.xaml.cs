using Hunted_Mobile.View;

using Xamarin.Forms;

namespace Hunted_Mobile {
    public partial class App : Application {
        public App() {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnStart() {
        }

        protected override void OnSleep() {
        }

        protected override void OnResume() {
        }
    }
}
