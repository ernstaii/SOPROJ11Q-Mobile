using Hunted_Mobile.ViewModel;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Hunted_Mobile.View {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Lobby : ContentPage {
        public Lobby(LobbyViewModel viewModel) {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
