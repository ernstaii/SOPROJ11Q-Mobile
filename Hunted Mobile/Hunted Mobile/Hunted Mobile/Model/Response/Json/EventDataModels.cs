using System;
using System.Collections.Generic;
using System.Text;

namespace Hunted_Mobile.Model.Response.Json {
#pragma warning disable IDE1006 // Naming Styles
    public class EventData : JsonResponseData {
        public string message;
        public int timeLeft;
    }

    public class IntervalEventData : EventData {
        public UserData[] users;
        public LootData[] loot;
    }

    public class UserEventData : EventData {
        public UserData user;
    }

    public class ScoreUpdatedEventData : EventData {
        public int thief_score;
        public int police_score;
    }
}
