using Hunted_Mobile.Model;
using Hunted_Mobile.ViewModel;

using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Hunted_Mobile.View {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Lobby : ContentPage {
        private readonly LobbyViewModel LobbyViewModel;

        public Lobby(EnterUsernameViewModel user) {
            InitializeComponent();
            BindingContext = this;

            LobbyViewModel = new LobbyViewModel(new Game() {
                Id = user.InviteKey.GameId,
            }, user);

            this.LoadUsers();
        }

        // Load all users inside a game
        public async Task LoadUsers() {
            this.DisplayLoadingScreen();
            await LobbyViewModel.GetUsers();

            this.ListOfCops.ItemsSource = LobbyViewModel.Police;
            this.ListOfThiefs.ItemsSource = LobbyViewModel.Thiefs;

            OnPropertyChanged(nameof(this.ListOfCops));
            OnPropertyChanged(nameof(this.ListOfThiefs));

            this.DisplayLoadingScreen(false);
        }

        private void DisplayLoadingScreen(bool isLoading = true) {
            Spinner.IsRunning = isLoading;
            SpinnerLayout.IsVisible = isLoading;
        }
    }
}
