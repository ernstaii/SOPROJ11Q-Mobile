﻿using Hunted_Mobile.Model;
using Hunted_Mobile.Service;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Hunted_Mobile.Repository {
    public class GameRepository {
        public async Task<int?> GetInterval(int gameId) {
            var response = new HttpClientResponse();
            await response.Convert(HttpClientRequestService.Get($"game/{gameId}/interval"));

            if(response.ResponseContent == null) {
                return null;
            }
            else {
                int.TryParse(response.ResponseContent, out int parsed);
                return parsed;
            }
        }
    }
}
