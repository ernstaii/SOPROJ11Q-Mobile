
using Xamarin.Forms;
using Hunted_Mobile.ViewModel;

namespace Hunted_Mobile.View {
    public partial class MainPage : ContentPage {
        private readonly MainPageViewModel mainPageViewModel;
        public MainPage() {
            this.InitializeComponent();
            BindingContext = mainPageViewModel = new MainPageViewModel(this);
        }

        // This method is called when rendering this page, because the InviteKey should be reset
        protected override void OnAppearing() {
            mainPageViewModel.ResetInviteKey();
            base.OnAppearing();
        }
    }
}
