using Hunted_Mobile.Model.Response.Json;

using System;
using System.Collections.Generic;
using System.Text;

namespace Hunted_Mobile.Model.GameModels {
    public class Thief : Player {
        public bool IsCaught => CaughtAt != null && !CaughtAt.Equals("") || Status == "caught";

        public bool IsFree => !IsCaught;

        public string CaughtAt { get; set; }

        public Thief(Player player, string caughtAt) : base(player) {
            CaughtAt = caughtAt;
        }
    }
}
