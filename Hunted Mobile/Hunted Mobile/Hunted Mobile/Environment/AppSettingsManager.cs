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

        private static AppSettingsManager instance = new AppSettingsManager();
        private readonly JObject secrets;
        private readonly Dictionary<string, string> retrievedValues;

        private AppSettingsManager() {
            retrievedValues = new Dictionary<string, string>();
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

        public static AppSettingsManager Instance => instance;

        private string GetRetrievedValuesKey(string[] nestedKey) {
            return string.Join(":", nestedKey);
        }

        public string GetValue(string[] nestedKey) {
            string key = GetRetrievedValuesKey(nestedKey);
            if(retrievedValues.ContainsKey(key)) {
                return retrievedValues[key];
            }
            else try {
                object jValue = secrets;
                foreach(string keyPart in nestedKey) {
                    if(jValue is JObject) {
                        jValue = ((JObject) jValue).GetValue(keyPart);
                    }
                }
                retrievedValues.Add(key, jValue?.ToString());
                return GetValue(nestedKey);
            }
            catch(Exception) {
                Debug.WriteLine($"Unable to retrieve secret '{nestedKey}'");
                return string.Empty;
            }
        }

        public string GetValue(string key) {
            return GetValue(new string[] { key });
        }
    }
}
