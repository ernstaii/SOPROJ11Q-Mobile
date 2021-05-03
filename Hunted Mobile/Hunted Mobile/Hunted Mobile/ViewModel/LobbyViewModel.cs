using Hunted_Mobile.Model;
using Hunted_Mobile.Model.GameModels;
using Hunted_Mobile.Repository;
using Hunted_Mobile.Service;
using Hunted_Mobile.View;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Hunted_Mobile.ViewModel {
    public class LobbyViewModel : BaseViewModel {
        private List<User> users = new List<User>();
        private Game gameModel = new Game();
        private readonly User currentUser;
        private readonly UserRepository userRepository = new UserRepository();
        private readonly GameRepository gameRepository = new GameRepository();
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

        public List<User> Users {
            get => users;
            set {
                users = value;
                OnPropertyChanged("Thiefs");
                OnPropertyChanged("Police");
            }
        }

        public ObservableCollection<User> Thiefs {
            get => new ObservableCollection<User>(Users.Where(user => user.Role == "thief").ToList());
        }

        public ObservableCollection<User> Police {
            get => new ObservableCollection<User>(Users.Where(user => user.Role == "police").ToList());
        }

        public LobbyViewModel(Lobby page, User currentUser) {
            this.page = page;
            this.currentUser = currentUser;
            gameModel.Id = this.currentUser.InviteKey.GameId;

            webSocketService = new WebSocketService(gameModel.Id);
            Task.Run(async () => await StartSocket());

            Task.Run(async () => await LoadUsers());
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
                var mapPage = new MapPage(new MapViewModel(GameModel, mapModel, new Service.Gps.GpsService(), new LootRepository(), new UserRepository(), new BorderMarkerRepository()));

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
    }
}
