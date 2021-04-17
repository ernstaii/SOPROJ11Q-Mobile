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

            Messages.Add("[" + DateTime.Now.ToString("HH:mm") + "] Groepsleider: " + "Pauze!");
            Messages.Add("[" + DateTime.Now.ToString("HH:mm") + "] Groepsleider: " + "Hervat!");
            Messages.Add("[" + DateTime.Now.ToString("HH:mm") + "] Groepsleider: " + "Einde");
        }
    }
}
