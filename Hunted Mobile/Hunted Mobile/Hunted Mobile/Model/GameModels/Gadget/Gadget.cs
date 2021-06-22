using Hunted_Mobile.Model.Response.Json;
using Xamarin.Forms;

namespace Hunted_Mobile.Model.GameModels.Gadget {
    public abstract class Gadget {
        public int Id { get; }
        public string Name { get; }
        public bool InUse { get; }
        public string Description { get; set; }
        public UriImageSource Icon { get; set; }

        protected Gadget(GadgetData data) {
            Id = data.id;
            Name = data.name;
            InUse = data.pivot.in_use;
        }

        protected UriImageSource GetUriImageSource(Model.Resource.Resource resource) {
            return new UriImageSource() {
                Uri = resource.Uri,
                CachingEnabled = false
            };
        }
    }
}
