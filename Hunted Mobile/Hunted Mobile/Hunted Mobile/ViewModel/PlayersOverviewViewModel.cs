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
        private bool playerIsPolice;

        public ObservableCollection<Player> Users {
            get => users;
            private set {
                users = value;
                OnPropertyChanged(nameof(Thieves));
                OnPropertyChanged(nameof(Police));
            }
        }

        public IReadOnlyCollection<Player> Thieves {
            get => Users.Where(user => user is Thief && !(user is FakePolice) || user is FakePolice && !playerIsPolice).ToList();
        }

        public IReadOnlyCollection<Player> Police {
            get => Users.Where(user => user is Police || playerIsPolice && user is FakePolice).ToList();
        }

        public PlayersOverviewViewModel(Player playingUser, WebSocketService socketService) {
            playerIsPolice = playingUser is Police;
            Users = new ObservableCollection<Player>(new List<Player>() { playingUser });
            Users.CollectionChanged += Users_CollectionChanged;
            this.socketService = socketService;

            Task.Run(async () => await SetupSocket());
        }

        private void Users_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
            OnPropertyChanged(nameof(Thieves));
            OnPropertyChanged(nameof(Police));
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
