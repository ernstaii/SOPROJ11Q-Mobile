using PusherClient;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hunted_Mobile.Service {
    /// <summary>
    /// Represents a single socket connection to our api that will be reused during the runtime of the application
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

        public static bool Connected { get; private set; }

        static WebSocketService() {
            _pusher.ConnectionStateChanged += ConnectionStateChanged;
            _pusher.Error += ErrorOccured;
        }

        private static void ErrorOccured(object sender, PusherException error) {
            throw error;
        }

        private static void ConnectionStateChanged(object sender, ConnectionState state) {
            switch(state) {
                case ConnectionState.Connected:
                    Connected = true;
                    break;
                case ConnectionState.Disconnecting:
                    Connected = true;
                    break;
                default:
                    Connected = false;
                    break;
            }
        }
        #endregion

        public delegate void SocketEvent();
        public event SocketEvent StartGame;

        public WebSocketService(int gameId) {
            _pusher.SubscribeAsync("game." + gameId);

            _pusher.Bind("startGame", (obj) => StartGame());
        }

        public async Task Connect() {
            await _pusher.ConnectAsync();
        }

        public async Task Disconnect() {
            await _pusher.DisconnectAsync();
        }
    }
}
