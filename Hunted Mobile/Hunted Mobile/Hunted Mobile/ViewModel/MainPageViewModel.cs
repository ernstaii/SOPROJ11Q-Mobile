using Hunted_Mobile.Model;
using Hunted_Mobile.Repository;
using Hunted_Mobile.Service;
using Hunted_Mobile.View;

using System.Collections.ObjectModel;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Forms;
using System.Linq;

namespace Hunted_Mobile.ViewModel {
    public class MainPageViewModel : BaseViewModel {
        private InviteKey _inviteKeyModel = new InviteKey();
        private bool _isloading = false;
        private InviteKeyRepository _inviteKeyRepository = new InviteKeyRepository();
        private ObservableCollection<InviteKey> _inviteKeys = new ObservableCollection<InviteKey>();
        private MainPage _page;
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
        private bool _isOverlayVisible;

        /// <summary>
        /// This property will disable the touch of the user with the mapView
        /// </summary>
        public bool IsOverlayVisible {
            get => _isOverlayVisible;
            set {
                _isOverlayVisible = value;
                OnPropertyChanged("IsOverlayVisible");
            }
        }

        public ObservableCollection<InviteKey> InviteKeys {
            get { return _inviteKeys; }
            set {
                _inviteKeys = value;
                OnPropertyChanged("InviteKeys");
            }
        }

        public bool IsValid { get; set; }

        public InviteKey SelectedPreferenceGame { get; set; }

        public MainPageViewModel(MainPage page) {
            _page = page;
        }

        /// <summary>
        /// Getting InviteKey based on the Value
        /// </summary>
        /// <returns></returns>
        public async Task GetAll() {
            if(IsValid = ValidationHelper.IsFormValid(InviteKeyModel, _page)) {
                var result = await _inviteKeyRepository.GetAll(InviteKeyModel.Value);
                IsOverlayVisible = false;

                if(result.Count == 1) {
                    InviteKeyModel = result.First();
                } else if(result.Count > 1) {
                    InviteKeys.Clear();

                    foreach(var inviteKey in result) {
                        InviteKeys.Add(inviteKey);
                    }

                    IsOverlayVisible = true;
                }
            }
        }

        /// <summary>
        /// Navigate to the EnterUsernamePage with a valid InviteKey
        /// </summary>
        public ICommand ButtonSelectedCommand => new Command(async (e) => {
            SubmitButtonIsEnable = false;
            await GetAll();

            // Navigate when InviteKey is valid
            if(IsValid = ValidationHelper.IsFormValid(InviteKeyModel, _page) && !IsOverlayVisible) { 
                await NavigateToEnterUsernamePage();
            }

            SubmitButtonIsEnable = true;
        });


        /// <summary>
        /// Set selected InviteKey as InviteKeymodel and navigate to the next page
        /// </summary>
        public ICommand EnterGamePreference => new Command(async (e) => {
            if(SelectedPreferenceGame != null) {
                InviteKeyModel = SelectedPreferenceGame;

                await NavigateToEnterUsernamePage();
                IsOverlayVisible = false;
            }
        });

        public async Task NavigateToEnterUsernamePage() {
            await Xamarin.Forms.Application.Current.MainPage.Navigation.PushAsync(new EnterUsername(InviteKeyModel));
        }
    }
}
