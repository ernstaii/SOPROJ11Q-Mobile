using Hunted_Mobile.Model;
using Hunted_Mobile.Repository;

using System.Threading.Tasks;

namespace Hunted_Mobile.ViewModel {
    public class MainPageViewModel {
        public InviteKey Model;
        private InviteKeyRepository _inviteKeyRepository = new InviteKeyRepository();

        public MainPageViewModel() {
            Model = new InviteKey();
        }

        public async Task Get(string inviteKey) {
            if(inviteKey != null && inviteKey.Length >= 2) {
                var result = await _inviteKeyRepository.Get(inviteKey);

                if(result != null)
                    Model = result;
            }
        }

        public bool IsValid => Model.Role != null && Model.Role.Length > 0 && Model.GameId > 0;
    }
}
