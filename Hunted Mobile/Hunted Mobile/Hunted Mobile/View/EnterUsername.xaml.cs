using Hunted_Mobile.ViewModel;

using System;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Hunted_Mobile.View {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EnterUsername : ContentPage {
        private readonly EnterUsernameViewModel EnterUsernameViewModel;

        public EnterUsername(MainPageViewModel inviteKeyViewModel) {
            InitializeComponent();
            BindingContext = this;

            EnterUsernameViewModel = new EnterUsernameViewModel();
            EnterUsernameViewModel.InviteKey = inviteKeyViewModel.InviteKeyModel;
        }

        private async void HandleJoinGame(object sender, EventArgs e) {
            ToggleButtonEnableState(false);

            EnterUsernameViewModel.UserName = this.UserNameField.Text;
            HandleErrorMessage();

            if(EnterUsernameViewModel.HasValidUserName) {
                await EnterUsernameViewModel.CreateUser();
            }

            if(EnterUsernameViewModel.CreatingUserSucceeded) {
                var previousPage = Navigation.NavigationStack.LastOrDefault();
                await Navigation.PushAsync(new Lobby(EnterUsernameViewModel), true);
                Navigation.RemovePage(previousPage);
            }

            ToggleButtonEnableState();
        }

        // Display or hide an errorMessage
        public void HandleErrorMessage() {
            this.UserNameMessage.Text = EnterUsernameViewModel.HasValidUserName ? "" : "Gebruikersnaam is verplicht en moet minimaal 4 karakters bevatten";
            OnPropertyChanged(nameof(this.UserNameMessage));
        }

        // Change the IsEnabled of SubmitButton
        public void ToggleButtonEnableState(bool enabled = true) {
            this.JoinGameButton.IsEnabled = enabled;

            OnPropertyChanged(nameof(this.JoinGameButton));
        }
    }
}
