using Hunted_Mobile.Model;

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
        public Game game = null;

        public EnterUsername(Game game) {
            InitializeComponent();
            BindingContext = this;

            this.game = game;
        }       

        private void HandleJoinGame(object sender, EventArgs e) {
            this.JoinGameButton.IsEnabled = false;
            this.Validate();

            if(isValid) {
                // TODO: Navigate to labby
                var previousPage = Navigation.NavigationStack.LastOrDefault();
                Navigation.PushAsync(new Lobby(), true);
                Navigation.RemovePage(previousPage);
            }

            this.JoinGameButton.IsEnabled = true;
        }

        // Check if InviteCode is valid
        public void Validate() {
            isValid = this.UserNameField.Text != null && this.UserNameField.Text.Length >= 4;

            // Display ErrorMessage
            this.UserNameMessage.Text = isValid ? "" : "Gebruikersnaam is verplicht en moet minimaal 4 karakters bevatten";
            OnPropertyChanged(nameof(this.UserNameMessage));
        }

        // Get game based on InviteCode
        // TODO: Join a Game, maybe create user
        public void JoinGame() {
            //return new Game();
        }
    }
}
