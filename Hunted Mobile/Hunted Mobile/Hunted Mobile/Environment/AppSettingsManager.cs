using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Hunted_Mobile {
    // https://www.andrewhoefling.com/Blog/Post/xamarin-app-configuration-control-your-app-settings
    public class AppSettingsManager {
        private const string NAMESPACE = "Hunted_Mobile";
        private const string FILE_NAME = "appsettings.json";

        private static AppSettingsManager instance;
        private readonly JObject secrets;
        private readonly Dictionary<string[], string> retrievedValues;

        private AppSettingsManager() {
            retrievedValues = new Dictionary<string[], string>();
            try {
                var assembly = IntrospectionExtensions.GetTypeInfo(typeof(AppSettingsManager)).Assembly;
                var stream = assembly.GetManifestResourceStream($"{NAMESPACE}.{FILE_NAME}");
                using(var reader = new StreamReader(stream)) {
                    var json = reader.ReadToEnd();
                    secrets = JObject.Parse(json);
                }
            }
            catch(Exception) {
                Debug.WriteLine("Unable to load secrets file");
            }
        }

        public static AppSettingsManager Instance {
            get {
                if(instance == null) {
                    instance = new AppSettingsManager();
                }

                return instance;
            }
        }

        public string GetValue(string[] nestedKey) {
            if(retrievedValues.ContainsKey(nestedKey)) {
                return retrievedValues[nestedKey];
            }
            else try {
                object jValue = secrets;
                foreach(string keyPart in nestedKey) {
                    if(jValue is JObject) {
                        jValue = ((JObject) jValue).GetValue(keyPart);
                    }
                }
                retrievedValues.Add(nestedKey, jValue?.ToString());
                return GetValue(nestedKey);
            }
            catch(Exception) {
                Debug.WriteLine($"Unable to retrieve secret '{nestedKey}'");
                return string.Empty;
            }
        }

        public string GetValue(string key) {
            if(retrievedValues.ContainsKey(new string[] { key })) {
                return retrievedValues[new string[] { key }];
            }
            else try {
                var path = key.Split(':');

                JToken node = secrets[path[0]];
                for(int index = 1; index < path.Length; index++) {
                    node = node[path[index]];
                }

                retrievedValues.Add(new string[] { key }, node.ToString());
                return GetValue(key);
            }
            catch(Exception) {
                Debug.WriteLine($"Unable to retrieve secret '{key}'");
                return string.Empty;
            }
        }
    }
}