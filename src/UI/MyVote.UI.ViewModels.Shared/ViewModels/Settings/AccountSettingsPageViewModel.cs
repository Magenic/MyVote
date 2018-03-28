using MyVote.UI.Helpers;
using System.Windows.Input;
using Csla.Security;
using MyVote.UI.Contracts;

namespace MyVote.UI.ViewModels.Settings
{
    public sealed class AccountSettingsPageViewModel : ViewModelBase
    {
		private readonly IAppSettings appSettings;
        private readonly INavigationService navigationService;

		public AccountSettingsPageViewModel(
			IAppSettings appSettings,
            INavigationService navigationService)
		{
			this.appSettings = appSettings;
            this.navigationService = navigationService;
		}

		public ICommand LogOut
		{
			get
			{
				return new Command(() => ExecuteLogOut());
			}
		}

		public void ExecuteLogOut()
		{
			this.appSettings.Remove(SettingsKeys.ProfileId);
			Csla.ApplicationContext.User = new UnauthenticatedPrincipal(); ;
            navigationService.ShowViewModel<LandingPageViewModel>();
			navigationService.ChangePresentation(new ClearBackstackHint());
		}
    }
}
