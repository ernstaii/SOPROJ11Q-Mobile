using Hunted_Mobile.Model.Response;
using Hunted_Mobile.Repository;
using Hunted_Mobile.Service.Json;

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

        private static string subscribedChannel;

        /// <summary>
        /// Whether or not the socket connection to the API is currently connected
        /// </summary>
        public static bool Online { get; private set; }

        // Static initializer, executed once during the first usage of the class
        static WebSocketService() {
            pusher.ConnectionStateChanged += ConnectionStateChanged;
            pusher.Error += ErrorOccurred;
        }

        private static void ErrorOccurred(object sender, PusherException error) {
            if(error is EventEmitterActionException<PusherEvent>) {
                DependencyService.Get<Toast>().Show("(#11) Error in event " + ((EventEmitterActionException<PusherEvent>) error).EventName + " (WebSocketService)");
            }
            else {
                DependencyService.Get<Toast>().Show("(#11) Er was een probleem met de pusher (WebSocketService)");
            }
            UnitOfWork.Instance.ErrorRepository.Create(error);
        }

        /// <summary>
        /// Updates the Connected property when the connection state changes
        /// </summary>
        private static void ConnectionStateChanged(object sender, ConnectionState state) {
            if(state == ConnectionState.Disconnected && Online) {
                DependencyService.Get<Toast>().Show("De socket verbinding wordt hersteld");
                pusher.ConnectAsync();
            }
            else if(state == ConnectionState.Connected && !Online) {
                DependencyService.Get<Toast>().Show("De socket verbinding wordt verbroken");
                pusher.DisconnectAsync();
            }
        }
        #endregion

        private readonly string gameIdString;

        public delegate void SocketEvent();
        public delegate void SocketEvent<T>(T data) where T : EventData;

        public event SocketEvent<EventData> StartGame;
        public event SocketEvent<EventData> PauseGame;
        public event SocketEvent<EventData> ResumeGame;
        public event SocketEvent<EventData> NotificationEvent;
        public event SocketEvent<EventData> EndGame;
        public event SocketEvent<IntervalEventData> IntervalEvent;
        public event SocketEvent<PlayerEventData> ThiefCaught;
        public event SocketEvent<PlayerEventData> ThiefReleased;
        public event SocketEvent<PlayerEventData> PlayerJoined;
        public event SocketEvent<ScoreUpdatedEventData> ScoreUpdated;
        public event SocketEvent<GadgetsUpdatedEventData> GadgetsUpdated;
        public event SocketEvent<PlayerEventData> ThiefFakePoliceToggle;

        public WebSocketService(string gameId) {
            gameIdString = gameId;
            string channelName = "game." + gameIdString;

            if(!channelName.Equals(subscribedChannel)) {
                subscribedChannel = channelName;
                pusher.UnbindAll();
                pusher.UnsubscribeAllAsync().ContinueWith(new Action<Task>(
                    (task) => pusher.SubscribeAsync(channelName)
                ));
            }

            Bind("game.start", (json) => InvokeEvent(StartGame, new EventJsonService().ToObject(json)));
            Bind("game.pause", (json) => InvokeEvent(PauseGame, new EventJsonService().ToObject(json)));
            Bind("game.resume", (json) => InvokeEvent(ResumeGame, new EventJsonService().ToObject(json)));
            Bind("game.notification", (json) => InvokeEvent(NotificationEvent, new EventJsonService().ToObject(json)));
            Bind("game.end", (json) => InvokeEvent(EndGame, new EventJsonService().ToObject(json)));
            Bind("game.interval", (json) => InvokeEvent(IntervalEvent, new IntervalEventJsonService().ToObject(json)));
            Bind("thief.caught", (json) => InvokeEvent(ThiefCaught, new PlayerEventJsonService().ToObject(json)));
            Bind("thief.released", (json) => InvokeEvent(ThiefReleased, new PlayerEventJsonService().ToObject(json)));
            Bind("player.joined", (json) => InvokeEvent(PlayerJoined, new PlayerEventJsonService().ToObject(json)));
            Bind("score.updated", (json) => InvokeEvent(ScoreUpdated, new ScoreUpdatedEventJsonService().ToObject(json)));
            Bind("gadgets.update", (json) => InvokeEvent(GadgetsUpdated, new GadgetsUpdatedEventJsonService().ToObject(json)));
            Bind("thief.fakeAgent", (json) => InvokeEvent(ThiefFakePoliceToggle, new ThiefFakePoliceToggleEventJsonService().ToObject(json)));
        }

        private void InvokeEvent<T>(SocketEvent<T> @event, T data) where T : EventData {
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

        public async Task Connect() {
            Online = true;
            await pusher.ConnectAsync();
        }

        public async Task Disconnect() {
            Online = false;
            await pusher.DisconnectAsync();
        }
    }
}
