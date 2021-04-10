using Hunted_Mobile.ViewModel;

using System;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Hunted_Mobile.View {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EnterUsername : ContentPage {
        private readonly UserViewModel UserViewModel;

        public EnterUsername(InviteKeyViewModel inviteKeyViewModel) {
            InitializeComponent();
            BindingContext = this;

            UserViewModel = new UserViewModel();
            UserViewModel.InviteKey = inviteKeyViewModel.model;
        }

        private async void HandleJoinGame(object sender, EventArgs e) {
            ToggleButtonEnableState(false);

            this.UserViewModel.UserName = this.UserNameField.Text;
            HandleErrorMessage();

            if(this.UserViewModel.HasValidUserName) {
                await this.UserViewModel.CreateUser();
            }

            if(this.UserViewModel.CreatingUserSucceeded) {
                var previousPage = Navigation.NavigationStack.LastOrDefault();
                await Navigation.PushAsync(new Lobby(this.UserViewModel), true);
                Navigation.RemovePage(previousPage);
            }

            ToggleButtonEnableState();
        }

        // Display or hide an errorMessage
        public void HandleErrorMessage() {
            this.UserNameMessage.Text = this.UserViewModel.HasValidUserName ? "" : "Gebruikersnaam is verplicht en moet minimaal 4 karakters bevatten";
            OnPropertyChanged(nameof(this.UserNameMessage));
        }

        // Change the IsEnabled of SubmitButton
        public void ToggleButtonEnableState(bool enabled = true) {
            this.JoinGameButton.IsEnabled = enabled;

            OnPropertyChanged(nameof(this.JoinGameButton));
        }
    }
}
