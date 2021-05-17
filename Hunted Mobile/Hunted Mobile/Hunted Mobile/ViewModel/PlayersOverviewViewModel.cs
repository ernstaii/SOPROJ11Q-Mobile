using Hunted_Mobile.Model.GameModels;
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

        private void AddUser(JObject data) {
            JObject jUser = (JObject) data.GetValue("user");

            var newUser = new Player() {
                Id = int.Parse(jUser.GetValue("id")?.ToString()),
                UserName = jUser.GetValue("username")?.ToString(),
            };
            string role = jUser.GetValue("role")?.ToString();
            if(role == "thief") {
                Thief thief = new Thief(newUser);
                thief.CaughtAt = jUser.GetValue("caught_at")?.ToString();
                users.Add(thief);
            }
            else users.Add(new Police(newUser));

            Users = users; // Trigger OnPropertyChanged
        }

        private void UpdateUsers(JObject data) {
            ObservableCollection<Player> updatedUsers = new ObservableCollection<Player>();
            foreach(JObject jUser in data.GetValue("users")) {
                var newUser = new Player() {
                    Id = int.Parse(jUser.GetValue("id")?.ToString()),
                    UserName = jUser.GetValue("username")?.ToString(),
                };
                string role = jUser.GetValue("role")?.ToString();
                if(role == "thief") {
                    Thief thief = new Thief(newUser);
                    thief.CaughtAt = jUser.GetValue("caught_at")?.ToString();
                    updatedUsers.Add(thief);
                }
                else updatedUsers.Add(new Police(newUser));
            }
            Users = updatedUsers; // Trigger OnPropertyChanged
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
