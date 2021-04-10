using Hunted_Mobile.ViewModel;

using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Hunted_Mobile.View {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Lobby : ContentPage {
        private readonly GameViewModel gameViewModel;

        public Lobby(UserViewModel user) {
            InitializeComponent();
            BindingContext = this;

            gameViewModel = new GameViewModel(new Game() {
                GameId = user.InviteKey.GameId,
            }, user);

            this.LoadUsers();
        }

        // Load all users inside a game
        public async Task LoadUsers() {
            this.DisplayLoadingScreen();
            await this.gameViewModel.GetUsers();

            this.ListOfCops.ItemsSource = this.gameViewModel.Police;
            this.ListOfThiefs.ItemsSource = this.gameViewModel.Thiefs;

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
