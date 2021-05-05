using Hunted_Mobile.Model;
using Hunted_Mobile.Model.GameModels;
using Hunted_Mobile.Repository;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Hunted_Mobile.ViewModel {
    public class PlayersOverviewViewModel : BaseViewModel {
        private List<User> users = new List<User>();
        private Game gameModel = new Game();
        private readonly UserRepository userRepository = new UserRepository();

        public Game GameModel {
            get => gameModel;
            set {
                gameModel = value;
                OnPropertyChanged("GameModel");
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

        public PlayersOverviewViewModel(Game game) {
            gameModel = game;

            Task.Run(async () => await LoadUsers());
        }

        public async Task LoadUsers() {
            Users = await userRepository.GetAll(GameModel.Id);
        }
    }
}
