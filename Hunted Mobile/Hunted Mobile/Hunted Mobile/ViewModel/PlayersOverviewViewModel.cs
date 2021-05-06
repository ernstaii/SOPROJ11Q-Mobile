using Hunted_Mobile.Model;
using Hunted_Mobile.Model.GameModels;
using Hunted_Mobile.Repository;
using Hunted_Mobile.Service;

using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Hunted_Mobile.ViewModel {
    public class PlayersOverviewViewModel : BaseViewModel {
        private List<Player> users = new List<Player>();
        private readonly WebSocketService socketService;

        // ReadOnlyList because operations like Add and Remove would not call OnPropertyChanged
        public IReadOnlyList<Player> Users {
            get => users;
            private set {
                users = new List<Player>(value);
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
            Users = users;
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

            socketService.ThiefCaught += UpdateUserState;
            socketService.ThiefReleased += UpdateUserState;
            socketService.IntervalEvent += UpdateUsers;
        }

        private void UpdateUsers(JObject data) {
            List<Player> updatedUsers = new List<Player>();
            foreach(JObject jUser in data.GetValue("users")) {
                var newUser = new Player() {
                    Id = int.Parse(jUser.GetValue("id")?.ToString()),
                    UserName = jUser.GetValue("username")?.ToString(),
                    CaughtAt = jUser.GetValue("caught_at")?.ToString(),
                };
                string role = jUser.GetValue("role")?.ToString();
                if(role == "thief") {
                    updatedUsers.Add(new Thief(newUser));
                }
                else updatedUsers.Add(new Police(newUser));
            }
            Users = updatedUsers;
        }

        private void UpdateUserState(Newtonsoft.Json.Linq.JObject data) {
            JObject jUserToUpdate = (JObject) data.GetValue("user");
            int id = int.Parse(jUserToUpdate.GetValue("id")?.ToString());
            foreach(Player player in Users) {
                if(player.Id == id) {
                    player.CaughtAt = jUserToUpdate.GetValue("caught_at")?.ToString();
                    break;
                }
            }
            Users = Users; // Trigger OnPropertyChanged
        }
    }
}
