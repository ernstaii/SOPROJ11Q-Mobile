namespace Hunted_Mobile.Service.Preference {
    public class GameSessionPreference : PreferenceService {
        private const string GAME_KEY = "game_key",
            PLAYER_KEY = "player_key";

        public void SetGame(int gameId) {
            Set(GAME_KEY, gameId.ToString());
        }

        public void SetUser(int userId) {
            Set(PLAYER_KEY, userId.ToString());
        }

        public int GetGame() {
            return GetInt(GAME_KEY);
        }

        public int GetUser() {
            return GetInt(PLAYER_KEY);
        }

        public void ClearUserAndGame() {
            Remove(GAME_KEY);
            Remove(PLAYER_KEY);
        }
    }
}
