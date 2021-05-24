using System;
using System.Collections.Generic;
using System.Text;

namespace Hunted_Mobile.Model.GameModels {
    public class FakeThief : Thief {
        public FakeThief(Player player, string caughtAt) : base(player, caughtAt) {
        }
    }
}
