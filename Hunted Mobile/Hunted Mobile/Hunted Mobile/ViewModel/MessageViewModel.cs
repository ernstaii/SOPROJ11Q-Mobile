using Hunted_Mobile.Service;
using Hunted_Mobile.View;

using System;
using System.Collections.ObjectModel;

namespace Hunted_Mobile.ViewModel {
    public class MessageViewModel : BaseViewModel {
        private readonly Messages _page;

        public ObservableCollection<string> Messages { get; set; } = new ObservableCollection<string>();

        public MessageViewModel(Messages page, int gameId) {
            _page = page;

            WebSocketService socket = new WebSocketService(gameId);
            AddMessage("Het spel is begonnen!");
            socket.PauseGame += (data) => AddMessage((string) data.GetValue("message"));
            socket.ResumeGame += () => AddMessage("Het spel wordt hervat!");
            socket.EndGame += (data) => AddMessage((string) data.GetValue("message"));

            if(!WebSocketService.Connected) {
                socket.Connect();
            }
        }

        private void AddMessage(string message) {
            Messages.Add("[" + DateTime.Now.ToString("HH:mm") + "] Spelleider: " + message);
        }
    }
}
