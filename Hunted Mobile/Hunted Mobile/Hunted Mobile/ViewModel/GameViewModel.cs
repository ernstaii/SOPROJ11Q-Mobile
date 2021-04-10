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
    public class GameViewModel {
        private readonly Game _model;
        public UserViewModel CurrentUser;
        private List<User> _users = new List<User>();
        public UserRepository userRepository = new UserRepository();

        public ObservableCollection<User> Thiefs {
            get => new ObservableCollection<User>(_users.Where(user => user.Role == "thief").ToList());
        }

        public ObservableCollection<User> Police {
            get => new ObservableCollection<User>(_users.Where(user => user.Role == "police").ToList());
        }

        public GameViewModel(Game game, UserViewModel currentUser) {
            _model = game ?? new Game();
            CurrentUser = currentUser;
        }

        // Get all users within a game
        public async Task GetUsers() {
            _users = await userRepository.GetAll(_model.GameId);
        }
    }
}
