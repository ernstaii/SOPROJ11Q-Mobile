using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Hunted_Mobile.Model {
    public class InviteKey : CustomModelErrorMessages<InviteKey> {
        [Required(ErrorMessage = "De code is verplicht")]
        [MinLength(4, ErrorMessage = "De code moet bestaan uit 4 karaktes")]
        [MaxLength(4, ErrorMessage = "De code moet bestaan uit 4 karaktes")]
        public string Value { get; set; }
        public int GameId { get; set; }
        public int UserId { get; set; }
        public string Role { get; set; }

        public string GameLabel => $"Spel {GameId}";
    }
}
