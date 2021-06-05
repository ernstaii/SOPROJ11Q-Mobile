using System;
using System.Collections.Generic;
using System.Text;

namespace Hunted_Mobile.Model.GameModels {
    public class FakePolice : Thief {
        public FakePolice(Thief player) : base(player, player.CaughtAt) {
        }
    }
}
