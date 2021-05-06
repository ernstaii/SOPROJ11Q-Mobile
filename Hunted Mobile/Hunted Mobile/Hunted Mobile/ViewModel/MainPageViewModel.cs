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
        private InviteKey inviteKeyModel = new InviteKey();
        private bool isloading = false;
        private readonly InviteKeyRepository inviteKeyRepository = new InviteKeyRepository();
        private ObservableCollection<InviteKey> inviteKeys = new ObservableCollection<InviteKey>();
        private readonly MainPage page;
        private bool isOverlayVisible;
        private GameRepository _gameRepository;
        private MainPage _page;

        public InviteKey InviteKeyModel {
            get => inviteKeyModel;
            set {
                inviteKeyModel = value;
                OnPropertyChanged("InviteKeyModel");
            }
        }

        public bool SubmitButtonIsEnable {
            get => isloading;
            set {
                isloading = value;
                OnPropertyChanged("SubmitButtonIsEnable");
            }
        }

        /// <summary>
        /// This property will disable the touch of the user with the mapView
        /// </summary>
        public bool IsOverlayVisible {
            get => isOverlayVisible;
            set {
                isOverlayVisible = value;
                OnPropertyChanged("IsOverlayVisible");
            }
        }

        public ObservableCollection<InviteKey> InviteKeys {
            get { return inviteKeys; }
            set {
                inviteKeys = value;
                OnPropertyChanged("InviteKeys");
            }
        }

        public bool IsValid { get; set; }

        public InviteKey SelectedPreferenceGame { get; set; }

        public MainPageViewModel(MainPage page) {
            _page = page;
            _gameRepository = new GameRepository();
            this.page = page;
        }

        /// <summary>
        /// Getting InviteKey based on the Value
        /// </summary>
        /// <returns></returns>
        public async Task GetInviteKey() {
            if(IsValid = ValidationHelper.IsFormValid(InviteKeyModel, page)) {
                var result = await inviteKeyRepository.GetAll(InviteKeyModel.Value);
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

            await GetInviteKey();

            Game toBeJoined = await _gameRepository.GetGame(InviteKeyModel.GameId);

            // Navigate when InviteKey is valid
            if((IsValid = ValidationHelper.IsFormValid(InviteKeyModel, _page) && !IsOverlayVisible) || toBeJoined.Status == GameStatus.OnGoing || toBeJoined.Status == GameStatus.Paused || toBeJoined.Status == GameStatus.Finished) { 
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
