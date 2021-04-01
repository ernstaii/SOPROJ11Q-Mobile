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

        public MainPage() {
            this.InitializeComponent();
            BindingContext = this;
        }

        private void HandleJoinGame(object sender, EventArgs e) {
            this.Validate();

            // TODO: If valid, send to different screen
        }

        public void Validate() {
            // TODO: Check if valid
            isValid = this.InviteCode.Text != "";

            this.ErrorMessage.Text = isValid ? "" : "De opgegeven code is ongeldig";
            OnPropertyChanged(nameof(ErrorMessage));
        }

        private void ToMapButtonClicked(object sender, EventArgs e) {
            Navigation.PushAsync(new MapPage(), true);
        }
    }
}
