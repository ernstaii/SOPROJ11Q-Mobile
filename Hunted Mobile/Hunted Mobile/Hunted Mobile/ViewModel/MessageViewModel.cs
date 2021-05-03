using Hunted_Mobile.Service;
using Hunted_Mobile.View;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Hunted_Mobile.ViewModel {
    public class MessageViewModel : BaseViewModel{
        private readonly Messages page;

        public ObservableCollection<string> Messages { get; set; } = new ObservableCollection<string>();

        public MessageViewModel(Messages page, int gameId){
            this.page = page;

            WebSocketService socket = new WebSocketService(gameId);
            AddMessage("Het spel is begonnen!");
            socket.PauseGame += (data) => AddMessage((String)data.GetValue("message"));
            socket.ResumeGame += () => AddMessage("Het spel wordt hervat!");
            socket.EndGame += (data) => AddMessage((String)data.GetValue("message"));

            if(!WebSocketService.Connected) {
                Task.Run(async () => await socket.Connect());
            }
        }

        private void AddMessage(String message) {
            Messages.Add("[" + DateTime.Now.ToString("HH:mm") + "] Spelleider: " + message);
        }
    }
}
