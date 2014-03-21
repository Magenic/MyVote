using Caliburn.Micro;
using Csla;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using MyVote.BusinessObjects;
using MyVote.BusinessObjects.Contracts;
using MyVote.UI.Helpers;
using MyVote.UI.NavigationCriteria;
using MyVote.UI.ViewModels;
using MyVote.UI.W8.Tests.Mocks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyVote.UI.W8.Tests.ViewModels
{
	[TestClass]
	public sealed class ViewPollPageViewModelTests
	{
		private NavigationMock Navigation { get; set; }
		private ObjectFactoryMock<IPollSubmissionCommand> ObjectFactory { get; set; }
		private ObjectFactoryMock<IPoll> PollFactory { get; set; }
		private MessageBoxMock MessageBox { get; set; }
		private ShareManagerMock ShareManager { get; set; }
		private SecondaryPinnerMock SecondaryPinner { get; set; }

		private ViewPollPageViewModel GetViewModel()
		{
			return new ViewPollPageViewModel(
				Navigation,
				ObjectFactory,
				PollFactory,
				MessageBox,
				ShareManager,
				SecondaryPinner);
		}

		private List<string> PropertiesChanged { get; set; }
		private void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
		{
			PropertiesChanged.Add(args.PropertyName);
		}

		[TestInitialize]
		public void Init()
		{
			Navigation = new NavigationMock();
			ObjectFactory = new ObjectFactoryMock<IPollSubmissionCommand>();
			PollFactory = new ObjectFactoryMock<IPoll>();
			MessageBox = new MessageBoxMock();
			ShareManager = new ShareManagerMock();
			SecondaryPinner = new SecondaryPinnerMock();

			PropertiesChanged = new List<string>();

			ViewLocator.LocateTypeForModelType = (modelType, displayLocation, context) =>
			{
				return modelType;
			};
		}

		[TestCleanup]
		public void Cleanup()
		{
			Csla.ApplicationContext.User = null;

			ViewLocator.LocateTypeForModelType = null;
		}

		[TestMethod]
		public async Task LoadPollAsync()
		{
			// Arrange
			var viewModel = GetViewModel();

			var random = new Random();

			var principal = new PrincipalMock();

			var identity = new UserIdentityMock();
			identity.UserID = random.Next();
			principal.Identity = identity;

			Csla.ApplicationContext.User = principal;

			var pollId = random.Next();

			viewModel.Parameter = Serializer.Serialize(
				new ViewPollPageNavigationCriteria { PollId = pollId });

			var pollSubmission = new PollSubmissionMock();
			IPollSubmissionCommand actualCommand = null;
			ObjectFactory.CreateAsyncDelegate = () => new PollSubmissionCommandMock();
			ObjectFactory.ExecuteAsyncDelegate = (command) =>
				{
					actualCommand = command;
					return new PollSubmissionCommandMock { Submission = pollSubmission };
				};

			var didNavigate = false;
			Navigation.NavigateToViewModelWithParameterDelegate = ((type, param) =>
			{
				didNavigate = true;
			});

			// Act
			await viewModel.LoadPollAsync();

			// Assert
			Assert.IsFalse(didNavigate, "Did Navigate");
			Assert.AreEqual(identity.UserID, actualCommand.UserID, "UserID");
			Assert.AreEqual(pollId, actualCommand.PollID, "PollID");
		}

		[TestMethod]
		public async Task LoadPollAsyncAlreadyVoted()
		{
			// Arrange
			var viewModel = GetViewModel();

			var random = new Random();

			var principal = new PrincipalMock();

			var identity = new UserIdentityMock();
			identity.UserID = random.Next();
			principal.Identity = identity;

			Csla.ApplicationContext.User = principal;

			var pollId = random.Next();

			viewModel.Parameter = Serializer.Serialize(
				new ViewPollPageNavigationCriteria { PollId = pollId });

			Type actualType = null;
			PollResultsPageNavigationCriteria actualParam = null;
			ObjectFactory.CreateAsyncDelegate = () => new PollSubmissionCommandMock();
			ObjectFactory.ExecuteAsyncDelegate = command => new PollSubmissionCommandMock();

			Navigation.NavigateToViewModelAndRemoveCurrentWithParameterDelegate = ((type, param) =>
			{
				actualType = type;
				actualParam = (PollResultsPageNavigationCriteria)param;
			});

			// Act
			await viewModel.LoadPollAsync();

			// Assert
			Assert.AreEqual(typeof(PollResultsPageViewModel), actualType, "Navigation Type");
			Assert.AreEqual(pollId, actualParam.PollId, "Navigated Poll Id");
		}

		[TestMethod]
		public async Task LoadPollAsyncWithError()
		{
			// Arrange
			var viewModel = GetViewModel();

			var random = new Random();

			var principal = new PrincipalMock();

			var identity = new UserIdentityMock();
			identity.UserID = random.Next();
			principal.Identity = identity;

			Csla.ApplicationContext.User = principal;

			var pollId = random.Next();

			viewModel.Parameter = Serializer.Serialize(
				new ViewPollPageNavigationCriteria { PollId = pollId });

			ObjectFactory.CreateAsyncDelegate = () => new PollSubmissionCommandMock();
			ObjectFactory.ExecuteAsyncDelegate = (command) =>
			{
				throw new DataPortalException(null, command);
			};

			var wasMessageBoxShown = false;
			MessageBox.ShowAsyncWithTitleDelegate = (message, title, buttons) =>
				{
					wasMessageBoxShown = true;
					return true;
				};

			// Act
			await viewModel.LoadPollAsync();

			// Assert
			Assert.IsTrue(wasMessageBoxShown, "Was Message Box Shown");
		}

		[TestMethod]
		public async Task SharePoll()
		{
			// Arrange
			var viewModel = GetViewModel();

			var random = new Random();

			var principal = new PrincipalMock();

			var identity = new UserIdentityMock();
			identity.UserID = random.Next();
			principal.Identity = identity;

			Csla.ApplicationContext.User = principal;

			var pollId = random.Next();
			var pollSubmission = new PollSubmissionMock
			{
				PollID = pollId
			};
			
			ObjectFactory.CreateAsyncDelegate = () => new PollSubmissionCommandMock();
			ObjectFactory.ExecuteAsyncDelegate = (command) =>
			{
				return new PollSubmissionCommandMock { Submission = pollSubmission };
			};

			var wasShareManagerInitialized = false;
			ShareManager.InitializeDelegate = () =>
				{
					wasShareManagerInitialized = true;
				};

			viewModel.Parameter = Serializer.Serialize(
				new ViewPollPageNavigationCriteria { PollId = pollId });

			await viewModel.LoadPollAsync();

			((IActivate)viewModel).Activate();

			var dataPackage = new DataPackage();
			var dataPackageView = dataPackage.GetView();

			// Act
			ShareManager.ExecuteShareRequested(dataPackage);

			// Assert
			Assert.IsTrue(wasShareManagerInitialized, "Was Share Manager Initialized");
			Assert.IsTrue(!string.IsNullOrEmpty(await dataPackageView.GetTextAsync()), "Text Exists");
			Assert.AreEqual(string.Format("myvote://poll/{0}", pollId), (await dataPackageView.GetApplicationLinkAsync()).ToString(), "Uri");
			Assert.IsTrue(!string.IsNullOrEmpty(await dataPackageView.GetHtmlFormatAsync()), "HTML Exists");
		}

		[TestMethod]
		public async Task Submit()
		{
			// Arrange
			var viewModel = GetViewModel();

			var random = new Random();

			var pollSubmission = new PollSubmissionMock { PollID = random.Next() };
			viewModel.PollSubmission = pollSubmission;

			Type actualType = null;
			PollResultsPageNavigationCriteria actualParam = null;
			Navigation.NavigateToViewModelAndRemoveCurrentWithParameterDelegate = ((type, param) =>
			{
				actualType = type;
				actualParam = (PollResultsPageNavigationCriteria)param;
			});

			// Act
			await viewModel.Submit();

			// Assert
			Assert.AreEqual(typeof(PollResultsPageViewModel), actualType, "Navigation Type");
			Assert.AreEqual(pollSubmission.PollID, actualParam.PollId, "Poll Id");
		}

		[TestMethod]
		public async Task SubmitWithError()
		{
			// Arrange
			var viewModel = GetViewModel();

			var random = new Random();

			var pollSubmission = new PollSubmissionMock { PollID = random.Next() };
			viewModel.PollSubmission = pollSubmission;
			pollSubmission.SaveAsyncDelegate = () =>
				{
					throw new DataPortalException(null, pollSubmission);
				};

			var wasMessageBoxShown = false;
			MessageBox.ShowAsyncWithTitleDelegate = (message, title, buttons) =>
			{
				wasMessageBoxShown = true;
				return true;
			};

			// Act
			await viewModel.Submit();

			// Assert
			Assert.IsTrue(wasMessageBoxShown, "Was Message Box Shown");
		}

		[TestMethod]
		public void PollSubmission()
		{
			// Arrange
			var viewModel = GetViewModel();

			var pollSubmission = new PollSubmissionMock();

			// Act
			viewModel.PropertyChanged += OnPropertyChanged;
			viewModel.PollSubmission = pollSubmission;
			viewModel.PropertyChanged -= OnPropertyChanged;

			// Assert
			Assert.AreSame(pollSubmission, viewModel.PollSubmission, "Poll Submission");
			Assert.IsTrue(PropertiesChanged.Contains("PollSubmission"), "Property Changed");
		}

		[TestMethod]
		public void CanSubmitTrue()
		{
			// Arrange
			var viewModel = GetViewModel();

			var pollSubmission = new PollSubmissionMock();
			pollSubmission.IsActive = true;
			pollSubmission.IsSavable = true;

			viewModel.PollSubmission = pollSubmission;

			// Act

			// Assert
			Assert.IsTrue(viewModel.CanSubmit);
		}

		[TestMethod]
		public void CanSubmitNotActive()
		{
			// Arrange
			var viewModel = GetViewModel();

			var pollSubmission = new PollSubmissionMock();
			pollSubmission.IsActive = false;
			pollSubmission.IsSavable = true;

			viewModel.PollSubmission = pollSubmission;

			// Act

			// Assert
			Assert.IsFalse(viewModel.CanSubmit);
		}

		[TestMethod]
		public void CanSubmitNotSavable()
		{
			// Arrange
			var viewModel = GetViewModel();

			var pollSubmission = new PollSubmissionMock();
			pollSubmission.IsActive = true;
			pollSubmission.IsSavable = false;

			viewModel.PollSubmission = pollSubmission;

			// Act

			// Assert
			Assert.IsFalse(viewModel.CanSubmit);
		}

		[TestMethod]
		public void CanSubmitSubmissionIsNull()
		{
			// Arrange
			var viewModel = GetViewModel();

			// Act

			// Assert
			Assert.IsFalse(viewModel.CanSubmit);
		}

		[TestMethod]
		public async Task PinPoll()
		{
			// Arrange
			var viewModel = GetViewModel();

			var pollSubmission = new PollSubmissionMock
			{
				PollID = new Random().Next(),
				PollQuestion = Guid.NewGuid().ToString()
			};
			viewModel.PollSubmission = pollSubmission;

			int actualPollId = -1;
			string actualPollQuestion = string.Empty;
			SecondaryPinner.PinPollDelegate = (frameworkElement, pollId, pollQuestion) =>
				{
					actualPollId = pollId;
					actualPollQuestion = pollQuestion;
					return true;
				};

			// Act
			await viewModel.PinPoll(null);

			// Assert
			Assert.AreEqual(pollSubmission.PollID, actualPollId, "Poll Id");
			Assert.AreEqual(pollSubmission.PollQuestion, actualPollQuestion, "Poll Question");
			Assert.IsTrue(viewModel.IsPollPinned, "Is Poll Pinned");
		}

		[TestMethod]
		public async Task UnpinPoll()
		{
			// Arrange
			var viewModel = GetViewModel();

			var pollSubmission = new PollSubmissionMock
			{
				PollID = new Random().Next(),
				PollQuestion = Guid.NewGuid().ToString()
			};
			viewModel.PollSubmission = pollSubmission;

			int actualPollId = -1;
			SecondaryPinner.UnpinPollDelegate = (frameworkElement, pollId) =>
			{
				actualPollId = pollId;
				return true;
			};

			// Act
			await viewModel.UnpinPoll(null);

			// Assert
			Assert.AreEqual(pollSubmission.PollID, actualPollId, "Poll Id");
			Assert.IsFalse(viewModel.IsPollPinned, "Is Poll Pinned");
		}

		[TestMethod]
		public async Task DeletePollYes()
		{
			// Arrange
			var viewModel = GetViewModel();

			var poll = new PollMock();

			var pollWasDeleted = false;
			poll.DeleteDelegate = () =>
			{
				pollWasDeleted = true;
			};

			this.PollFactory.FetchAsyncWithCriteriaDelegate = (criteria) =>
			{
				return poll;
			};

			this.MessageBox.ShowAsyncWithTitleDelegate = (content, title, buttons) =>
			{
				return true;
			};

			viewModel.PollSubmission = new PollSubmissionMock();

			// Act
			await viewModel.DeletePoll();

			// Assert
			Assert.IsTrue(pollWasDeleted);
		}

		[TestMethod]
		public async Task DeletePollNo()
		{
			// Arrange
			var viewModel = GetViewModel();

			var poll = new PollMock();

			var pollWasDeleted = false;
			poll.DeleteDelegate = () =>
			{
				pollWasDeleted = true;
			};

			this.PollFactory.FetchAsyncWithCriteriaDelegate = (criteria) =>
			{
				return poll;
			};

			this.MessageBox.ShowAsyncWithTitleDelegate = (content, title, buttons) =>
			{
				return false;
			};

			viewModel.PollSubmission = new PollSubmissionMock();

			// Act
			await viewModel.DeletePoll();

			// Assert
			Assert.IsFalse(pollWasDeleted);
		}
	}
}
