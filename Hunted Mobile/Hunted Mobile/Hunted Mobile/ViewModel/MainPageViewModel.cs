using Hunted_Mobile.Model;
using Hunted_Mobile.Repository;
using Hunted_Mobile.Service;
using Hunted_Mobile.View;

using System;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Forms;

namespace Hunted_Mobile.ViewModel {
    public class MainPageViewModel : BaseViewModel {
        private InviteKey _inviteKeyModel = new InviteKey();
        private bool _isloading = false;
        public InviteKey InviteKeyModel {
            get => _inviteKeyModel;
            set {
                _inviteKeyModel = value;
                OnPropertyChanged("InviteKeyModel");
            }
        }

        public bool SubmitButtonIsEnable {
            get => _isloading;
            set {
                _isloading = value;
                OnPropertyChanged("SubmitButtonIsEnable");
            }
        }

        private InviteKeyRepository _inviteKeyRepository = new InviteKeyRepository();
        private MainPage _page;

        public bool IsValid { get; set; }

        public MainPageViewModel(MainPage page) {
            _page = page;
        }

        // Getting InviteKey based on the Value
        public async Task Get() {
            if(IsValid = ValidationHelper.IsFormValid(InviteKeyModel, _page)) {
                InviteKeyModel = await _inviteKeyRepository.Get(InviteKeyModel.Value);
            }
        }

        // When user tries to validate his InviteKey
        public ICommand ButtonSelectedCommand => new Command(async (e) => {
            SubmitButtonIsEnable = false;
            await Get();

            // Navigate when InviteKey is valid
            if(IsValid = ValidationHelper.IsFormValid(InviteKeyModel, _page)) {
                await Xamarin.Forms.Application.Current.MainPage.Navigation.PushAsync(new EnterUsername(InviteKeyModel));
            }

            SubmitButtonIsEnable = true;
        });
    }
}
