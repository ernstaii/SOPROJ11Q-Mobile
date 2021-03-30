using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Hunted_Mobile {
    public partial class MainPage : ContentPage {
        public MainPage() {
            this.InitializeComponent();
        }

        private void Button_Clicked(object sender, EventArgs e) {
            Console.WriteLine(this.InviteCode.Text);
        }
    }
}
