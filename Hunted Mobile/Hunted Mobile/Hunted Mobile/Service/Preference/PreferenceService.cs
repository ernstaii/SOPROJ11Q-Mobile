using Xamarin.Essentials;

namespace Hunted_Mobile.Service {
    public abstract class PreferenceService {
        protected void Set(string key, string value) {
            Preferences.Set(key, value);
        }

        protected string Get(string key) {
            return Preferences.Get(key, "");
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
