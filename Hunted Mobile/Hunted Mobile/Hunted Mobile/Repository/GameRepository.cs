using Hunted_Mobile.Model;
using Hunted_Mobile.Service;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Hunted_Mobile.Repository {
    public class GameRepository {
        public async Task<int?> GetInterval(int gameId) {
            var intervalResponse = await new HttpClient().GetAsync(HttpClientService.GetUrl($"game/{gameId}/interval"));
            var intervalResult = await ConvertResponseService.ConvertRaw(intervalResponse);

            if(intervalResult == null) {
                return null;
            }
            else {
                int.TryParse(intervalResult, out int parsed);
                return parsed;
            }
        }
    }
}
