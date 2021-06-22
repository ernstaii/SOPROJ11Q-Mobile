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
                OnPropertyChanged();

                if(value == MapDialogOptions.NONE) {
                    IsVisible = false;
                }
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
                OnPropertyChanged();
            }
        }

        public bool HasSingleClickEvent  => !HandleButtonHasHoldEvent && HandleButtonIsVisible;

        public MapDialog() { }

        public void CloseDialog() {
            SelectedDialog = MapDialogOptions.NONE;
        }

        public void SetContent(string title, string description) {
            Title = title;
            Description = description;
        }

        private void SetActionButtons(bool hasHandleButton = true, bool hasCancelButton = true) {
            handleButtonIsVisible = hasHandleButton;
            cancelButtonIsVisible = hasCancelButton;
        }

        private void HideActionButtons() {
            SetActionButtons(false, false);
        }

        private void FinishSuccessfullyAction() {
            handleButtonIsVisible = true;
            HandleButtonText = "Klaar";
        }

        public void DisplayPickingUpLoot(string title, bool isCloseToSelectedLoot = false) {
            BeforeDisplayScreen();
            string description = isCloseToSelectedLoot ? "Houd de knop 5 seconden ingedrukt om de buit op te pakken." : "Je bent te ver weg om deze buit op te pakken.";

            HandleButtonHasHoldEvent = true;
            HandleButtonText = "Oppakken";

            SetContent(title, description);
            SetActionButtons(isCloseToSelectedLoot);
            AfterDisplayScreen();
        }

        public void DisplayPickedUpLootSuccessfully(string title) {
            BeforeDisplayScreen();
            SetContent(title, "✔ Klaar! De buit is opgepakt!");
            FinishSuccessfullyAction();
            AfterDisplayScreen();
        }

        public void DisplayArrestingThief(string title, bool isCloseToSelectedThief = false) {
            BeforeDisplayScreen();
            string description = isCloseToSelectedThief ? "Houd de knop 5 seconden ingedrukt om de boef op te pakken." : "Je bent te ver weg om deze boef op te pakken.";

            HandleButtonHasHoldEvent = true;
            HandleButtonText = "Arresteren";

            SetContent(title, description);
            SetActionButtons(isCloseToSelectedThief);
            AfterDisplayScreen();
        }

        public void DisplayArrestedThiefSuccessfully(string title) {
            BeforeDisplayScreen();
            SetContent(title, "✔ Klaar! Opgepakt! De boef is gearresteerd!");
            FinishSuccessfullyAction();
            AfterDisplayScreen();
        }

        public void DisplayPauseScreen() {
            BeforeDisplayScreen();
            SetContent("Gepauzeerd", "Momenteel is het spel gepauzeerd door de spelleider. Wanneer de pauze voorbij is, zal het spel weer hervat worden.");
            HideActionButtons();
            AfterDisplayScreen();
        }

        public void DisplayEndScreen() {
            BeforeDisplayScreen();
            SetContent("Het spel is afgelopen!", "Ga terug naar de spelleider!");
            HandleButtonText = "Terug naar hoofdscherm";

            SetActionButtons(true, false);
            AfterDisplayScreen();
        }

        public void DisplayBoundaryScreen() {
            BeforeDisplayScreen();
            SetContent("Keer terug!", "Je bevindt je buiten de spelgrens! Ga zo snel mogelijk terug.");
            SetActionButtons(false);
            AfterDisplayScreen();
        }

        public void DisplayArrestedScreen() {
            BeforeDisplayScreen();
            SetContent("Gearresteerd!", "Je bent opgepakt en kan niet meer deelnemen aan het spel!");
            AfterDisplayScreen();
        }

        private void BeforeDisplayScreen() {
            handleButtonHasHoldEvent = false;
            handleButtonIsVisible = false;
            cancelButtonIsVisible = false;
        }

        private void AfterDisplayScreen() {
            OnPropertyChanged(nameof(HasSingleClickEvent));
            OnPropertyChanged(nameof(HandleButtonHasHoldEvent));
            OnPropertyChanged(nameof(HandleButtonIsVisible));
            OnPropertyChanged(nameof(CancelButtonIsVisible));

            IsVisible = SelectedDialog != MapDialogOptions.NONE;
        }
    }
}
