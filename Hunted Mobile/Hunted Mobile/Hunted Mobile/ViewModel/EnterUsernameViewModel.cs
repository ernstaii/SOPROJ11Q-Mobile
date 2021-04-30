using Hunted_Mobile.Model;
using Hunted_Mobile.Model.GameModels;
using Hunted_Mobile.Repository;
using Hunted_Mobile.Service;
using Hunted_Mobile.View;

using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Forms;

namespace Hunted_Mobile.ViewModel {
    public class EnterUsernameViewModel : BaseViewModel {
        private User _userModel = new User();
        private bool _isloading = false;
        private readonly UserRepository _userRepository = new UserRepository();
        private readonly EnterUsername _page;
        private bool _creatingUserSucceeded { get; set; }
        public bool IsValid { get; set; }

        public User UserModel {
            get => _userModel;
            set {
                _userModel = value;
                OnPropertyChanged("UserModel");
            }
        }

        public bool SubmitButtonIsEnable {
            get => _isloading;
            set {
                _isloading = value;
                OnPropertyChanged("SubmitButtonIsEnable");
            }
        }

        public EnterUsernameViewModel(EnterUsername page, InviteKey key) {
            _page = page;
            UserModel.InviteKey = key;
        }

        /// <summary>
        /// Add new user to a game
        /// </summary>
        /// <returns></returns>
        public async Task CreateUser() {
            if(IsValid = ValidationHelper.IsFormValid(UserModel, _page)) {
                UserModel = await _userRepository.Create(UserModel.InviteKey, UserModel.UserName);
            }
        }

        /// <summary>
        /// Button event will navigate to the lobby with a new user
        /// </summary>
        public ICommand ButtonSelectedCommand => new Command(async (e) => {
            SubmitButtonIsEnable = false;
            await CreateUser();

            // Navigate when InviteKey is valid
            if(IsValid = ValidationHelper.IsFormValid(UserModel, _page)) {
                var Navigation = Xamarin.Forms.Application.Current.MainPage.Navigation;

                var previousPage = Navigation.NavigationStack.LastOrDefault();
                await Navigation.PushAsync(new Lobby(UserModel), true);
                Navigation.RemovePage(previousPage);
            }

            SubmitButtonIsEnable = true;
        });
    }
}
