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
        private List<Player> _users = new List<Player>();
        private Game _gameModel = new Game();
        private Player _currentUser;
        private readonly UserRepository _userRepository = new UserRepository();
        private readonly GameRepository _gameRepository = new GameRepository();
        private readonly InviteKeyRepository _inviteKeyRepository = new InviteKeyRepository();
        private Lobby _page;
        private bool _isloading { get; set; }
        private readonly WebSocketService _webSocketService;

        public Game GameModel {
            get => _gameModel;
            set {
                _gameModel = value;
                OnPropertyChanged("GameModel");
            }
        }

        public bool IsLoading {
            get => _isloading;
            set {
                _isloading = value;
                OnPropertyChanged("IsLoading");
            }
        }

        public List<Player> Users {
            get => _users;
            set {
                _users = value;
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
            _page = page;
            _currentUser = currentUser;
            _gameModel.Id = _currentUser.InviteKey.GameId;

            _webSocketService = new WebSocketService(_gameModel.Id);
            Task.Run(async () => await StartSocket());

            Task.Run(async () => await LoadUsers());
        }

        private async Task StartSocket() {
            if(!WebSocketService.Connected) {
                await _webSocketService.Connect();
            }

            _webSocketService.StartGame += StartGame;
        }

        private async void StartGame() {
            GameModel = await _gameRepository.GetGame(_gameModel.Id);
            NavigateToMapPage();
        }

        // To manually navigate to a different page, the mainthread need to be approached
        public void NavigateToMapPage() {
            try {
                Map mapModel = new Map() {
                    PlayingUser = _currentUser
                };

                var mapPage = new MapPage(new MapViewModel(GameModel, mapModel, new Service.Gps.GpsService(), new LootRepository(), _userRepository, _gameRepository, _inviteKeyRepository));

                Device.BeginInvokeOnMainThread(() => {
                    Xamarin.Forms.Application.Current.MainPage.Navigation.PushAsync(mapPage, true);
                    _webSocketService.StartGame -= StartGame;
                });
            }
            catch(Exception ex) {
                Console.WriteLine(ex.ToString());
            }
        }

        public async Task LoadUsers() {
            IsLoading = true;
            Users.Clear();
            foreach(User user in await _userRepository.GetAll(GameModel.Id, _inviteKeyRepository)) {
                if(user is Player) {
                    Users.Add((Player) user);
                }
            }
            IsLoading = false;
        }
    }
}
