using Caliburn.Micro;
using MyVote.UI.Helpers;

namespace MyVote.UI.ViewModels.Settings
{
	public class AccountSettingsViewModel : PageViewModelBase
	{
		private readonly IAppSettings appSettings;

		public AccountSettingsViewModel(
			INavigation navigation,
			IAppSettings appSettings)
			: base(navigation)
		{
			this.appSettings = appSettings;
		}

		public void LogOff()
		{
			this.appSettings.Remove(SettingsKeys.ProfileId);
			Csla.ApplicationContext.User = null;
			while (this.Navigation.CanGoBack)
			{
				this.Navigation.GoBack();
			}
		}
	}
}
