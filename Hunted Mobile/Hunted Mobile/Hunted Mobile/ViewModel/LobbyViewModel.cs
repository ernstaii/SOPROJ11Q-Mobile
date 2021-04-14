using Hunted_Mobile.Model;
using Hunted_Mobile.Model.GameModels;
using Hunted_Mobile.Repository;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hunted_Mobile.ViewModel {
    public class LobbyViewModel {
        private readonly Game _model;
        private List<User> _users = new List<User>();
        private UserRepository _userRepository = new UserRepository();
        public EnterUsernameViewModel CurrentUser;

        public ObservableCollection<User> Thiefs {
            get => new ObservableCollection<User>(_users.Where(user => user.Role == "thief").ToList());
        }

        public ObservableCollection<User> Police {
            get => new ObservableCollection<User>(_users.Where(user => user.Role == "police").ToList());
        }

        public LobbyViewModel(Game game, EnterUsernameViewModel currentUser) {
            _model = game ?? new Game();
            CurrentUser = currentUser;
        }

        // Get all users within a game
        public async Task GetUsers() {
            _users = await _userRepository.GetAll(_model.Id);
        }
    }
}
