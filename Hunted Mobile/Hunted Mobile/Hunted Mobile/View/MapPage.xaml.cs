using Hunted_Mobile.Model;
using Hunted_Mobile.ViewModel;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Hunted_Mobile {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapPage : ContentPage {
        public MapPage(MapViewModel viewModel) {
            InitializeComponent();

            viewModel.SetMapView(mapView);

            BindingContext = viewModel;
        }
    }
}
