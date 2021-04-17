using Hunted_Mobile.ViewModel;

using Mapsui;
using Mapsui.Projection;
using Mapsui.Utilities;
using Mapsui.Widgets;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
