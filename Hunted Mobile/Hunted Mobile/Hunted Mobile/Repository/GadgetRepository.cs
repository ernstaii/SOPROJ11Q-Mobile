using Hunted_Mobile.Service;

using System.Threading.Tasks;

namespace Hunted_Mobile.Repository {
    public class GadgetRepository {
        public GadgetRepository() {

        }

        public async Task<bool> DecreaseGadgetAmount(int playerId, string gadgetName) {
            var response = new HttpClientResponse();
            await response.Convert(HttpClientRequestService.Patch(HttpClientRequestService.GetUrl($"users/{playerId}/gadgets/{gadgetName}")));

            return response.IsSuccessful;
        }
    }
}
