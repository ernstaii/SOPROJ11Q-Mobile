using Hunted_Mobile.Model;
using Hunted_Mobile.Model.GameModels;
using Hunted_Mobile.Repository;
using Hunted_Mobile.Service;
using Hunted_Mobile.View;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Hunted_Mobile.ViewModel {
    public class LobbyViewModel : BaseViewModel {
        private List<User> _users = new List<User>();
        private Game _gameModel = new Game();
        private User _currentUser;
        private readonly UserRepository _userRepository = new UserRepository();
        private Lobby _page;
        private bool _isloading { get; set; }

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

        public List<User> Users {
            get => _users;
            set {
                _users = value;
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
            _page = page;
            _currentUser = currentUser;
            _gameModel.Id = _currentUser.InviteKey.GameId;

            Task.Run(async () => await LoadUsers());

            if(!WebSocketService.Connected) {
                WebSocketService socket = new WebSocketService(_gameModel.Id);
                socket.Connect();
                socket.StartGame += StartGame;
            }
        }

        private void StartGame() {
            Task.Run(async () => await NavigateToMapPage());
        }

        // To manually navigate to a different page, the mainthread need to be approached
        public async Task NavigateToMapPage() {
            try {
                Device.BeginInvokeOnMainThread(() => {
                    Xamarin.Forms.Application.Current.MainPage.Navigation.PushAsync(new MapPage(), true);
                }); 
            }
            catch(Exception e) {
            }
        }

        public async Task LoadUsers() {
            IsLoading = true;
            Users = await _userRepository.GetAll(GameModel.Id);
            IsLoading = false;
        }
    }
}
