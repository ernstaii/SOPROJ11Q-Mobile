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
            model = await inviteKeyRepository.Get(inviteKey);
        }

        public bool IsValid => model.GameId != null;
    }
}
