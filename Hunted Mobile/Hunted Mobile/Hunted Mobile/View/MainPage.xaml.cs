using Hunted_Mobile.Model;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hunted_Mobile.Repository;

using Xamarin.Forms;
using Hunted_Mobile.ViewModel;

namespace Hunted_Mobile.View {
    public partial class MainPage : ContentPage {
        public MainPageViewModel MainPageViewModel;

        public MainPage() {
            this.InitializeComponent();
            BindingContext = this;
        }

        // This method is called when rendering this page
        protected override void OnAppearing() {
            MainPageViewModel = new MainPageViewModel();
            base.OnAppearing();
        }

        // If InviteCode is valid, then user will be redirected to screen for entering an username
        private async void SubmitInviteCode(object sender, EventArgs e) {
            // Disable the button, so user doesn't click the button twice for loading the game
            EnableButton(false);

            await MainPageViewModel.Get(this.InviteCodeField.Text);
            HandleErrorMessage();

            if(MainPageViewModel.IsValid) {
                await Navigation.PushAsync(new EnterUsername(MainPageViewModel), true);
            }

            EnableButton();
        }

        // Display or hide an errorMessage
        public void HandleErrorMessage() {
            this.InviteCodeMessage.Text = MainPageViewModel.IsValid ? "" : "De opgegeven code is ongeldig";
            OnPropertyChanged(nameof(this.InviteCodeMessage));
        }

        // Change the IsEnabled of SubmitButton
        public void EnableButton(bool enabled = true) {
            this.SubmitInviteCodeButton.IsEnabled = enabled;
            OnPropertyChanged(nameof(this.SubmitInviteCodeButton));
        }

        private void ToLobbyButtonClicked(object sender, EventArgs e) {
            Navigation.PushAsync(new Lobby(new EnterUsernameViewModel() {
                InviteKey = new InviteKey() {
                    GameId = 2
                }
            }), true);
        }

        // Test button for navigation to MapPage
        private void ToMapButtonClicked(object sender, EventArgs e) {
            Navigation.PushAsync(new MapPage(), true);
        }
    }
}
