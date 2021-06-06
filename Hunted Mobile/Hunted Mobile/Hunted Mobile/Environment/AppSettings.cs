using System.Globalization;

namespace Hunted_Mobile {
    public readonly struct AppSettings {
        public static string WebAddress => AppSettingsManager.Instance.GetValue(new string[] { "web_service", "address" });

        public static string PusherKey => AppSettingsManager.Instance.GetValue(new string[] { "pusher", "key" });

        public static CultureInfo Locale => new CultureInfo("en-US");
    }
}
