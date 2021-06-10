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
using Hunted_Mobile.Enum;
using Hunted_Mobile.Service.Preference;
using Hunted_Mobile.Model.GameModels;

namespace Hunted_Mobile.ViewModel {
    public class MainPageViewModel : BaseViewModel {
        private InviteKey inviteKeyModel = new InviteKey();
        private bool isloading;
        private ObservableCollection<InviteKey> inviteKeys = new ObservableCollection<InviteKey>();
        private readonly MainPage page;
        private Game gameModel;
        private Player playingUser;
        private bool isOverlayVisible;
        private bool checkIfUserCanJoinAGame;
        private bool displayJoinGameButton;
        private bool isLocked = false;
        private readonly GameSessionPreference gameSessionPreference;

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
                isloading = !isLocked && value;
                OnPropertyChanged("SubmitButtonIsEnable");
            }
        }

        public bool DisplayJoinGameButton {
            get => displayJoinGameButton;
            set {
                displayJoinGameButton = value;
                OnPropertyChanged(nameof(DisplayJoinGameButton));
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
            this.page = page;
            gameSessionPreference = new GameSessionPreference();
            AskPermission();
        }

        /// <summary>
        /// Getting InviteKey based on the Value
        /// </summary>
        /// <returns></returns>
        public async Task GetInviteKey() {
            if(IsValid = ValidationHelper.IsFormValid(InviteKeyModel, page)) {
                var result = await UnitOfWork.Instance.InviteKeyRepository.GetAll(InviteKeyModel.Value);
                IsOverlayVisible = false;

                if(result.Count == 1) {
                    InviteKeyModel = result.First();
                }
                else if(result.Count > 1) {
                    InviteKeys.Clear();

                    foreach(var inviteKey in result) {
                        InviteKeys.Add(inviteKey);
                    }

                    IsOverlayVisible = true;
                }
            }
        }

        public ICommand NavigateToEnterUserNamePageCommand => new Command(async (e) => {
            SubmitButtonIsEnable = false;

            await GetInviteKey();

            // Navigate when InviteKey is valid
            if(IsValid = ValidationHelper.IsFormValid(InviteKeyModel, page) && !IsOverlayVisible) {
                await NavigateToEnterUsernamePage();
            }

            SubmitButtonIsEnable = true;
        });

        public ICommand NavigateToMapPageCommand => new Command(async (e) => {
            await NavigateToExistingGame();
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
            var viewModel = new EnterUsernameViewModel(inviteKeyModel);
            var view = new EnterUsername(viewModel);
            await Xamarin.Forms.Application.Current.MainPage.Navigation.PushAsync(view);
        }

        public void ResetInviteKey() {
            InviteKeyModel = new InviteKey();
        }

        public void AskPermission() {
            bool isEnableState = SubmitButtonIsEnable;

            SubmitButtonIsEnable = false;
            PermissionService.AskPermissionForLocation()
                .ContinueWith(x => {
                    PermissionService.CheckPermissionLocation();

                    if(PermissionService.HasGpsPermission) {
                        SubmitButtonIsEnable = isEnableState;
                    } else {
                        isLocked = true;
                        OnPropertyChanged(nameof(SubmitButtonIsEnable));
                    }
                });
        }

        public async void LoadPreviousGame() {
            int gameId = gameSessionPreference.GetGame(),
                userId = gameSessionPreference.GetUser();

            if(gameId > 0 && userId > 0) {
                await GetGame(gameId);

                if(checkIfUserCanJoinAGame) {
                    await GetUser(userId, gameId);

                    if(playingUser == null) {
                        GameSessionPreference.ClearUserAndGame();
                    } else {
                        DisplayJoinGameButton = true;
                    }
                }
            }
        }

        private async Task GetGame(int gameId) {
            gameModel = await UnitOfWork.Instance.GameRepository.GetGame(gameId);
            checkIfUserCanJoinAGame = gameModel.Status == GameStatus.ONGOING || gameModel.Status == GameStatus.PAUSED || gameModel.Status == GameStatus.CONFIG;

            if(!checkIfUserCanJoinAGame) {
                GameSessionPreference.ClearUserAndGame();
            }
        }

        private async Task GetUser(int userId, int gameId) {
            playingUser = await UnitOfWork.Instance.UserRepository.GetUser(userId, gameId);
        }

        private async Task NotifyGame() {
            await UnitOfWork.Instance.NotificationRepository.Create(
                playingUser.UserName + " neemt weer deel aan het spel!",
                gameModel.Id,
                playingUser.Id
            );
        }

        private async Task NavigateToExistingGame() {
            if(!PermissionService.HasGpsPermission) return;

            SubmitButtonIsEnable = false;

            await UnitOfWork.Instance.UserRepository.Update(playingUser.Id, playingUser.Location);
            await NotifyGame();

            if(gameModel.Status == GameStatus.CONFIG) {
                await NavigateToLobbyPage();
            }
            else {
                await NavigateToMapPage();
            }
            SubmitButtonIsEnable = true;
        }

        private async Task NavigateToLobbyPage() {
            if(!PermissionService.HasGpsPermission) return;

            await Xamarin.Forms.Application.Current.MainPage.Navigation.PushAsync(new Lobby(new LobbyViewModel(playingUser)), true);
        }

        private async Task NavigateToMapPage() {
            try {
                Map mapModel = new Map() {
                    PlayingUser = playingUser,
                };

                var mapPage = new MapPage(new MapViewModel(gameModel, mapModel));
                await Xamarin.Forms.Application.Current.MainPage.Navigation.PushAsync(mapPage, true);
            }
            catch(Exception ex) {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
