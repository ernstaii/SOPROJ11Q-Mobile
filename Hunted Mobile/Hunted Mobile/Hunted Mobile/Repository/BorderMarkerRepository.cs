using Hunted_Mobile.Model;
using Hunted_Mobile.Service;
using Hunted_Mobile.Service.Json;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hunted_Mobile.Repository {
    public class BorderMarkerRepository {
        public async Task<List<Location>> GetAll(int gameId) {
            var response = new HttpClientResponse() {
                HasMultipleResults = true,
            };
            await response.Convert(HttpClientRequestService.GetAll($"games/{gameId}/border-markers"));

            List<Location> result = new List<Location>(
                new LocationJsonService().ToObjects(response.ResponseContent)
            );

            return result;
        }
    }
}
