using Hunted_Mobile.Model.GameModels;
using Hunted_Mobile.Model.Response;
using Hunted_Mobile.Service;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Hunted_Mobile.ViewModel {
    public class PlayersOverviewViewModel : BaseViewModel {
        private ObservableCollection<Player> users = new ObservableCollection<Player>();
        private readonly WebSocketService socketService;

        public ObservableCollection<Player> Users {
            get => users;
            private set {
                users = value;
                OnPropertyChanged("Thieves");
                OnPropertyChanged("Police");
            }
        }

        public ObservableCollection<Player> Thieves {
            get => new ObservableCollection<Player>(Users.Where(user => user is Thief).ToList());
        }

        public ObservableCollection<Player> Police {
            get => new ObservableCollection<Player>(Users.Where(user => user is Police).ToList());
        }

        public PlayersOverviewViewModel(IReadOnlyList<Player> users, WebSocketService socketService) {
            Users = new ObservableCollection<Player>(users);
            this.socketService = socketService;

            Task.Run(async () => await SetupSocket());
        }

        private async Task SetupSocket() {
            if(!WebSocketService.Online) {
                await socketService.Connect();
            }
            socketService.ThiefCaught -= UpdateUserState;
            socketService.ThiefReleased -= UpdateUserState;
            socketService.IntervalEvent -= UpdateUsers;
            socketService.PlayerJoined -= AddUser;

            socketService.ThiefCaught += UpdateUserState;
            socketService.ThiefReleased += UpdateUserState;
            socketService.IntervalEvent += UpdateUsers;
            socketService.PlayerJoined += AddUser;
        }

        private void AddUser(PlayerEventData data) {
            Users.Add(data.PlayerBuilder.ToPlayer());
        }

        private void UpdateUsers(IntervalEventData data) {
            Users.Clear();
            foreach(var builder in data.PlayerBuilders) {
                Users.Add(builder.ToPlayer());
            }
        }

        private void UpdateUserState(PlayerEventData data) {
            var eventPlayer = data.PlayerBuilder.ToPlayer();
            foreach(Player player in Users) {
                if(player.Id == eventPlayer.Id && eventPlayer is Thief) {
                    Users.Remove(player);
                    Users.Add(eventPlayer);
                    break;
                }
            }
        }
    }
}
