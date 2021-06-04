using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using Hunted_Mobile.Repository;

namespace Hunted_Mobile.Model.GameModels {
    public class Player : CustomModelErrorMessages<Player> {
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

        public Player(int id, string userName, InviteKey inviteKey) {
            Id = id;
            UserName = userName;
            InviteKey = inviteKey;
        }

        public Player() { }

        protected Player(Player player) {
            Id = player.Id;
            Location = player.Location;
            UserName = player.UserName;
            InviteKey = player.InviteKey;
            ErrorMessages = player.ErrorMessages;
            Status = player.Status;
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
