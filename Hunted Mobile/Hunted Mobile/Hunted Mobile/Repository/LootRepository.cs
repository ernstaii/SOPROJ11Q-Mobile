using Hunted_Mobile.Model;
using Hunted_Mobile.Model.GameModels;
using Hunted_Mobile.Service;
using Hunted_Mobile.Service.Json;

using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;


namespace Hunted_Mobile.Repository {
    public class LootRepository {
        // Get all loot that is linked to a game
        public async Task<List<Loot>> GetAll(int gameId) {
            var response = new HttpClientResponse() {
                HasMultipleResults = true,
            };
            await response.Convert(HttpClientRequestService.Get($"games/{gameId}/loot"));
            
            var result = new List<Loot>();

            // Looping through the result
            foreach(string lootJson in new ConvertFromJsonService(response.ResponseContent).ToArray()) {
                result.Add(new ConvertFromJsonService(lootJson).ToLoot());
            }

            return result;
        }

        public async Task<bool> Delete(int lootId) {
            var response = new HttpClientResponse();

            await response.Convert(HttpClientRequestService.Delete($"loot/{lootId}"));

            return response.IsSuccessful;
        }
    }
}
