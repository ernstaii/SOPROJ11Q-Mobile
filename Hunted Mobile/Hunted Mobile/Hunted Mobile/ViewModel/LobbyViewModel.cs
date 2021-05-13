using Hunted_Mobile.Enum;
using Hunted_Mobile.Model;
using Hunted_Mobile.Model.GameModels;
using Hunted_Mobile.Repository;
using Hunted_Mobile.Service;
using Hunted_Mobile.View;

using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Hunted_Mobile.ViewModel {
    public class LobbyViewModel : BaseViewModel {
        private List<Player> users = new List<Player>();
        private Game gameModel = new Game();
        private readonly Player currentUser;
        private readonly UserRepository userRepository = new UserRepository();
        private readonly GameRepository gameRepository = new GameRepository(); 
        private readonly InviteKeyRepository inviteKeyRepository = new InviteKeyRepository();
        private readonly Lobby page;
        private readonly WebSocketService webSocketService;

        private bool isloading;
        
        public Game GameModel {
            get => gameModel;
            set {
                gameModel = value;
                OnPropertyChanged("GameModel");
            }
        }

        public bool IsLoading {
            get => isloading;
            set {
                isloading = value;
                OnPropertyChanged("IsLoading");
            }
        }

        public List<Player> Users {
            get => users;
            set {
                users = value;
                OnPropertyChanged("Thiefs");
                OnPropertyChanged("Police");
            }
        }

        public ObservableCollection<Player> Thiefs {
            get => new ObservableCollection<Player>(Users.Where(user => user is Thief).ToList());
        }

        public ObservableCollection<Player> Police {
            get => new ObservableCollection<Player>(Users.Where(user => user is Police).ToList());
        }

        public LobbyViewModel(Lobby page, Player currentUser) {
            this.page = page;
            this.currentUser = currentUser;
            gameModel.Id = this.currentUser.InviteKey.GameId;

            webSocketService = new WebSocketService(gameModel.Id);
            Task.Run(async () => await StartSocket());
            Task.Run(async () => await LoadUsers());
            Task.Run(async () => await CheckForStatus());
        }

        private async Task StartSocket() {
            if(!WebSocketService.Connected) {
                await webSocketService.Connect();
            }

            webSocketService.StartGame += StartGame;
        }

        private async void StartGame() {
            GameModel = await gameRepository.GetGame(gameModel.Id);
            NavigateToMapPage();
        }

        // To manually navigate to a different page, the mainthread need to be approached
        public void NavigateToMapPage() {
            try {
                Map mapModel = new Map() {
                    PlayingUser = currentUser
                };

                var mapPage = new MapPage(new MapViewModel(GameModel, mapModel, new Service.Gps.GpsService(), new LootRepository(), userRepository, gameRepository, inviteKeyRepository, new BorderMarkerRepository(), new ResourceRepository(), new NotificationRepository()));

                Device.BeginInvokeOnMainThread(() => {
                    Xamarin.Forms.Application.Current.MainPage.Navigation.PushAsync(mapPage, true);
                    webSocketService.StartGame -= StartGame;
                });
            }
            catch(Exception ex) {
                Console.WriteLine(ex.ToString());
            }
        }

        public async Task LoadUsers() {
            IsLoading = true;
            Users = await userRepository.GetAll(GameModel.Id);
            IsLoading = false;
        }

        public async Task CheckForStatus() {
            Game gameStatus = await gameRepository.GetGame(gameModel.Id);
            if(gameStatus.Status == GameStatus.ONGOING || gameStatus.Status == GameStatus.PAUSED || gameStatus.Status == GameStatus.FINISHED) {
                StartGame();
            }
        }
    }
}
