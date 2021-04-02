using Hunted_Mobile.Model;
using Hunted_Mobile.Model.GameModels;
using Hunted_Mobile.Repository;

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
        private List<User> _users = new List<User>();
        private UserRepository _userRepository = new UserRepository();

        public ObservableCollection<User> Thiefs {
            get => new ObservableCollection<User>(_users.Where(user => user.Role == "thief").ToList());
        }
        public ObservableCollection<User> Cops {
            get => new ObservableCollection<User>(_users.Where(user => user.Role == "police").ToList());
        }

        private int gameId;

        public Lobby(InviteKey inviteKey, User user, int gameId = 0) {
            InitializeComponent();
            BindingContext = this;

            // TODO: Only for testing purpose
            this.gameId = gameId;

            this.GetGameUsers();
        }

        // Load all users inside a game
        public async Task GetGameUsers() {
            this.Loading();
            this._users = await _userRepository.GetAll(gameId);

            this.ListOfCops.ItemsSource = Cops;
            this.ListOfThiefs.ItemsSource = Thiefs;

            OnPropertyChanged(nameof(this.ListOfCops));
            OnPropertyChanged(nameof(this.ListOfThiefs));

            this.Loading(false);
        }

        private void Loading(bool isLoading = true) {
            Spinner.IsRunning = isLoading;
            SpinnerLayout.IsVisible = isLoading;
        }
    }
}
