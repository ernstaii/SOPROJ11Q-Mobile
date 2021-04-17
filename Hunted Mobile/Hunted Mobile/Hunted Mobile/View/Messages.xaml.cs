using Hunted_Mobile.ViewModel;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Hunted_Mobile.View {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Messages : ContentPage {
        public Messages() {
            InitializeComponent();
            BindingContext = new MessageViewModel(this);
        }
    }
}