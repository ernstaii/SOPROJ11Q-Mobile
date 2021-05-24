using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace Hunted_Mobile.Model.GameModels {
    public class Player : CustomModelErrorMessages<Player> {
        private ObservableCollection<Gadget.Gadget> gadgets;

        public int Id { get; set; }
        public Location Location { get; set; }
        public InviteKey InviteKey { get; set; }

        [Required(ErrorMessage = "De gebruikersnaam is verplicht")]
        [MinLength(3, ErrorMessage = "De gebruikersnaam heeft een minimale lengte van 3 karaktes")]
        [MaxLength(50, ErrorMessage = "De gebruikersnam geeft een maximale lengte van 50 karaktes")]
        public string UserName { get; set; }
        public string Status { get; set; }

        public bool IsValid => UserName != null && Location != null;

        public ICollection<Gadget.Gadget> Gadgets { get => gadgets; }

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
    }
}
