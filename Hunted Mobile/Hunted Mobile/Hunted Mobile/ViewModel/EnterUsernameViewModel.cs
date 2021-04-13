using Hunted_Mobile.Model;
using Hunted_Mobile.Model.GameModels;
using Hunted_Mobile.Repository;

using System;
using System.Threading.Tasks;

namespace Hunted_Mobile.ViewModel {
    public class EnterUsernameViewModel {
        private User _model { get; set; }
        private UserRepository _userRepository = new UserRepository();

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

        public EnterUsernameViewModel(User user = null) {
            _model = user ?? new User();
        }

        // Add new user to a game
        public async Task CreateUser() {
            if(this.HasValidUserName) {
                var result = await _userRepository.Create(_model.InviteKey, this.UserName);

                CreatingUserSucceeded = result != null;

                if(CreatingUserSucceeded)
                    _model = result;
            }
        }

        // TODO: Create a globa abstract or interface for implementing validation in every models
        // Idea: https://docs.microsoft.com/en-us/xamarin/xamarin-forms/enterprise-application-patterns/validation
        public bool HasValidUserName => _model != null && _model.Name != null && _model.Name.Length >= 3 && _model.Name.Length <= 30;
        public bool CreatingUserSucceeded { get; set; }
    }
}
