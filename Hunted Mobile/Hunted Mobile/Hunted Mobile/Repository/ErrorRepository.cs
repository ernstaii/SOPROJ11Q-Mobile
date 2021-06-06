using Hunted_Mobile.Service;

using PusherClient;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Hunted_Mobile.Repository {
    public class ErrorRepository {
        private const string DEFAULT_MESSAGE = "No message available";
        private const string DEFAULT_STACKTRACE = "No stacktrace available";

        public async void Create(Exception e) {
            var response = new HttpClientResponse();
            await response.Convert(HttpClientRequestService.Create("app-errors", new {
                error_id = GetErrorId(e),
                message = e.Message ?? DEFAULT_MESSAGE,
                stacktrace = e.StackTrace ?? DEFAULT_STACKTRACE
            }));
        }

        public async void Create(PusherException e) {
            var response = new HttpClientResponse();
            await response.Convert(HttpClientRequestService.Create("app-errors", new {
                error_id = GetErrorId(e),
                message = e.Message ?? DEFAULT_MESSAGE + " [PUSHER ERROR]",
                stacktrace = e.StackTrace ?? DEFAULT_STACKTRACE + " [PUSHER ERROR]"
            }));
        }

        private int GetErrorId(Exception e) {
            int errorCode = 137;

            if(e is Win32Exception) {
                var w32ex = e as Win32Exception;
                if(w32ex == null && e.InnerException != null) {
                    w32ex = e.InnerException as Win32Exception;
                    errorCode = w32ex.ErrorCode;
                }
            }

            return errorCode;
        }
    }
}
