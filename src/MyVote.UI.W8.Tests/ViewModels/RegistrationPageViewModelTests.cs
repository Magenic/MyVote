using Caliburn.Micro;
using Csla;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using MyVote.BusinessObjects.Contracts;
using MyVote.UI.Helpers;
using MyVote.UI.ViewModels;
using MyVote.UI.W8.Tests.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyVote.UI.W8.Tests.ViewModels
{
	[TestClass]
	public class RegistrationPageViewModelTests
	{
		private NavigationMock Navigation { get; set; }
		private ObjectFactoryMock<IUser> ObjectFactory { get; set; }
		private ObjectFactoryMock<IUserIdentity> UserIdentityObjectFactory { get; set; }
		private MessageBoxMock MessageBox { get; set; }

		private RegistrationPageViewModel GetViewModel()
		{
			return new RegistrationPageViewModel(this.Navigation, this.ObjectFactory, this.UserIdentityObjectFactory, this.MessageBox);
		}

		[TestInitialize]
		public void Init()
		{
			Navigation = new NavigationMock();
			ObjectFactory = new ObjectFactoryMock<IUser>();
			UserIdentityObjectFactory = new ObjectFactoryMock<IUserIdentity>();
			MessageBox = new MessageBoxMock();

			ViewLocator.LocateTypeForModelType = (modelType, displayLocation, context) =>
			{
				return modelType;
			};
		}

		[TestCleanup]
		public void Cleanup()
		{
			ViewLocator.LocateTypeForModelType = null;
			Csla.ApplicationContext.User = null;
		}

		[TestMethod]
		public async Task CreateUserAsync()
		{
			// Arrange
			var viewModel = GetViewModel();

			var user = new UserMock();
			ObjectFactory.CreateAsyncWithCriteriaDelegate = (criteria) =>
			{
				return user;
			};

			viewModel.Parameter = Serializer.Serialize(new NavigationCriteria.RegistrationPageNavigationCriteria
			{
				ProfileId = Guid.NewGuid().ToString()
			});

			// Act
			await viewModel.CreateUserAsync();

			// Assert
			Assert.AreSame(user, viewModel.User);
		}

		[TestMethod]
		public async Task Submit()
		{
			// Arrange
			var viewModel = GetViewModel();

			var user = new UserMock { ProfileID = Guid.NewGuid().ToString() };
			ObjectFactory.CreateAsyncWithCriteriaDelegate = (criteria) =>
			{
				return user;
			};

			string actualProfileId = null;
			UserIdentityObjectFactory.FetchAsyncWithCriteriaDelegate = (profileId) =>
				{
					actualProfileId = (string)profileId;
					return new UserIdentityMock();
				};

			var userSaved = false;
			user.SaveAsyncDelegate = () =>
			{
				userSaved = true;
				return user;
			};

			Type actualType = null;
			Navigation.NavigateToViewModelDelegate = ((type) =>
			{
				actualType = type;
			});

			viewModel.Parameter = Serializer.Serialize(new NavigationCriteria.RegistrationPageNavigationCriteria
			{
				ProfileId = Guid.NewGuid().ToString()
			});

			((IActivate)viewModel).Activate();

			// Act
			await viewModel.Submit();

			// Assert
			Assert.IsTrue(userSaved, "User Saved");
			Assert.AreEqual(typeof(PollsPageViewModel), actualType, "Navigation Type");
			Assert.AreEqual(user.ProfileID, actualProfileId, "Loaded Profile Id");
			Assert.IsNotNull(Csla.ApplicationContext.User, "CSLA User");
		}

		[TestMethod]
		public async Task SubmitWithError()
		{
			// Arrange
			var viewModel = GetViewModel();

			var user = new UserMock();
			ObjectFactory.CreateAsyncWithCriteriaDelegate = (criteria) =>
			{
				return user;
			};

			user.SaveAsyncDelegate = () =>
			{
				throw new DataPortalException(string.Empty, new UserMock());
			};

			var navigated = false;
			Navigation.NavigateToViewModelWithParameterDelegate = ((type, param) =>
			{
				navigated = true;
			});

			var messageBoxShown = false;
			MessageBox.ShowAsyncWithTitleDelegate = (message, title, buttons) =>
			{
				messageBoxShown = true;
				return true;
			};

			viewModel.Parameter = Serializer.Serialize(new NavigationCriteria.RegistrationPageNavigationCriteria
			{
				ProfileId = Guid.NewGuid().ToString()
			});

			await viewModel.CreateUserAsync();

			// Act
			await viewModel.Submit();

			// Assert
			Assert.IsFalse(navigated, "Navigated");
			Assert.IsTrue(messageBoxShown, "Message box shown.");
		}

		[TestMethod]
		public async Task CanSave()
		{
			// Arrange
			var viewModel = GetViewModel();

			var user = new UserMock();
			ObjectFactory.CreateAsyncWithCriteriaDelegate = (criteria) =>
			{
				return user;
			};

			user.IsSavable = true;

			viewModel.Parameter = Serializer.Serialize(new NavigationCriteria.RegistrationPageNavigationCriteria
			{
				ProfileId = Guid.NewGuid().ToString()
			});

			await viewModel.CreateUserAsync();

			// Act

			// Assert
			Assert.IsTrue(viewModel.CanSave, "Can Save");
		}
	}
}
