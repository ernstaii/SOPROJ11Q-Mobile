using Hunted_Mobile.Model;
using Hunted_Mobile.Model.GameModels;
using Hunted_Mobile.Repository;
using Hunted_Mobile.Service;
using Hunted_Mobile.View;

using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Forms;

namespace Hunted_Mobile.ViewModel {
    public class EnterUsernameViewModel {
        private User _userModel = new User();
        private bool _isloading = false;
        private bool _creatingUserSucceeded { get; set; }
        public bool IsValid { get; set; }

        public User UserModel {
            get => _userModel;
            set {
                _userModel = value;

                if(PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("UserModel"));
            }
        }

        public bool SubmitButtonIsEnable {
            get => _isloading;
            set {
                _isloading = value;

                if(PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("SubmitButtonIsEnable"));
            }
        }

        private UserRepository _userRepository = new UserRepository();
        private EnterUsername _page;

        public event PropertyChangedEventHandler PropertyChanged;

        public EnterUsernameViewModel(EnterUsername page, InviteKey key) {
            _page = page;
            UserModel.InviteKey = key;
        }

        // Add new user to a game
        public async Task CreateUser() {
            if(IsValid = ValidationHelper.IsFormValid(UserModel, _page)) {
                var result = await _userRepository.Create(UserModel.InviteKey, this.UserModel.Name);

                _creatingUserSucceeded = result != null;

                if(_creatingUserSucceeded)
                    UserModel = result;
            }
        }

        public ICommand ButtonSelectedCommand => new Command(async (e) => {
            SubmitButtonIsEnable = false;
            await CreateUser();

            // Navigate when InviteKey is valid
            if(_creatingUserSucceeded) {
                var Navigation = Xamarin.Forms.Application.Current.MainPage.Navigation;

                var previousPage = Navigation.NavigationStack.LastOrDefault();
                await Navigation.PushAsync(new Lobby(this), true);
                Navigation.RemovePage(previousPage);
            }

            SubmitButtonIsEnable = true;
        });
    }
}
