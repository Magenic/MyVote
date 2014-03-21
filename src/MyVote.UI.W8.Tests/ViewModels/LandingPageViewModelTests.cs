using Caliburn.Micro;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using MyVote.UI.NavigationCriteria;
using MyVote.UI.W8.Tests.Mocks;
using MyVote.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyVote.UI.Helpers;
using MyVote.BusinessObjects.Contracts;
using Csla;
using MyVote.UI.Services;

namespace MyVote.UI.W8.Tests.ViewModels
{
	[TestClass]
	public sealed class LandingPageViewModelTests
	{
		private NavigationMock Navigation { get; set; }
		private MessageBoxMock MessageBox { get; set; }
		private MobileServiceMock AuthenticationService { get; set; }
		private ObjectFactoryMock<IUserIdentity> ObjectFactory { get; set; }
		private AppSettingsMock AppSettings { get; set; }

		private LandingPageViewModel GetViewModel()
		{
			return new LandingPageViewModel(Navigation, MessageBox, AuthenticationService, ObjectFactory, AppSettings);
		}

		[TestInitialize]
		public void Init()
		{
			Navigation = new NavigationMock();
			MessageBox = new MessageBoxMock();
			AuthenticationService = new MobileServiceMock();
			ObjectFactory = new ObjectFactoryMock<IUserIdentity>();
			AppSettings = new AppSettingsMock();

			ViewLocator.LocateTypeForModelType = (modelType, displayLocation, context) =>
			{
				return modelType;
			};
		}

		[TestCleanup]
		public void Cleanup()
		{
			ViewLocator.LocateTypeForModelType = null;
		}

		[TestMethod]
		public async Task SignInWithTwitter()
		{
			// Arrange
			var viewModel = GetViewModel();

			var userId = Guid.NewGuid().ToString();

			AuthenticationProvider? actualProvider = null;
			AuthenticationService.AuthenticateWithProviderDelegate = ((provider) =>
				{
					actualProvider = provider;
					return userId;
				});

			Type actualType = null;
			RegistrationPageNavigationCriteria actualParam = null;
			Navigation.NavigateToViewModelWithParameterDelegate = ((type, param) =>
				{
					actualType = type;
					actualParam = (RegistrationPageNavigationCriteria)param;
				});

			var identity = new UserIdentityMock { IsAuthenticated = false };
			this.ObjectFactory.FetchAsyncWithCriteriaDelegate = (profileId) =>
				{
					return identity;
				};

			// Act
			await viewModel.SignInWithTwitter();

			// Assert
			Assert.AreEqual(AuthenticationProvider.Twitter, actualProvider, "Provider");
			Assert.AreEqual(typeof(RegistrationPageViewModel), actualType, "Navigation Type");
			Assert.AreEqual(userId, actualParam.ProfileId, "Profile Id");
		}

		[TestMethod]
		public async Task SignInWithFacebook()
		{
			// Arrange
			var viewModel = GetViewModel();

			var userId = Guid.NewGuid().ToString();

			AuthenticationProvider? actualProvider = null;
			AuthenticationService.AuthenticateWithProviderDelegate = ((provider) =>
			{
				actualProvider = provider;
				return userId;
			});

			Type actualType = null;
			RegistrationPageNavigationCriteria actualParam = null;
			Navigation.NavigateToViewModelWithParameterDelegate = ((type, param) =>
			{
				actualType = type;
				actualParam = (RegistrationPageNavigationCriteria)param;
			});

			var identity = new UserIdentityMock { IsAuthenticated = false };
			this.ObjectFactory.FetchAsyncWithCriteriaDelegate = (profileId) =>
			{
				return identity;
			};

			// Act
			await viewModel.SignInWithFacebook();

			// Assert
			Assert.AreEqual(AuthenticationProvider.Facebook, actualProvider, "Provider");
			Assert.AreEqual(typeof(RegistrationPageViewModel), actualType, "Type");
			Assert.AreEqual(userId, actualParam.ProfileId, "Profile Id");
		}

		[TestMethod]
		public async Task SignInWithMicrosoft()
		{
			// Arrange
			var viewModel = GetViewModel();

			var userId = Guid.NewGuid().ToString();

			AuthenticationProvider? actualProvider = null;
			AuthenticationService.AuthenticateWithProviderDelegate = ((provider) =>
			{
				actualProvider = provider;
				return userId;
			});

			Type actualType = null;
			RegistrationPageNavigationCriteria actualParam = null;
			Navigation.NavigateToViewModelWithParameterDelegate = ((type, param) =>
			{
				actualType = type;
				actualParam = (RegistrationPageNavigationCriteria)param;
			});

			var identity = new UserIdentityMock { IsAuthenticated = false };
			this.ObjectFactory.FetchAsyncWithCriteriaDelegate = (profileId) =>
			{
				return identity;
			};

			// Act
			await viewModel.SignInWithMicrosoft();

			// Assert
			Assert.AreEqual(AuthenticationProvider.Microsoft, actualProvider, "Provider");
			Assert.AreEqual(typeof(RegistrationPageViewModel), actualType, "Type");
			Assert.AreEqual(userId, actualParam.ProfileId, "Profile Id");
		}

		[TestMethod]
		public async Task SignInWithGoogle()
		{
			// Arrange
			var viewModel = GetViewModel();

			var userId = Guid.NewGuid().ToString();

			AuthenticationProvider? actualProvider = null;
			AuthenticationService.AuthenticateWithProviderDelegate = ((provider) =>
			{
				actualProvider = provider;
				return userId;
			});

			Type actualType = null;
			RegistrationPageNavigationCriteria actualParam = null;
			Navigation.NavigateToViewModelWithParameterDelegate = ((type, param) =>
			{
				actualType = type;
				actualParam = (RegistrationPageNavigationCriteria)param;
			});

			var identity = new UserIdentityMock { IsAuthenticated = false };
			this.ObjectFactory.FetchAsyncWithCriteriaDelegate = (profileId) =>
			{
				return identity;
			};

			// Act
			await viewModel.SignInWithGoogle();

			// Assert
			Assert.AreEqual(AuthenticationProvider.Google, actualProvider, "Provider");
			Assert.AreEqual(typeof(RegistrationPageViewModel), actualType, "Type");
			Assert.AreEqual(userId, actualParam.ProfileId, "Profile Id");
		}

		[TestMethod]
		public async Task SignInProfileExists()
		{
			// Arrange
			var viewModel = GetViewModel();

			var userId = Guid.NewGuid().ToString();

			AuthenticationProvider? actualProvider = null;
			AuthenticationService.AuthenticateWithProviderDelegate = ((provider) =>
			{
				actualProvider = provider;
				return userId;
			});

			Type actualType = null;
			Navigation.NavigateToViewModelDelegate = ((type) =>
			{
				actualType = type;
			});

			var identity = new UserIdentityMock { IsAuthenticated = true };
			this.ObjectFactory.FetchAsyncWithCriteriaDelegate = (profileId) =>
			{
				return identity;
			};

			// Act
			await viewModel.SignInWithGoogle();

			// Assert
			Assert.AreEqual(AuthenticationProvider.Google, actualProvider, "Provider");
			Assert.AreEqual(typeof(PollsPageViewModel), actualType, "Type");
		}

		[TestMethod]
		public async Task SignInFailure()
		{
			// Arrange
			var viewModel = GetViewModel();

			AuthenticationService.AuthenticateWithProviderDelegate = ((provider) =>
			{
				throw new InvalidOperationException();
			});

			var messageBoxWasShown = false;
			MessageBox.ShowAsyncWithTitleDelegate = (content, title, buttons) =>
				{
					messageBoxWasShown = true;
					return true;
				};

			// Act
			await viewModel.SignInWithGoogle();

			// Assert
			Assert.IsTrue(messageBoxWasShown);
		}

		[TestMethod]
		public async Task SignInProfileError()
		{
			// Arrange
			var viewModel = GetViewModel();

			var userId = Guid.NewGuid().ToString();

			AuthenticationProvider? actualProvider = null;
			AuthenticationService.AuthenticateWithProviderDelegate = ((provider) =>
			{
				actualProvider = provider;
				return userId;
			});

			Type actualType = null;
			Navigation.NavigateToViewModelDelegate = ((type) =>
			{
				actualType = type;
			});

			this.ObjectFactory.FetchAsyncWithCriteriaDelegate = (profileId) =>
			{
				throw new DataPortalException(string.Empty, new UserIdentityMock());
			};

			var messageBoxWasShown = false;
			MessageBox.ShowAsyncWithTitleDelegate = (content, title, buttons) =>
			{
				messageBoxWasShown = true;
				return true;
			};

			// Act
			await viewModel.SignInWithGoogle();

			// Assert
			Assert.IsTrue(messageBoxWasShown);
		}

		[TestMethod]
		public void AutoSignInProfileExists()
		{
			// Arrange
			var viewModel = GetViewModel();

			var profileId = Guid.NewGuid().ToString();
			AppSettings.TryGetValueDelegate = (key, value) =>
				{
					value = profileId;
					return true;
				};

			Type actualType = null;
			Navigation.NavigateToViewModelDelegate = ((type) =>
			{
				actualType = type;
			});

			var identity = new UserIdentityMock { IsAuthenticated = true };
			this.ObjectFactory.FetchAsyncWithCriteriaDelegate = (profileIdParam) =>
			{
				return identity;
			};

			// Act
			((IActivate)viewModel).Activate();

			// Assert
			Assert.AreEqual(typeof(PollsPageViewModel), actualType, "Type");
		}

		[TestMethod]
		public void AutoSignInProfileDoesNotExist()
		{
			// Arrange
			var viewModel = GetViewModel();

			AppSettings.TryGetValueDelegate = (key, value) =>
			{
				return false;
			};

			Type actualType = null;
			Navigation.NavigateToViewModelDelegate = ((type) =>
			{
				actualType = type;
			});

			var identity = new UserIdentityMock { IsAuthenticated = true };
			this.ObjectFactory.FetchAsyncWithCriteriaDelegate = (profileIdParam) =>
			{
				return identity;
			};

			// Act
			((IActivate)viewModel).Activate();

			// Assert
			Assert.IsNull(actualType, "Type");
		}
	}
}
