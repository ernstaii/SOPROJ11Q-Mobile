using Hunted_Mobile.Model.GameModels.Gadget;
using Hunted_Mobile.Service;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Hunted_Mobile.ViewModel {
    public class GadgetOverviewViewModel : BaseViewModel {
        private readonly WebSocketService socketService;
        private readonly int playingUserId;
        private ObservableCollection<Gadget> gadgets;

        public ObservableCollection<Gadget> Gadgets {
            get => gadgets; 
            set {
                if(value == null) {
                    gadgets = new ObservableCollection<Gadget>();
                }
                else gadgets = value;

                OnPropertyChanged(nameof(Gadgets));
            } 
        }

        public GadgetOverviewViewModel(WebSocketService webSocketService, int playingUserId) {
            Gadgets = new ObservableCollection<Gadget>();
            this.playingUserId = playingUserId;
            socketService = webSocketService;

            socketService.GadgetsUpdated += GadgetsUpdate;
            socketService.IntervalEvent += IntervalEvent;
        }

        private void IntervalEvent(Model.Response.IntervalEventData data) {
            var gadgets = data.Players.Where(p => p.Id == playingUserId).FirstOrDefault()?.Gadgets;
            if(gadgets != null) {
                Gadgets = new ObservableCollection<Gadget>(gadgets);
            }
        }

        private void GadgetsUpdate(Model.Response.GadgetsUpdatedEventData data) {
            if(data?.Player?.Id == playingUserId) {
                Gadgets = new ObservableCollection<Gadget>(data.Gadgets);
            }
        }
    }
}
