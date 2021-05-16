using System;
using System.Collections.Generic;
using System.Text;

namespace Hunted_Mobile.Model.Response.Json {
#pragma warning disable IDE1006 // Naming Styles
    public struct IntervalEventData {
        public IReadOnlyList<UserData> users;
        public IReadOnlyList<LootData> loot;
    }
}
