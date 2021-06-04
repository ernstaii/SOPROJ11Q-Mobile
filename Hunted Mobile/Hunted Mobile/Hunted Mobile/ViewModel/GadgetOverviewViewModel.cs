using Hunted_Mobile.Model;
using Hunted_Mobile.Model.GameModels;
using Hunted_Mobile.Model.GameModels.Gadget;
using Hunted_Mobile.Service;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Hunted_Mobile.ViewModel {
    public class GadgetOverviewViewModel : BaseViewModel {
        public class GadgetWithCommand : BaseViewModel {
            private bool used = false;

            public Player PlayingUser { get; set; }
            public Gadget Gadget { get; set; }
            public ObservableCollection<GadgetWithCommand> GadgetCollection { get; set; }
            public ICommand UseGadgetCommand => new Xamarin.Forms.Command(async (e) => {
                used = true; // Disable the button while awaiting success
                OnPropertyChanged(nameof(Available));
                used = await PlayingUser.Use(Gadget);
                OnPropertyChanged(nameof(Available));
            });
            public bool Available => !Gadget.InUse && !used;

            public GadgetWithCommand(Gadget gadget, Player playingUser, ObservableCollection<GadgetWithCommand> gadgetCollection) {
                Gadget = gadget;
                PlayingUser = playingUser;
                GadgetCollection = gadgetCollection;
            }

            public void Update() {
                OnPropertyChanged(nameof(Available));
            }
        }

        private readonly WebSocketService socketService;
        private ObservableCollection<GadgetWithCommand> gadgets;
        private readonly Map mapModel;

        public ObservableCollection<GadgetWithCommand> Gadgets {
            get => gadgets; 
            set {
                if(value == null) {
                    gadgets = new ObservableCollection<GadgetWithCommand>();
                }
                else gadgets = value;

                OnPropertyChanged(nameof(Gadgets));
            }
        }

        public GadgetOverviewViewModel(WebSocketService webSocketService, Map mapModel) {
            Gadgets = new ObservableCollection<GadgetWithCommand>();
            this.mapModel = mapModel;
            socketService = webSocketService;

            socketService.GadgetsUpdated += GadgetsUpdate;
            socketService.IntervalEvent += IntervalEvent;
        }

        private void IntervalEvent(Model.Response.IntervalEventData data) {
            var gadgets = data.Players.Where(p => p.Id == mapModel.PlayingUser.Id).FirstOrDefault()?.Gadgets;
            UpdateGadgets(gadgets);
        }

        private void GadgetsUpdate(Model.Response.GadgetsUpdatedEventData data) {
            if(data?.Player?.Id == mapModel.PlayingUser.Id) {
                UpdateGadgets(data.Gadgets);
            }
        }

        private void UpdateGadgets(ICollection<Gadget> gadgets) {
            if(gadgets != null) {
                Gadgets.Clear();
                foreach(var gadget in gadgets) {
                    Gadgets.Add(new GadgetWithCommand(gadget, mapModel.PlayingUser, Gadgets));
                }
            }
        }

        public void Update() {
            foreach(var singleGadgetViewModel in Gadgets) {
                singleGadgetViewModel.Update();
            }
        }
    }
}
