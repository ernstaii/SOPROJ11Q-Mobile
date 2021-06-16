using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using Hunted_Mobile.Repository;

namespace Hunted_Mobile.Model.GameModels {
    public abstract class Player : CustomModelErrorMessages<Player> {
        private List<Gadget.Gadget> gadgets;

        public int Id { get; set; }
        public Location Location { get; set; }
        public InviteKey InviteKey { get; set; }

        [Required(ErrorMessage = "De gebruikersnaam is verplicht")]
        [MinLength(3, ErrorMessage = "De gebruikersnaam heeft een minimale lengte van 3 karaktes")]
        [MaxLength(50, ErrorMessage = "De gebruikersnam geeft een maximale lengte van 50 karaktes")]
        public string UserName { get; set; }
        public string Status { get; set; }

        public bool IsValid => UserName != null && Location != null;

        public List<Gadget.Gadget> Gadgets { 
            get => gadgets;
            set {
                if(value == null) {
                    gadgets = new List<Gadget.Gadget>();
                }
                else gadgets = value;
            }
        }

        public bool TriggeredAlarm { get; set; }

        protected Player(
            int id,
            string username, 
            InviteKey inviteKey,
            Location location,
            string status,
            List<Gadget.Gadget> gadgets,
            bool triggeredAlarm
        ) {
            Id = id;
            Location = location;
            UserName = username;
            InviteKey = inviteKey;
            Status = status;
            TriggeredAlarm = triggeredAlarm;
            Gadgets = gadgets;
            ValidationField = "Player";
        }

        public async Task<bool> Use(Gadget.Gadget gadget) {
            foreach(Gadget.Gadget playerGadget in Gadgets) {
                if(playerGadget.Id == gadget.Id) {
                    bool success = await UnitOfWork.Instance.GadgetRepository.UseGadget(Id, playerGadget.Name);
                    if(success) {
                        Gadgets.Remove(playerGadget);
                    }
                    return success;
                }
            }
            return false;
        }
    }
}
