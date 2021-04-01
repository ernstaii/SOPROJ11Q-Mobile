using Hunted_Mobile.Model;
using Hunted_Mobile.Model.GameModels;
using Hunted_Mobile.Repository;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Hunted_Mobile.View {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EnterUsername : ContentPage {
        public bool isValid = false;
        public InviteKey inviteKey = null;
        public User user = null;
        public UserRepository _userRepository = new UserRepository();

        public EnterUsername(InviteKey inviteKey) {
            InitializeComponent();
            BindingContext = this;

            this.inviteKey = inviteKey;
        }

        private async void HandleJoinGame(object sender, EventArgs e) {
            EnableButton(false);

            await this.Validate();

            if(isValid) {
                var previousPage = Navigation.NavigationStack.LastOrDefault();
                await Navigation.PushAsync(new Lobby(this.inviteKey, this.user), true);
                Navigation.RemovePage(previousPage);
            }

            EnableButton(true);
        }

        // Check if InviteCode is valid
        public async Task Validate() {
            if(this.UserNameField.Text != null && this.UserNameField.Text.Length >= 4) {

                // Creating a user with the values
                this.user = await _userRepository.Create(this.inviteKey.Value, this.UserNameField.Text);
            }

            isValid = this.user != null;

            // Display ErrorMessage
            this.UserNameMessage.Text = isValid ? "" : "Gebruikersnaam is verplicht en moet minimaal 4 karakters bevatten";
            OnPropertyChanged(nameof(this.UserNameMessage));
        }

        // Get game based on InviteCode
        // TODO: Join a Game, maybe create user
        public void JoinGame() {
            //return new Game();
        }

        // Change the IsEnabled of SubmitButton
        public void EnableButton(bool enabled = true) {
            this.JoinGameButton.IsEnabled = enabled;

            OnPropertyChanged(nameof(this.JoinGameButton));
        }
    }
}
