using Hunted_Mobile.ViewModel;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Hunted_Mobile.View {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InformationPage : ContentPage {
        public InformationPage(InformationPageViewModel viewModel) {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}