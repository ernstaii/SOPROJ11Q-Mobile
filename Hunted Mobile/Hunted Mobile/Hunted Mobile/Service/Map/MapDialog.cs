using Hunted_Mobile.Enum;

using System.Diagnostics.CodeAnalysis;

using Xamarin.Forms;

namespace Hunted_Mobile.Service.Map {
    public class MapDialog : BindableObject {
        private string title;
        private string description;
        private bool isVisible;

        private bool cancelButtonIsVisible;
        private bool handleButtonIsVisible;
        private bool handleButtonIsEnabled = true;
        private bool handleButtonHasHoldEvent = false;
        private string handleButtonText;

        private MapDialogOptions selectedDialog = MapDialogOptions.NONE;

        public MapDialogOptions SelectedDialog {
            get => selectedDialog;
            set {
                selectedDialog = value;
                IsVisible = value != MapDialogOptions.NONE;
            }
        }

        public string Title {
            get => title;
            set {
                title = value;
                OnPropertyChanged();
            }
        }

        public string Description {
            get => description;
            set {
                description = value;
                OnPropertyChanged();
            }
        }

        public bool IsVisible {
            get => isVisible;
            private set {
                isVisible = value;
                OnPropertyChanged();
            }
        }

        public bool CancelButtonIsVisible {
            get => cancelButtonIsVisible;
            set {
                cancelButtonIsVisible = value;
                OnPropertyChanged();
            }
        }

        public bool HandleButtonIsVisible {
            get => handleButtonIsVisible;
            set {
                handleButtonIsVisible = value;
                OnPropertyChanged();
            }
        }

        public bool HandleButtonIsEnabled {
            get => handleButtonIsEnabled;
            set {
                handleButtonIsEnabled = value;
                OnPropertyChanged();
            }
        }

        public string HandleButtonText {
            get => handleButtonText;
            set {
                handleButtonText = value;
                OnPropertyChanged();
            }
        }

        public bool HandleButtonHasHoldEvent {
            get => handleButtonHasHoldEvent && HandleButtonIsVisible;
            set {
                handleButtonHasHoldEvent = value;
                HasSingleClickEvent = !value;
                OnPropertyChanged();
            }
        }

        public bool HasSingleClickEvent {
            get => !HandleButtonHasHoldEvent && HandleButtonIsVisible;
            private set => OnPropertyChanged();
        }

        public MapDialog() { }

        public void CloseDialog() {
            SelectedDialog = MapDialogOptions.NONE;
        }

        public void SetContent(string title, string description) {
            Title = title;
            Description = description;
        }

        private void SetActionButtons(bool hasHandleButton = true, bool hasCancelButton = true) {
            HandleButtonIsVisible = hasHandleButton;
            CancelButtonIsVisible = hasCancelButton;
        }

        private void HideActionButtons() {
            SetActionButtons(false, false);
        }

        private void FinishSuccessfullyAction() {
            HandleButtonHasHoldEvent = false;
            CancelButtonIsVisible = false;
            HandleButtonText = "Klaar";
        }

        public void DisplayPickingUpLoot(string title, bool isToFarFromSelectedLoot = false) {
            string description = isToFarFromSelectedLoot ? "Je bent te ver weg om deze buit op te pakken." : "Houd de knop 5 seconden ingedrukt om de buit op te pakken.";

            HandleButtonHasHoldEvent = true;
            HandleButtonText = "Oppakken";

            SetContent(title, description);
            SetActionButtons();
        }

        public void DisplayPickedUpLootSuccessfully(string title) {
            SetContent(title, "✔ Klaar! De buit is opgepakt!");
            FinishSuccessfullyAction();
        }

        public void DisplayArrestingThief(string title, bool isToFarFromSelectedThief = false) {
            string description = isToFarFromSelectedThief ? "Je bent te ver weg om deze dief op te pakken." : "Houd de knop 5 seconden ingedrukt om de dief op te pakken.";

            HandleButtonHasHoldEvent = true;
            HandleButtonText = "Arresteren";

            SetContent(title, description);
            SetActionButtons();
        }

        public void DisplayArrestedThiefSuccessfully(string title) {
            SetContent(title, "✔ Klaar! Opgepakt! De dief is gearresteerd!");
            FinishSuccessfullyAction();
        }

        public void DisplayPauseScreen() {
            HandleButtonHasHoldEvent = false;

            SetContent("Gepauzeerd", "Momenteel is het spel gepauzeerd door de spelleider. Wanneer de pauze voorbij is, zal het spel weer hervat worden.");
            HideActionButtons();
        }

        public void DisplayEndScreen() {
            HandleButtonHasHoldEvent = false;

            SetContent("Het spel is afgelopen!", "Ga terug naar de spelleider!");
            HandleButtonText = "Terug naar hoofdscherm";
            SetActionButtons(true, false);
        }
    }
}
