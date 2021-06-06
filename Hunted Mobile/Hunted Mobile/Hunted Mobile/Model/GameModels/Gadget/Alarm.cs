using Hunted_Mobile.Model.Response.Json;
using Hunted_Mobile.Repository;

using System;

namespace Hunted_Mobile.Model.GameModels.Gadget {
    public class Alarm : Gadget {
        public float TriggerRangeInMeters => 50;
        public Location Location { get; }

        public Alarm(GadgetData data, Location location) : base(data) {
            Location = location;
            Description = "Plaatst een alarm op de huidige locatie dat afgaat wanneer een boef in de buurt komt";
            Icon = GetUriImageSource(UnitOfWork.Instance.ResourceRepository.GetGuiImage("alarm.png"));
        }
    }
}
