﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Hunted_Mobile {
    public partial class MainPage : ContentPage {
        public MainPage() {
            InitializeComponent();
        }

        private void ToMapButtonClicked(object sender, EventArgs e) {
            Navigation.PushAsync(new MapPage(), true);
        }
    }
}
