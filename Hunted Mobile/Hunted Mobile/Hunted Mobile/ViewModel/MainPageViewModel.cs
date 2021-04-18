﻿using Hunted_Mobile.Model;
using Hunted_Mobile.Repository;
using Hunted_Mobile.Service;
using Hunted_Mobile.View;

using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Forms;

namespace Hunted_Mobile.ViewModel {
    public class MainPageViewModel : BaseViewModel {
        private InviteKey _inviteKeyModel = new InviteKey();
        private bool _isloading = false;
        public InviteKey InviteKeyModel {
            get => _inviteKeyModel;
            set {
                _inviteKeyModel = value;
                OnPropertyChanged("InviteKeyModel");
            }
        }

        public bool SubmitButtonIsEnable {
            get => _isloading;
            set {
                _isloading = value;
                OnPropertyChanged("SubmitButtonIsEnable");
            }
        }
        private bool _isOverlayVisible { get; set; }
        /// <summary>
        /// This property will disable the touch of the user with the mapView
        /// </summary>
        public bool IsOverlayVisible {
            get => _isOverlayVisible;
            set {
                _isOverlayVisible = value;

                OnPropertyChanged("IsOverlayVisible ");
            }
        }

        private InviteKeyRepository _inviteKeyRepository = new InviteKeyRepository();
        private MainPage _page;

        public bool IsValid { get; set; }

        public MainPageViewModel(MainPage page) {
            _page = page;
        }

        /// <summary>
        /// Getting InviteKey based on the Value
        /// </summary>
        /// <returns></returns>
        public async Task Get() {
            IsValid = ValidationHelper.IsFormValid(InviteKeyModel, _page);

            if(IsValid) {
                var result = await _inviteKeyRepository.Get(InviteKeyModel.Value);

                if(result != null)
                    InviteKeyModel = result;
                else
                    IsValid = false;
            }
        }

        /// <summary>
        /// Navigate to the EnterUsernamePage with a valid InviteKey
        /// </summary>
        public ICommand ButtonSelectedCommand => new Command(async (e) => {
            SubmitButtonIsEnable = false;
            await Get();

            // Navigate when InviteKey is valid
            if(IsValid) {
                await NavigateToEnterUsernamePage();
            }

            SubmitButtonIsEnable = true;
        });

        public InviteKey SelectedPreferenceGame { get; set; }

        private ObservableCollection<InviteKey> _inviteKeys = new ObservableCollection<InviteKey>() {
            new InviteKey() { GameId = 1 },
            new InviteKey() { GameId = 2 }
        };

        public ObservableCollection<InviteKey> InviteKeys {
            get { return _inviteKeys; }
            set {
                _inviteKeys = value;
                OnPropertyChanged("InviteKeys ");
            }
        }

        /// <summary>
        /// Set selected InviteKey as InviteKeymodel and navigate to the next page
        /// </summary>
        public ICommand EnterGamePreference => new Command(async (e) => {
            if(SelectedPreferenceGame != null) {
                InviteKeyModel = SelectedPreferenceGame;

                await NavigateToEnterUsernamePage();
            }
        });

        public async Task NavigateToEnterUsernamePage() {
            await Xamarin.Forms.Application.Current.MainPage.Navigation.PushAsync(new EnterUsername(InviteKeyModel));
        }
    }
}
