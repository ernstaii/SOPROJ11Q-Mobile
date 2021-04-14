using Hunted_Mobile.Model;
using Hunted_Mobile.Model.GameModels;
using Hunted_Mobile.ViewModel;

using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Hunted_Mobile.View {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Lobby : ContentPage {
        private LobbyViewModel LobbyViewModel;

        public Lobby(User userModel) {
            InitializeComponent();
            LobbyViewModel = new LobbyViewModel(this, userModel);
            BindingContext = LobbyViewModel;

            // TODO: somehow these are not binding
            /*this.ListOfCops.ItemsSource = LobbyViewModel.Police;
            this.ListOfThiefs.ItemsSource = LobbyViewModel.Thiefs;
            OnPropertyChanged(nameof(this.ListOfCops));
            OnPropertyChanged(nameof(this.ListOfThiefs));*/
        }
    }
}
