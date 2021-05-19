using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using PusherClient;

using System;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Hunted_Mobile.Service {
    /// <summary>
    /// Represents a single socket connection to our API that will be reused during the runtime of the application
    /// The static initialization should enforce a single connection per running app
    /// </summary>
    public class WebSocketService {
        #region Static
        private static readonly Pusher pusher = new Pusher(
            AppSettings.PusherKey,
            new PusherOptions() {
                Cluster = "eu",
                Encrypted = true,
            }
        );

        /// <summary>
        /// Whether or not the socket connection to the API is currently connected
        /// </summary>
        public static bool Connected { get; private set; }

        // Static initializer, executed once during the first usage of the class
        static WebSocketService() {
            pusher.ConnectionStateChanged += ConnectionStateChanged;
            pusher.Error += ErrorOccurred;
        }

        private static void ErrorOccurred(object sender, PusherException error) {
            throw error;
        }

        /// <summary>
        /// Updates the Connected property when the connection state changes
        /// </summary>
        private static void ConnectionStateChanged(object sender, ConnectionState state) {
            Connected = state == ConnectionState.Connected
                || state == ConnectionState.Disconnecting;
        }
        #endregion

        public delegate void SocketEvent();
        public delegate void SocketEvent<T>(T data);

        public event SocketEvent StartGame;
        public event SocketEvent<JObject> PauseGame;
        public event SocketEvent<JObject> ResumeGame;
        public event SocketEvent<JObject> NotificationEvent;
        public event SocketEvent<JObject> EndGame;
        public event SocketEvent<JObject> IntervalEvent;
        public event SocketEvent<JObject> ThiefCaught;
        public event SocketEvent<JObject> ThiefReleased;
        public event SocketEvent<JObject> PlayerJoined;
        public event SocketEvent<JObject> ScoreUpdated;

        public WebSocketService(int gameId) {
            string gameIdStr = gameId.ToString();
            string channelName = "game." + gameIdStr;
            
            var channel = pusher.GetChannel(channelName);
            if(channel == null || !channel.IsSubscribed) {
                pusher.SubscribeAsync(channelName);
            }

            Bind("game.start", () => StartGame(), gameIdStr);
            Bind<JObject>("game.pause", (data) => PauseGame(data), gameIdStr);
            Bind<JObject>("game.resume", (data) => ResumeGame(data), gameIdStr);
            Bind<JObject>("game.notification", (data) => NotificationEvent(data), gameIdStr);
            Bind<JObject>("game.end", (data) => EndGame(data), gameIdStr);
            Bind<JObject>("game.interval", (data) => IntervalEvent(data), gameIdStr);
            Bind<JObject>("thief.caught", (data) => ThiefCaught(data), gameIdStr);
            Bind<JObject>("thief.released", (data) => ThiefReleased(data), gameIdStr);
            Bind<JObject>("player.joined", (data) => PlayerJoined(data), gameIdStr);
            Bind<JObject>("score.updated", (data) => ScoreUpdated(data), gameIdStr);
        }

        private void Bind(string eventName, Action action, string gameIdStr) {
            pusher.Bind(eventName, (PusherEvent eventData) => {
                if(eventData.ChannelName.EndsWith(gameIdStr)) {
                    action();
                }
            });
        }

        private void Bind<T>(string eventName, Action<T> action, string gameIdStr) {
            pusher.Bind(eventName, (PusherEvent eventData) => {
                try {
                    if(eventData.ChannelName.EndsWith(gameIdStr)) {
                        T data = JsonConvert.DeserializeObject<T>(eventData.Data);
                        action(data);
                    }
                }
                catch(Exception ex) {
                    DependencyService.Get<Toast>().Show("Er was een probleem met het deserialiseren van de event data");
                }
            });
        }

        public async Task Connect() {
            await pusher.ConnectAsync();
        }

        public async Task Disconnect() {
            await pusher.DisconnectAsync();
        }
    }
}
