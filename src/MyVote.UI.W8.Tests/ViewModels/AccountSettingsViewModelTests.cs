using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using MyVote.UI.Helpers;
using MyVote.UI.ViewModels.Settings;
using MyVote.UI.W8.Tests.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVote.UI.W8.Tests.ViewModels
{
	[TestClass]
	public class AccountSettingsViewModelTests
	{
		private NavigationMock Navigation { get; set; }
		private AppSettingsMock AppSettings { get; set; }

		private AccountSettingsViewModel GetViewModel()
		{
			return new AccountSettingsViewModel(Navigation, AppSettings);
		}

		[TestInitialize]
		public void Init()
		{
			Navigation = new NavigationMock();
			AppSettings = new AppSettingsMock();
		}

		[TestMethod]
		public void LogOff()
		{
			// Arrange
			var viewModel = GetViewModel();

			var didGoBack = false;
			Navigation.CanGoBack = true;
			Navigation.GoBackDelegate = () =>
				{
					didGoBack = true;
					Navigation.CanGoBack = false;
				};

			var actualKey = string.Empty;
			AppSettings.RemoveDelegate = (key) =>
				{
					actualKey = key;
					return true;
				};

			// Act
			viewModel.LogOff();

			// Assert
			Assert.IsTrue(didGoBack, "Did Go Back");
			Assert.AreEqual(SettingsKeys.ProfileId, actualKey, "Key");
		}
	}
}
