using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Hunted_Mobile.Model.GameModels {
    public class Player : CustomModelErrorMessages<Player> {
        public int Id { get; set; }
        public Location Location { get; set; }
        public InviteKey InviteKey { get; set; }

        [Required(ErrorMessage = "De gebruikersnaam is verplicht")]
        [MinLength(3, ErrorMessage = "De gebruikersnaam heeft een minimale lengte van 3 karaktes")]
        [MaxLength(50, ErrorMessage = "De gebruikersnam geeft een maximale lengte van 50 karaktes")]
        public string UserName { get; set; }

        public bool IsCaught => CaughtAt != null && !CaughtAt.Equals("") || Status == "caught";

        public bool IsFree => !IsCaught;
        public string CaughtAt { get; set; }
        public string Status { get; set; }


        public Player(int id, string userName, InviteKey inviteKey) {
            Id = id;
            UserName = userName;
            InviteKey = inviteKey;
        }

        public Player() {}

        public Player(Player player) {
            Id = player.Id;
            Location = player.Location;
            UserName = player.UserName;
            InviteKey = player.InviteKey;
            Status = player.Status;
            CaughtAt = player.CaughtAt;
            ErrorMessages = player.ErrorMessages;
            CaughtAt = player.CaughtAt;
            Status = player.Status;
        }
    }
}
