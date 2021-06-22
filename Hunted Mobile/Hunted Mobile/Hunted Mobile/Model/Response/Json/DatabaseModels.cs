
using System;

namespace Hunted_Mobile.Model.Response.Json {
#pragma warning disable IDE1006 // Naming Styles
    public struct UserData : JsonResponseData {
        public int id;
        public string username;
        public string location;
        public string status;
        public string role;
        public string caught_at;
        public bool triggered_alarm;
        public bool is_fake_agent;
        public string created_at;
        public string updated_at;
        public object gadgets;
    }

    public struct LootData : JsonResponseData {
        public int id;
        public int lootable_id;
        public string lootable_type;
        public string name;
        public string location;
        public int game_id;
    }

    public struct InviteKeyData : JsonResponseData {
        public string value;
        public int game_id;
        public int user_id;
        public string role;
        public DateTime created_at;
        public DateTime updated_at;
    }

    // Currently unused because there is no BorderMarker model. Might be used in the future
    public struct BorderMarkerData : JsonResponseData {
        public int id;
        public int borderable_id;
        public int borderable_type;
        public string location;
        public DateTime created_at;
        public DateTime updated_at;
    }

    public struct LocationData : JsonResponseData {
        public string location;
    }

    public struct GameData : JsonResponseData {
        public int id;
        public string status;
        public int duration;
        public int interval;
        public int time_left;
        public string police_station_location;
        public int thieves_score;
        public int police_score;
        public DateTime last_interval_at;
        public DateTime started_at;
        public object logo;
        public string colour_theme;
        public DateTime created_at;
        public DateTime updated_at;
    }

    public struct GadgetData : JsonResponseData {
        public int id;
        public string name;
        public DateTime created_at;
        public DateTime updated_at;
        public GadgetPivotData pivot;
    }

    public struct GadgetPivotData : JsonResponseData {
        public int user_id;
        public int gadget_id;
        public int amount;
        public string location;
        public bool in_use;
        public DateTime activated_at;
    }
}
