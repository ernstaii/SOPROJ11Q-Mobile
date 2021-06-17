using System;

using Xamarin.Essentials;

namespace Hunted_Mobile.Service {
    public abstract class PreferenceService {
        protected void Set(string key, string value) {
            Preferences.Set(key, value);
        }

        protected string Get(string key) {
            string value = "";
            try {
                value = Preferences.Get(key, "");
            }
            catch(Exception e) { }

            return value;
        }

        protected int GetInt(string key) {
            int.TryParse(Get(key), out int value);

            return value;
        }

        public static void Remove(string key) {
            Preferences.Remove(key);
        }

        public static void Clear() {
            Preferences.Clear();
        }
    }
}
