using Hunted_Mobile.Model;
using Hunted_Mobile.ViewModel;

using Xamarin.Forms;

namespace Hunted_Mobile.View {
    public partial class MainPage : ContentPage {
        private readonly MainPageViewModel mainPageViewModel;
        public MainPage() {
            InitializeComponent();
            BindingContext = mainPageViewModel = new MainPageViewModel(this);
        }

        // This method is called when rendering this page, because the InviteKey should be reset
        protected override void OnAppearing() {
            mainPageViewModel.InviteKeyModel = new InviteKey();
            base.OnAppearing();
        }
    }
}
