using System;
using System.Collections.Generic;
using System.Text;

namespace Hunted_Mobile.Model.GameModels {
    public class FakePolice : Thief {
        public FakePolice(Thief thief) : base(
            thief.Id,
            thief.UserName,
            thief.InviteKey,
            thief.Location,
            thief.Status,
            thief.Gadgets,
            thief.TriggeredAlarm,
            thief.CaughtAt
        ) {
        }
    }
}
