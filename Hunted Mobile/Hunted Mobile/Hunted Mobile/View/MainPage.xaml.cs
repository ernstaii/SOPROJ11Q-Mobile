using Hunted_Mobile.Model;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Hunted_Mobile.View {
    public partial class MainPage : ContentPage {
        public bool isValid = false;
        public Game game = null;

        public MainPage() {
            this.InitializeComponent();
            BindingContext = this;
        }

        // If InviteCode is valid, then user will be redirected to screen for entering an username
        private void SubmitInviteCode(object sender, EventArgs e) {
            // Disable the button, so user doesn't click the button twice for loading the game
            this.SubmitInviteCodeButton.IsEnabled = false;
            this.Validate();

            if(isValid) {
                Navigation.PushAsync(new EnterUsername(this.game), true);
            }

            this.SubmitInviteCodeButton.IsEnabled = true;
        }

        // Check if InviteCode is valid
        public void Validate() {
            if(this.InviteCodeField.Text != null)
                this.game = GetGame(this.InviteCodeField.Text);

            isValid = this.game != null;

            // Display ErrorMessage
            this.InviteCodeMessage.Text = isValid ? "" : "De opgegeven code is ongeldig";
            OnPropertyChanged(nameof(this.InviteCodeMessage));
        }

        // Get game based on InviteCode
        // TODO: Check if player is able to get the game, so it is able to validate InviteCode
        public Game GetGame(string code) {
            return new Game();
        }

        // Test button for navigation to MapPage
        private void ToMapButtonClicked(object sender, EventArgs e) {
            Navigation.PushAsync(new MapPage(), true);
        }
    }
}
