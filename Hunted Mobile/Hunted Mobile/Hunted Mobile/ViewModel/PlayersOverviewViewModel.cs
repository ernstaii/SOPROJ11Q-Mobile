using Hunted_Mobile.Model.GameModels;
using Hunted_Mobile.Model.Response;
using Hunted_Mobile.Service;

using Newtonsoft.Json.Linq;

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
                OnPropertyChanged("Users");
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
            if(!WebSocketService.Connected) {
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
            users.Add(data.Player);
            Users = users; // Trigger OnPropertyChanged
        }

        private void UpdateUsers(IntervalEventData data) {
            Users = new ObservableCollection<Player>(data.Players);
        }

        private void UpdateUserState(Newtonsoft.Json.Linq.JObject data) {
            JObject jUserToUpdate = (JObject) data.GetValue("user");
            int id = int.Parse(jUserToUpdate.GetValue("id")?.ToString());
            foreach(Player player in Users) {
                if(player.Id == id && player is Thief) {
                    ((Thief)player).CaughtAt = jUserToUpdate.GetValue("caught_at")?.ToString();
                    break;
                }
            }
            Users = Users; // Trigger OnPropertyChanged
        }
    }
}
