using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Hunted_Mobile.Model {
    public class InviteKey : CustomModelErrorMessages<InviteKey> {
        [Required(ErrorMessage = "De code is verplicht")]
        [MinLength(3, ErrorMessage = "De code heeft een minimale lengte van 3 karaktes")]
        public string Value { get; set; }
        public int GameId { get; set; }
        public int UserId { get; set; }
        public string Role { get; set; }

        public string GameLabel => $"Spel {GameId}";
    }
}
