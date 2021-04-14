using Hunted_Mobile.Model;
using Hunted_Mobile.Model.GameModels;
using Hunted_Mobile.Repository;
using Hunted_Mobile.View;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hunted_Mobile.ViewModel {
    public class LobbyViewModel {
        private List<User> _users = new List<User>();
        private Game _gameModel = new Game();
        private User _currentUser;
        private bool _isloading { get; set; }
        public Game GameModel {
            get => _gameModel;
            set {
                _gameModel = value;

                if(PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("GameModel"));
            }
        }

        public List<User> Users {
            get => _users;
            set {
                _users = value;

                if(PropertyChanged != null) {
                    IsLoading = false;
                    PropertyChanged(this, new PropertyChangedEventArgs("Police"));
                    PropertyChanged(this, new PropertyChangedEventArgs("Thiefs"));
                    PropertyChanged(this, new PropertyChangedEventArgs("Users"));
                    PropertyChanged(this, new PropertyChangedEventArgs("IsLoading"));
                }
            }
        }

        public bool IsLoading {
            get => _isloading;
            set {
                _isloading = value;

                if(PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("IsLoading"));
            }
        }

        private readonly UserRepository _userRepository = new UserRepository();
        private Lobby _page;
        public event PropertyChangedEventHandler PropertyChanged;
        public bool IsValid { get; set; }

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

            this.LoadUsers();
        }

        public void LoadUsers() {
            IsLoading = true;

            GetAll().ContinueWith(t => IsLoading = false);
        }

        // Get all users within a game
        public async Task GetAll() {
            Users = await _userRepository.GetAll(GameModel.Id);
        }
    }
}
