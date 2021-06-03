using System.Collections.Generic;

namespace Hunted_Mobile.Model.GameModels {
    public class Thief : Player {
        public bool IsCaught => CaughtAt != null && !CaughtAt.Equals("") || Status == "caught";

        public bool IsFree => !IsCaught;

        public string CaughtAt { get; set; }

        public Thief(
            int id,
            string username,
            InviteKey inviteKey,
            Location location,
            string status,
            ICollection<Gadget.Gadget> gadgets,
            bool triggeredAlarm,
            string caughtAt
        ) : base(id, username, inviteKey, location, status, gadgets, triggeredAlarm) {
            CaughtAt = caughtAt;
        }
    }
}
