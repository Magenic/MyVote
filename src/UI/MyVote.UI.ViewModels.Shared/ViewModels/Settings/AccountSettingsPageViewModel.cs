using MyVote.UI.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using MvvmCross.Core.ViewModels;

namespace MyVote.UI.ViewModels.Settings
{
    public sealed class AccountSettingsPageViewModel : ViewModelBase
    {
		private readonly IAppSettings appSettings;

		public AccountSettingsPageViewModel(
			IAppSettings appSettings)
		{
			this.appSettings = appSettings;
		}

		public ICommand LogOut
		{
			get
			{
				return new MvxCommand(() => this.ExecuteLogOut());
			}
		}

		public void ExecuteLogOut()
		{
			this.appSettings.Remove(SettingsKeys.ProfileId);
			Csla.ApplicationContext.User = null;
			// TODO: Implement custom presenter with ability to reset the
			//		 view stack and show the landing page.
			//this.ChangePresentation(new LogOutHint());
		}
    }
}
