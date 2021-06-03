using Hunted_Mobile.Service;

using System.Threading.Tasks;

namespace Hunted_Mobile.Repository {
    public class GadgetRepository {
        public GadgetRepository() {

        }

        public async Task<bool> UseGadget(int playerId, string gadgetName) {
            var response = new HttpClientResponse();
            await response.Convert(HttpClientRequestService.Patch($"users/{playerId}/gadgets/{gadgetName}"));

            return response.IsSuccessful;
        }

        public async Task<bool> TriggerAlarm(int playerId) {
            var response = new HttpClientResponse();
            await response.Convert(HttpClientRequestService.Create($"users/{playerId}/trigger-alarm", new { }));

            return response.IsSuccessful;
        }
    }
}
