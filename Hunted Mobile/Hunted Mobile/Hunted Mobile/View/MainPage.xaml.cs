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
            MainPageViewModel = new MainPageViewModel(this);
            BindingContext = MainPageViewModel; //new MainPageViewModel(this);
        }

        // This method is called when rendering this page
        protected override void OnAppearing() {
            //MainPageViewModel = new MainPageViewModel();
            base.OnAppearing();
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
