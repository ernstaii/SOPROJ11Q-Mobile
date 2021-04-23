using Hunted_Mobile.ViewModel;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Hunted_Mobile.View {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Messages : ContentPage {
        public Messages(int gameId) {
            InitializeComponent();
            BindingContext = new MessageViewModel(this, gameId);
        }
    }
}