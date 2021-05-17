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

        public MessageViewModel(int gameId, CollectionView collection) {
            CollectionView = collection;

            WebSocketService socket = new WebSocketService(gameId);
            AddMessage("Het spel is begonnen!");
            socket.PauseGame += (data) => AddMessage((String) data.GetValue("message"));
            socket.ResumeGame += (data) => AddMessage((String) data.GetValue("message"));
            socket.NotificationEvent += (data) => AddMessage((String) data.GetValue("message"));
            socket.EndGame += (data) => AddMessage((String) data.GetValue("message"));
            socket.ThiefCaught += (data) => AddMessage((String) data.GetValue("message"));
            socket.ThiefReleased += (data) => AddMessage((String) data.GetValue("message"));

            if(!WebSocketService.Connected) {
                Task.Run(async () => await socket.Connect());
            }
        }

        public void AddMessage(String message) {
            ChatMessages.Insert(0, new GameMessage() {
                Message = message,
                Time = DateTime.Now.ToString("HH:mm"),
                UserName = "Spelleider"
            });

            // Scroll to top of CollectionView, because otherwise new items are not shown
            CollectionView.ScrollTo(0, position: ScrollToPosition.Start);
        }
    }
}
