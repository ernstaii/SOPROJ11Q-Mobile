using Hunted_Mobile.Model;
using Hunted_Mobile.Repository;

using System.Threading.Tasks;

namespace Hunted_Mobile.ViewModel {
    public class InviteKeyViewModel {
        public InviteKey model;
        private InviteKeyRepository inviteKeyRepository = new InviteKeyRepository();

        public InviteKeyViewModel() {
            model = new InviteKey();
        }

        public async Task Get(string inviteKey) {
            if(inviteKey != null && inviteKey.Length >= 2) {
                var result = await inviteKeyRepository.Get(inviteKey);

                // TODO: Here you would like to do result.IsValid
                if(result != null)
                    model = result;
            }
        }

        public bool IsValid => model.Role != null && model.Role.Length > 0 && model.GameId > 0;
    }
}
