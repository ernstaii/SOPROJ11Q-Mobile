using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using PusherClient;

using System;
using System.Threading.Tasks;

namespace Hunted_Mobile.Service {
    /// <summary>
    /// Represents a single socket connection to our API that will be reused during the runtime of the application
    /// The static initialization should enforce a single connection per running app
    /// </summary>
    public class WebSocketService {
        #region Static
        private static readonly Pusher pusher = new Pusher(
            "27357622ad22f596bba2",
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
            pusher.Error += ErrorOccured;
        }

        private static void ErrorOccured(object sender, PusherException error) {
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
        public event SocketEvent ResumeGame;
        public event SocketEvent<JObject> EndGame;
        public event SocketEvent<JObject> IntervalEvent;

        public WebSocketService(int gameId) {
            pusher.SubscribeAsync("game." + gameId);

            string gameIdStr = gameId.ToString();
            Bind("game.start", () => StartGame(), gameIdStr);
            Bind<JObject>("game.pause", (data) => PauseGame(data), gameIdStr);
            Bind("game.resume", () => ResumeGame(), gameIdStr);
            Bind<JObject>("game.end", (data) => EndGame(data), gameIdStr);
            Bind<JObject>("game.interval", (data) => IntervalEvent(data), gameIdStr);
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
                    Console.WriteLine("An error occurred while deserializing event data: " + ex.StackTrace);
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
