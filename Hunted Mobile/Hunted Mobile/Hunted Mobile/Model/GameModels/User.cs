using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Hunted_Mobile.Model.GameModels {
    public class User {
        public int Id { get; set; }
        public Location Location { get; set; }
        public InviteKey InviteKey { get; set; }
        [Required(ErrorMessage = "De code is verplicht")]
        [MinLength(3, ErrorMessage = "De gebruikersnaam heeft een minimale lengte van 3 karaktes")]
        [MaxLength(50, ErrorMessage = "De gebruikersnam geeft een maximale lengte van 50 karaktes")]
        public string Name { get; set; }
        public string Role { get; set; }

        public User(int id) {
            Id = id;
        }

        public User() {
        }
    }
}
