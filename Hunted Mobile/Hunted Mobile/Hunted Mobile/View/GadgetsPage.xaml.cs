using Hunted_Mobile.ViewModel;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Hunted_Mobile.View {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GadgetsPage : ContentPage {
        public GadgetsPage(GadgetOverviewViewModel viewModel) {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}