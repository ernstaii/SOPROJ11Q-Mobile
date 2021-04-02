using Hunted_Mobile.Model;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hunted_Mobile.Repository;

using Xamarin.Forms;

namespace Hunted_Mobile.View {
    public partial class MainPage : ContentPage {
        public bool isValid = false;
        public InviteKey inviteKey = null;

        private InviteKeyRepository _inviteKeyRepository = new InviteKeyRepository();

        public MainPage() {
            this.InitializeComponent();
            BindingContext = this;
        }

        // If InviteCode is valid, then user will be redirected to screen for entering an username
        private async void SubmitInviteCode(object sender, EventArgs e) {
            // Disable the button, so user doesn't click the button twice for loading the game
            EnableButton(false);

            await this.Validate();

            if(isValid) {
                await Navigation.PushAsync(new EnterUsername(this.inviteKey), true);
            }

            EnableButton(true);
        }

        // Check if InviteCode is valid
        public async Task Validate() {
            if(this.InviteCodeField.Text != null)
                this.inviteKey = await _inviteKeyRepository.Get(this.InviteCodeField.Text);

            isValid = this.inviteKey != null;

            // Display ErrorMessage
            this.InviteCodeMessage.Text = isValid ? "" : "De opgegeven code is ongeldig";
            OnPropertyChanged(nameof(this.InviteCodeMessage));
        }

        // Change the IsEnabled of SubmitButton
        public void EnableButton(bool enabled = true) {
            this.SubmitInviteCodeButton.IsEnabled = enabled;

            OnPropertyChanged(nameof(this.SubmitInviteCodeButton));
        }

        // Test button for opening the Lobby
        private void ToLobbyButtonClicked(object sender, EventArgs e) {
            Navigation.PushAsync(new Lobby(null, null, 2), true);
        }

        // Test button for navigation to MapPage
        private void ToMapButtonClicked(object sender, EventArgs e) {
            Navigation.PushAsync(new MapPage(), true);
        }
    }
}
