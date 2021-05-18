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
            userModel = new Player() {
                InviteKey = key,
            };
        }

        /// <summary>
        /// Add new user to a game
        /// </summary>
        /// <returns></returns>
        public async Task CreateUser() {
            if(IsValid = ValidationHelper.IsFormValid(UserModel, page)) {
                UserModel = await UnitOfWork.Instance.UserRepository.Create(UserModel);
            }
        }

        /// <summary>
        /// Button event will navigate to the lobby with a new user
        /// </summary>
        public ICommand ButtonSelectedCommand => new Command(async (e) => {
            SubmitButtonIsEnable = false;
            await CreateUser();

            // Navigate when InviteKey is valid
            if(IsValid = ValidationHelper.IsFormValid(UserModel, page)) {
                var navigation = Application.Current.MainPage.Navigation;

                var previousPage = navigation.NavigationStack.LastOrDefault();
                var view = new Lobby(new LobbyViewModel(UserModel));
                await navigation.PushAsync(view, true);
                navigation.RemovePage(previousPage);
            }

            SubmitButtonIsEnable = true;
        });
    }
}
