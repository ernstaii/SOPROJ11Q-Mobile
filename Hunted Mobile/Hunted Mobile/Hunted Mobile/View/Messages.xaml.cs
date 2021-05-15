using Hunted_Mobile.ViewModel;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Hunted_Mobile.View {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Messages : ContentPage {
        public Messages(MessageViewModel viewModel) {
            InitializeComponent();

            viewModel.CollectionView = ChatMessages_Collection;

            BindingContext = viewModel;
        }
    }
}
