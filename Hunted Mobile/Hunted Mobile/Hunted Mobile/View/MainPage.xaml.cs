using Xamarin.Forms;
using Hunted_Mobile.ViewModel;

namespace Hunted_Mobile.View {
    public partial class MainPage : ContentPage {
        private readonly MainPageViewModel mainPageViewModel;
        public MainPage(AppViewModel appViewModel) {
            this.InitializeComponent();
            BindingContext = mainPageViewModel = new MainPageViewModel(this, appViewModel);
        }

        // This method is called when rendering this page, because the InviteKey should be reset
        protected override void OnAppearing() {
            mainPageViewModel.ResetInviteKey();
            mainPageViewModel.LoadPreviousGame();
            mainPageViewModel.ShowInformationPage();

            base.OnAppearing();
        }
    }
}
