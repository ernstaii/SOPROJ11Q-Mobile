using Hunted_Mobile.Model;
using Hunted_Mobile.Service;
using Hunted_Mobile.View;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Hunted_Mobile.ViewModel {
    public class MessageViewModel : BaseViewModel {
        public ObservableCollection<GameMessage> ChatMessages { get; set; } = new ObservableCollection<GameMessage>();
        public CollectionView CollectionView { get; set; }
        private string colourTheme;
        public string ColourTheme {
            get => colourTheme; set {
                colourTheme = value;
                OnPropertyChanged(ColourTheme);
            }
        }

        public MessageViewModel(string gameId, string colourTheme) {
            ColourTheme = colourTheme;
            WebSocketService socket = new WebSocketService(gameId.ToString());
            AddMessage("Het spel is begonnen!");
            socket.PauseGame += (data) => AddMessage(data.Message);
            socket.ResumeGame += (data) => AddMessage(data.Message);
            socket.NotificationEvent += (data) => AddMessage(data.Message);
            socket.EndGame += (data) => AddMessage(data.Message);
            socket.ThiefCaught += (data) => AddMessage(data.Message);
            socket.ThiefReleased += (data) => AddMessage(data.Message);

            if(!WebSocketService.Online) {
                Task.Run(async () => await socket.Connect());
            }
        }

        public void AddMessage(String message) {
            ChatMessages.Insert(0, new GameMessage() {
                Message = message,
                Time = DateTime.Now.ToString("HH:mm"),
                UserName = "Spelleider",
                FontColor = ColourTheme,
            });

            if(CollectionView != null) {
                // Scroll to top of CollectionView, because otherwise new items are not shown
                CollectionView.ScrollTo(0, position: ScrollToPosition.Start);
            }
        }
    }
}
