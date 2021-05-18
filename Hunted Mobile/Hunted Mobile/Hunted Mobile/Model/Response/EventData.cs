using Hunted_Mobile.Model.GameModels;

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
    }
}
