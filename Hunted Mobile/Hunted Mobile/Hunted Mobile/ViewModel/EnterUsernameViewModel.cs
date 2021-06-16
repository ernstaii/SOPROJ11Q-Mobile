using Hunted_Mobile.Model;
using Hunted_Mobile.Model.GameModels;
using Hunted_Mobile.Repository;
using Hunted_Mobile.Service;
using Hunted_Mobile.Service.Builder;
using Hunted_Mobile.Service.Preference;
using Hunted_Mobile.View;

using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Forms;

namespace Hunted_Mobile.ViewModel {
    public class EnterUsernameViewModel : BaseViewModel {
        private Player userModel;
        private bool isloading = false;
        private EnterUsername page;

        public bool IsValid { get; set; }

        public Player UserModel {
            get => userModel;
            set {
                userModel = value;
                OnPropertyChanged("UserModel");
            }
        }

        public bool SubmitButtonIsEnable {
            get => isloading;
            set {
                isloading = value;
                OnPropertyChanged("SubmitButtonIsEnable");
            }
        }

        public EnterUsername View { set => page = value; }

        public EnterUsernameViewModel(InviteKey key) {
            userModel = new PlayerBuilder().SetInviteKey(key).ToPlayer();
        }

        /// <summary>
        /// Button event will navigate to the lobby with a new user
        /// </summary>
        public ICommand HandleEnterUserNameCommand => new Command(async (e) => {
            SubmitButtonIsEnable = false;

            // First validation is for model validation
            if(Valid()) {
                await CreateUser();

                // Second validation is for displaying ServerErrors
                if(Valid()) await NavigateToLobby();
            }

            SubmitButtonIsEnable = true;
        });

        private async Task CreateUser() {
            UserModel = await UnitOfWork.Instance.UserRepository.Create(UserModel);
        }

        private bool Valid() {
            return IsValid = ValidationHelper.IsFormValid(UserModel, page);
        }

        private async Task NavigateToLobby() {
            var navigation = Application.Current.MainPage.Navigation;

            var previousPage = navigation.NavigationStack.LastOrDefault();
            var view = new Lobby(new LobbyViewModel(UserModel));
            await navigation.PushAsync(view, true);
            navigation.RemovePage(previousPage);
        }
    }
}
