using Hunted_Mobile.Model;
using Hunted_Mobile.ViewModel;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Hunted_Mobile.View {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EnterUsername : ContentPage {
        public EnterUsername(InviteKey inviteKeyModel) {
            InitializeComponent();
            BindingContext = new EnterUsernameViewModel(this, inviteKeyModel);
        }
    }
}
