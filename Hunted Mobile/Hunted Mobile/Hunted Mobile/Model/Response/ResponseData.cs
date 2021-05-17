using Hunted_Mobile.Model.GameModels;

using System;
using System.Collections.Generic;
using System.Text;

namespace Hunted_Mobile.Model.Response {
    public interface ResponseData {
    }

    public struct IntervalEventData : ResponseData {
        public Player[] Players { get; set; }
        public Loot[] Loot { get; set; }
        public string Message { get; set; }
        public int TimeLeft { get; set; }
    }
}
