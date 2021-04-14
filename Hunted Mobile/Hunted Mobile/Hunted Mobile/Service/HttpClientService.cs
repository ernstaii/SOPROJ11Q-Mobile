using System;
using System.Collections.Generic;
using System.Text;

namespace Hunted_Mobile.Service {
    public class HttpClientService {
        private const String IPAdress = "http://192.168.1.104:8080";

        public static String GetUrl(string path) {
            return $"{IPAdress}/api/{path}";
        }
    }
}
