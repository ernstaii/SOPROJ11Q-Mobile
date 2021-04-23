using Hunted_Mobile.Model;
using Hunted_Mobile.ViewModel;

using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Hunted_Mobile {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapPage : ContentPage {
        public MapPage(MapViewModel viewModel) {
            InitializeComponent();

            Task.Run(async () => await viewModel.SetMapView(mapView));

            BindingContext = viewModel;
        }
    }
}
