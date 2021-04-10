using Hunted_Mobile.Model;
using Hunted_Mobile.Model.GameModels;
using Hunted_Mobile.Repository;

using System;
using System.Threading.Tasks;

namespace Hunted_Mobile.ViewModel {
    public class UserViewModel {
        private User _model { get; set; }
        public UserRepository userRepository = new UserRepository();

        public string UserName {
            get => _model.Name;
            set {
                _model.Name = value;
            }
        }

        public InviteKey InviteKey {
            get => _model.InviteKey;
            set {
                _model.InviteKey = value;
            }
        }

        public UserViewModel(User user = null) {
            _model = user ?? new User();
        }

        // Add new user to a game
        public async Task CreateUser() {
            if(this.IsValid) {

                // Creating a user with the values
                _model = await userRepository.Create(_model.InviteKey, this.UserName);
            }
        }

        // TODO: Create a globa abstract or interface for implementing validation in every models
        public bool IsValid => _model.Name != null && _model.Name.Length >= 3 && _model.Name.Length <= 30;
    }
}
