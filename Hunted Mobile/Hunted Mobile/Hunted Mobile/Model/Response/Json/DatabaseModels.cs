﻿
using System;

namespace Hunted_Mobile.Model.Response.Json {
#pragma warning disable IDE1006 // Naming Styles
    public struct UserData : DataModel {
        public int id;
        public string username;
        public string location;
        public string status;
        public string role;
        public string caught_at;
        public string created_at;
        public string updated_at;
    }

    public struct LootData : DataModel {
        public int id;
        public int lootable_id;
        public string lootable_type;
        public string name;
        public string location;
        public int game_id;
    }

    public struct InviteKeyData : DataModel {
        public string value;
        public int game_id;
        public int user_id;
        public string role;
        public DateTime created_at;
        public DateTime updated_at;
    }
}