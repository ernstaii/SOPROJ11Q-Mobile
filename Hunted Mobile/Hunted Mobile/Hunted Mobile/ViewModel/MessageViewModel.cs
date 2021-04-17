using Hunted_Mobile.Service;
using Hunted_Mobile.View;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

using Xamarin.Forms;

namespace Hunted_Mobile.ViewModel {
    public class MessageViewModel : BaseViewModel{
        private Messages _page;
        public ObservableCollection<string> Messages { get; set; } = new ObservableCollection<string>();
        public MessageViewModel(Messages page){
            _page = page;

            AddMessage("Pauze");
            AddMessage("Hervat");
            AddMessage("We stoppen");

            WebSocketService socket = new WebSocketService(1);
            socket.StartGame += AddMessage("Het spel gaat beginnen!");
            socket.StartGame += AddMessage("Het spel gaat beginnen!");
            socket.StartGame += AddMessage("Het spel gaat beginnen!");
            socket.StartGame += AddMessage("Het spel gaat beginnen!");

            if(!WebSocketService.Connected) {
                socket.Connect();
            }
        }

        public void AddMessage(String message) {
            Messages.Add("[" + DateTime.Now.ToString("HH:mm") + "] Groepsleider: " + message);
        }
    }
}
