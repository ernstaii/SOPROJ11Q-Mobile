using Hunted_Mobile.Model;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hunted_Mobile.Repository;

using Xamarin.Forms;
using Hunted_Mobile.ViewModel;
using Hunted_Mobile.Model.GameModels;

namespace Hunted_Mobile.View {
    public partial class MainPage : ContentPage {
        private MainPageViewModel _mainPageViewModel;
        public MainPage() {
            this.InitializeComponent();
            BindingContext = _mainPageViewModel = new MainPageViewModel(this);
        }

        // This method is called when rendering this page, because the InviteKey should be reset
        protected override void OnAppearing() {
            _mainPageViewModel.InviteKeyModel = new InviteKey();
            base.OnAppearing();
        }

        // Test button for navigation to MapPage
        private void ToMapButtonClicked(object sender, EventArgs e) {
            Navigation.PushAsync(new MapPage(), true);
        }
    }
}
