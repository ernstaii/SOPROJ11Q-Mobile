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
            socketService.ThiefCaught += UpdateUserState;
            socketService.ThiefReleased += UpdateUserState;
            socketService.IntervalEvent += UpdateUsers;
        }

        private void UpdateUsers(JObject data) {
            List<Player> users = new List<Player>();
            foreach(JObject jUser in data.GetValue("users")) {
                var newUser = new Player() {
                    Id = int.Parse(jUser.GetValue("id")?.ToString()),
                    UserName = jUser.GetValue("username")?.ToString()
                };
                foreach(Player existingPlayer in Users) {
                    if(existingPlayer.Id == newUser.Id) {
                        if(existingPlayer is Thief) {
                            users.Add(new Thief(newUser));
                        }
                        else if(existingPlayer is Police) {
                            users.Add(new Police(newUser));
                        }
                        break;
                    }
                }
            }
            Users = users;
        }

        private void UpdateUserState(Newtonsoft.Json.Linq.JObject data) {
            JObject jUserToUpdate = (JObject) data.GetValue("user");
            foreach(Player player in Users) {
                int id = int.Parse(jUserToUpdate.GetValue("id")?.ToString());
                if(player.Id == id) {
                    player.Status = jUserToUpdate.GetValue("status")?.ToString();
                    break;
                }
            }
            Users = Users; // Trigger OnPropertyChanged
        }
    }
}
