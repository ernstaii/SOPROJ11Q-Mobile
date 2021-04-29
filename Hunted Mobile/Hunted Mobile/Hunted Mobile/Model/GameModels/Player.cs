using System;
using System.Collections.Generic;
using System.Text;

namespace Hunted_Mobile.Model.GameModels {
    public abstract class Player : User {
        protected Player(User user) : base(user.Id, user.UserName, user.InviteKey) {
            ErrorMessages = user.ErrorMessages;
            Location = user.Location;
        }
    }
}
