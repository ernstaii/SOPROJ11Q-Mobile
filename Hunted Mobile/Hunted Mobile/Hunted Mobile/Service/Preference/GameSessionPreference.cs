namespace Hunted_Mobile.Service.Preference {
    public class GameSessionPreference : PreferenceService {
        private const string GAME_KEY = "game_key",
            PLAYER_KEY = "player_key",
            INFORMATION_KEY = "information_key",
            INFORMATION_STATE = "seen";

        public void SetGame(int gameId) {
            Set(GAME_KEY, gameId.ToString());
        }

        public void SetUser(int userId) {
            Set(PLAYER_KEY, userId.ToString());
        }

        public void ToggleInformationState() {
            Set(INFORMATION_KEY, INFORMATION_STATE);
        }

        public int GetGame() {
            return GetInt(GAME_KEY);
        }

        public int GetUser() {
            return GetInt(PLAYER_KEY);
        }

        public bool HasSeenInformation() {
            return Get(INFORMATION_KEY) == INFORMATION_STATE;
        }

        public static void ClearUserAndGame() {
            Remove(GAME_KEY);
            Remove(PLAYER_KEY);
        }
    }
}
