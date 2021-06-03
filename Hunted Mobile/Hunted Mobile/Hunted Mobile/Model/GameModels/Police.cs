using System.Collections.Generic;

namespace Hunted_Mobile.Model.GameModels {
    public class Police : Player {
        public Police(
            int id,
            string username,
            InviteKey inviteKey,
            Location location,
            string status,
            ICollection<Gadget.Gadget> gadgets,
            bool triggeredAlarm
        ) : base(id, username, inviteKey, location, status, gadgets, triggeredAlarm) { 
        }
    }
}
