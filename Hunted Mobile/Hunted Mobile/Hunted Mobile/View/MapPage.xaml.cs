using Hunted_Mobile.Model;
using Hunted_Mobile.ViewModel;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Hunted_Mobile {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapPage : ContentPage {
        public MapPage(Game game) {
            InitializeComponent();

            BindingContext = new MapViewModel(mapView, game);
        }
    }
}
