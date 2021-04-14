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

namespace Hunted_Mobile.View {
    public partial class MainPage : ContentPage {
        public MainPage() {
            this.InitializeComponent();
            BindingContext = new MainPageViewModel(this);
        }

        // This method is called when rendering this page
        protected override void OnAppearing() {
            //BindingContext = new MainPageViewModel();
            base.OnAppearing();
        }

        // Test button for navigation to MapPage
        private void ToMapButtonClicked(object sender, EventArgs e) {
            Navigation.PushAsync(new MapPage(), true);
        }
    }
}
