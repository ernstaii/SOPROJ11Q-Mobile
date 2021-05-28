using Hunted_Mobile.Service;

using PusherClient;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Hunted_Mobile.Repository {
    public class ErrorRepository {
        public async void Create(Exception e) {
            var w32ex = e as Win32Exception;

            var response = new HttpClientResponse();
            await response.Convert(HttpClientRequestService.Create("app-errors", new {
                error_id = w32ex.ErrorCode,
                message = e.Message,
                stacktrace = e.StackTrace
            }));
        }

        public async void Create(PusherException e) {

            var response = new HttpClientResponse();
            await response.Convert(HttpClientRequestService.Create("app-errors", new {
                error_id = e.PusherCode,
                message = e.Message,
                stacktrace = e.StackTrace
            }));
        }
    }
}
