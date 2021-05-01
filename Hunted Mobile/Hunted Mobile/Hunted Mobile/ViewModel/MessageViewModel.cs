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
        private Messages _page;
        public ObservableCollection<GameMessage> ChatMessages { get; set; } = new ObservableCollection<GameMessage>();

        public MessageViewModel(Messages page, int gameId){
            _page = page;

            WebSocketService socket = new WebSocketService(gameId);
            AddMessage("Het spel is begonnen!");
            socket.PauseGame += (data) => AddMessage((String)data.GetValue("message"));
            socket.ResumeGame += () => AddMessage("Het spel wordt hervat!");
            socket.EndGame += (data) => AddMessage((String)data.GetValue("message"));

            if(!WebSocketService.Connected) {
                Task.Run(async() => await socket.Connect());
            }
        }

        public void AddMessage(String message) {
            ChatMessages.Add(new GameMessage() {
                Message = message,
                Time = DateTime.Now.ToString("HH:mm"),
                UserName = "Spelleider"
            });
        }
    }
}
