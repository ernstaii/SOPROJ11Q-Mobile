using Hunted_Mobile.Repository;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hunted_Mobile.Model.GameModels.Gadget {
    public abstract class Gadget {
        public int Id { get; set; }

        public async Task Use(Player user) {
            await UnitOfWork.Instance.GadgetRepository.DecreaseGadgetAmount(user.Id, Id);
            user.Gadgets.Remove(this);
        }

        public abstract bool CanBeUsed();
    }
}
