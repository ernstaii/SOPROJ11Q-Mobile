using Hunted_Mobile.Model.Response.Json;
using Hunted_Mobile.Repository;
using Hunted_Mobile.Service.Map;

namespace Hunted_Mobile.Model.GameModels.Gadget {
    public class SmokeScreen : Gadget {
        public SmokeScreen(GadgetData data) : base(data) {
            Description = "Zorgt ervoor dat je locatie niet te zien is in de volgende locatieupdate";
            Icon = GetUriImageSource(UnitOfWork.Instance.ResourceRepository.GetGuiImage("smoke.png"));
        }
    }
}
