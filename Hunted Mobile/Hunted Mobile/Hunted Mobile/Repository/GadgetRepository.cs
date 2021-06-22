using Hunted_Mobile.Model.GameModels.Gadget;
using Hunted_Mobile.Service;
using Hunted_Mobile.Service.Builder;
using Hunted_Mobile.Service.Json;

using System.Collections.Generic;
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

        public async Task<List<PlayerBuilder>> GetAll(int gameId){
            var response = new HttpClientResponse();
            await response.Convert(HttpClientRequestService.GetAll($"games/{gameId}/gadgets"));

            List<PlayerBuilder> playersWithGadgets = new List<PlayerBuilder>();

            foreach(PlayerBuilder player in new PlayerJsonService().ToObjects(response.ResponseContent)) {
                playersWithGadgets.Add(player);
            }

            return playersWithGadgets;
        }
    }
}
