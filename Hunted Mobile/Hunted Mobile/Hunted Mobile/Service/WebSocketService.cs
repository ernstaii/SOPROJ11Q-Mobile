using Hunted_Mobile.Model.Response;
using Hunted_Mobile.Service.Json;

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
            Console.WriteLine(error.ToString());
        }

        /// <summary>
        /// Updates the Connected property when the connection state changes
        /// </summary>
        private static void ConnectionStateChanged(object sender, ConnectionState state) {
            Connected = state == ConnectionState.Connected
                || state == ConnectionState.Disconnecting;
        }
        #endregion

        private readonly string gameIdString;

        public delegate void SocketEvent();
        public delegate void SocketEvent<T>(T data);

        public event SocketEvent StartGame;
        public event SocketEvent<JObject> PauseGame;
        public event SocketEvent<JObject> ResumeGame;
        public event SocketEvent<JObject> NotificationEvent;
        public event SocketEvent<JObject> EndGame;
        public event SocketEvent<IntervalEventData> IntervalEvent;
        public event SocketEvent<JObject> ThiefCaught;
        public event SocketEvent<JObject> ThiefReleased;
        public event SocketEvent<JObject> PlayerJoined;

        public WebSocketService(int gameId) {
            gameIdString = gameId.ToString();
            string channelName = "game." + gameIdString;
            
            var channel = pusher.GetChannel(channelName);
            if(channel == null || !channel.IsSubscribed) {
                pusher.SubscribeAsync(channelName);
            }

            Bind("game.start", () => StartGame());
            Bind<JObject>("game.pause", (data) => PauseGame(data));
            Bind<JObject>("game.resume", (data) => ResumeGame(data));
            Bind<JObject>("game.notification", (data) => NotificationEvent(data));
            Bind<JObject>("game.end", (data) => EndGame(data));
            Bind("game.interval", (json) => InvokeEvent(IntervalEvent, new IntervalEventJsonService().ToObject(json)));
            Bind<JObject>("thief.caught", (data) => ThiefCaught(data));
            Bind<JObject>("thief.released", (data) => ThiefReleased(data));
            Bind<JObject>("player.joined", (data) => PlayerJoined(data));
        }

        private void InvokeEvent<T>(SocketEvent<T> @event, T data) {
            if(@event != null) {
                @event(data);
            }
        }

        private void Bind(string eventName, Action action) {
            pusher.Bind(eventName, (PusherEvent eventData) => {
                if(eventData.ChannelName.EndsWith(gameIdString)) {
                    action();
                }
            });
        }

        private void Bind(string eventName, Action<string> action) {
            pusher.Bind(eventName, (PusherEvent eventData) => {
                if(eventData.ChannelName.EndsWith(gameIdString)) {
                    action(eventData.Data);
                }
            });
        }

        private void Bind<T>(string eventName, Action<T> action) {
            pusher.Bind(eventName, (PusherEvent eventData) => {
                T data = default;
                try {
                    if(eventData.ChannelName.EndsWith(gameIdString)) {
                        data = JsonConvert.DeserializeObject<T>(eventData.Data);
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
