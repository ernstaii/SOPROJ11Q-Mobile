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
            int errorCode = 137;

            if(w32ex == null && e.InnerException != null) {
                    w32ex = e.InnerException as Win32Exception;
                    errorCode = w32ex.ErrorCode;
            }

            var response = new HttpClientResponse();
            await response.Convert(HttpClientRequestService.Create("app-errors", new {
                error_id = errorCode,
                message = e.Message ?? "Er was geen message",
                stacktrace = e.StackTrace ?? "Er was geen stacktrace"
            }));
        }

        public async void Create(PusherException e) {
            var response = new HttpClientResponse();
            await response.Convert(HttpClientRequestService.Create("app-errors", new {
                error_id = int.Parse(e.PusherCode.ToString()),
                message = e.Message ?? "Er was geen message [PUSHER ERROR]",
                stacktrace = e.StackTrace ?? "Er was geen stacktrace [PUSHER ERROR]"
            }));
        }
    }
}
