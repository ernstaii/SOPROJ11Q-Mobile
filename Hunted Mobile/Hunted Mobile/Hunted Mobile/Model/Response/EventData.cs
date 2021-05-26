using Hunted_Mobile.Model.GameModels;
using Hunted_Mobile.Model.GameModels.Gadget;

using System;
using System.Collections.Generic;
using System.Text;

namespace Hunted_Mobile.Model.Response {
    public class EventData : ResponseData {
        public string Message { get; set; }
        public int TimeLeft { get; set; }
    }

    public class IntervalEventData : EventData {
        public Player[] Players { get; set; }
        public Loot[] Loot { get; set; }
        public bool DroneActive { get; set; }
    }

    public class PlayerEventData : EventData {
        public Player Player { get; set; }
    }

    public class ScoreUpdatedEventData : EventData {
        public int ThiefScore { get; set; }
        public int PoliceScore { get; set; }
    }

    public class GadgetsUpdatedEventData : EventData {
        public Gadget[] Gadgets { get; set; }
        public Player Player { get; set; }
    }
}
