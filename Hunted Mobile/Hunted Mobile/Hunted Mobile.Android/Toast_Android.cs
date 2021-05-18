using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Hunted_Mobile.Droid;
using Hunted_Mobile;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(Toast_Android))]

namespace Hunted_Mobile.Droid {
    public class Toast_Android : Service.Toast {
        public void Show(string message) {
            Device.BeginInvokeOnMainThread(
                () => Android.Widget.Toast.MakeText(Android.App.Application.Context, message, ToastLength.Long).Show()
            );
        }
    }
}