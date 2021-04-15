using System;
using System.Collections.Generic;
using System.Text;

namespace Hunted_Mobile.Service {
    public class HttpClientService {
        private const string IPAdress = "http://soproj11q.herokuapp.com";

        public static string GetUrl(string path) {
            return $"{IPAdress}/api/{path}";
        }
    }
}
