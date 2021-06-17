using System;
using System.Collections.Generic;
using System.Text;

namespace Hunted_Mobile.ViewModel {
    public class AppViewModel : BaseViewModel {
        private string colourTheme;

        public string ColourTheme {
            get => colourTheme;
            set {
                colourTheme = value;
                OnPropertyChanged(nameof(ColourTheme));
            }
        }

        public AppViewModel() {
            ResetColor();
        }

        public void ResetColor() {
            ColourTheme = null;
        }
    }
}
