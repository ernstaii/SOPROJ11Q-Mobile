using Hunted_Mobile.Model;
using Hunted_Mobile.Model.GameModels;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Hunted_Mobile.View {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Lobby : ContentPage {
        public ObservableCollection<User> Users = new ObservableCollection<User>();

        public Lobby(InviteKey inviteKey, User user, int gameId = 0) {
            InitializeComponent();
            BindingContext = this;
            this.ListOfThiefs.ItemsSource = Users;

            // Test purposes
            for(int x = 0; x < 10; x++) {
                Users.Add(new User(1) { Name = "Martijn Zentjens" });
            }
            OnPropertyChanged(nameof(this.ListOfThiefs));
        }

        // Load all users inside a game
        public async Task GetGameUsers() {
            // TODO: Load users
        }

        private async void Button_Clicked(object sender, EventArgs e) {
            Spinner.IsRunning = true;
            SpinnerLayout.IsVisible = true;

            await Task.Delay(2000);
            SpinnerLayout.IsVisible = false;
            Spinner.IsRunning = false;
        }
    }
}
