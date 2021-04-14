using Newtonsoft.Json;

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
        private static readonly Pusher _pusher = new Pusher(
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
            _pusher.ConnectionStateChanged += ConnectionStateChanged;
            _pusher.Error += ErrorOccured;
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
        public event SocketEvent StartGame;

        public WebSocketService(int gameId) {
            _pusher.SubscribeAsync("game." + gameId);

            Bind("startGame", ()=> StartGame(), gameId);
        }

        private void Bind(string eventName, Action action, int gameId) {
            string gameIdStr = gameId.ToString();
            _pusher.Bind(eventName, (PusherEvent eventData) => {
                object data = JsonConvert.DeserializeObject<object>(eventData.Data);
                if(eventData.ChannelName.EndsWith(gameIdStr)) {
                    action();
                }
            });
        }

        public async Task Connect() {
            await _pusher.ConnectAsync();
        }

        public async Task Disconnect() {
            await _pusher.DisconnectAsync();
        }
    }
}
