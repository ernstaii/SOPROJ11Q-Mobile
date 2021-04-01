using Hunted_Mobile.Model;
using Hunted_Mobile.Model.GameModels;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Hunted_Mobile.View {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Lobby : ContentPage {
        public Lobby(InviteKey inviteKey, User user) {
            InitializeComponent();
            BindingContext = this;
        }
    }
}
