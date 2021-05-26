using Hunted_Mobile.Model.GameModels;
using Hunted_Mobile.Model.GameModels.Gadget;
using Hunted_Mobile.Service;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hunted_Mobile.Repository {
    public class GadgetRepository {
        public GadgetRepository() {

        }

        public async Task<bool> DecreaseGadgetAmount(int playerId, int gadgetId) {
            var response = new HttpClientResponse();
            await response.Convert(HttpClientRequestService.Patch(HttpClientRequestService.GetUrl($"users/{playerId}/gadgets/{gadgetId}")));

            return response.IsSuccessful;
        }
    }
}
