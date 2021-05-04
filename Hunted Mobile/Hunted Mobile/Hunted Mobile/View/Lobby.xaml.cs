using Hunted_Mobile.Model;
using Hunted_Mobile.Model.GameModels;
using Hunted_Mobile.ViewModel;

using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Hunted_Mobile.View {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Lobby : ContentPage {
        public Lobby(User userModel, string gameStatus) {
            InitializeComponent();
            BindingContext = new LobbyViewModel(this, userModel, gameStatus);
        }
    }
}
