using Hunted_Mobile.Service;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hunted_Mobile.Repository {
    public class NotificationRepository {
        public async Task<bool> Create(string message, int gameId, int? userId) {
            var response = new HttpClientResponse();

            dynamic parameters = new {
                message = message,
            };
            if(userId != null) {
                parameters.user_id = userId;
            };

            await response.Convert(HttpClientRequestService.Create($"games/{gameId}/notifications", parameters));

            return response.IsSuccessful;
        }
    }
}
