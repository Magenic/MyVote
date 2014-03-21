using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Caliburn.Micro;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using MyVote.BusinessObjects.Contracts;
using MyVote.UI.Helpers;
using MyVote.UI.NavigationCriteria;
using MyVote.UI.ViewModels;
using MyVote.UI.W8.Tests.Mocks;
using MyVote.UI.W8.Tests.Mocks.Base;
using Windows.ApplicationModel.DataTransfer;
using MyVote.BusinessObjects;

namespace MyVote.UI.W8.Tests.ViewModels
{
	[TestClass]
	public class PollResultsPageViewModelTests
	{
		private NavigationMock Navigation { get; set; }
		private ObjectFactoryMock<IPollResults> ObjectFactory { get; set; }
		private ObjectFactoryMock<IPoll> PollFactory { get; set; }
		private ObjectFactoryMock<IPollComment> PollCommentFactory { get; set; }
		private MessageBoxMock MessageBox { get; set; }
		private ShareManagerMock ShareManager { get; set; }
		private SecondaryPinnerMock SecondaryPinner { get; set; }

		private PollResultsPageViewModel GetViewModel()
		{
			return new PollResultsPageViewModel(
				Navigation,
				ObjectFactory,
				PollFactory,
				PollCommentFactory,
				MessageBox,
				ShareManager,
				SecondaryPinner);
		}

		[TestInitialize]
		public void Init()
		{
			Navigation = new NavigationMock();
			ObjectFactory = new ObjectFactoryMock<IPollResults>();
			PollFactory = new ObjectFactoryMock<IPoll>();
			PollCommentFactory = new ObjectFactoryMock<IPollComment>();
			MessageBox = new MessageBoxMock();
			ShareManager = new ShareManagerMock();
			SecondaryPinner = new SecondaryPinnerMock();
		}

		[TestCleanup]
		public void Cleanup()
		{
			Csla.ApplicationContext.User = null;
		}

		[TestMethod]
		public async Task SharePoll()
		{
			// Arrange
			var viewModel = GetViewModel();

			var random = new Random();

			var pollId = random.Next();
			var pollDataResults = new PollDataResultsMock
				{
					PollID = pollId,
					Question = Guid.NewGuid().ToString()
				};
			pollDataResults.ResultsDelegate = () =>
				{
					return new ReadOnlyListBaseCoreMock<IPollDataResult>(new List<IPollDataResult>());
				};

			var pollResults = new PollResultsMock
			{
				PollID = pollId,
				PollDataResults = pollDataResults
			};

			Csla.ApplicationContext.User = new PrincipalMock
			{
				Identity = new UserIdentityMock
				{
					UserID = random.Next()
				}
			};

			int actualPollId = -1;
			ObjectFactory.FetchAsyncWithCriteriaDelegate = (criteria) =>
			{
				actualPollId = ((PollResultsCriteria)criteria).PollID;
				return pollResults;
			};

			var wasShareManagerInitialized = false;
			ShareManager.InitializeDelegate = () =>
			{
				wasShareManagerInitialized = true;
			};

			viewModel.Parameter = Serializer.Serialize(
				new PollResultsPageNavigationCriteria { PollId = pollId });

			await viewModel.LoadPollAsync();

			((IActivate)viewModel).Activate();

			var dataPackage = new DataPackage();
			var dataPackageView = dataPackage.GetView();

			// Act
			ShareManager.ExecuteShareRequested(dataPackage);

			// Assert
			Assert.IsTrue(wasShareManagerInitialized, "Was Share Manager Initialized");
			Assert.AreEqual(pollId, actualPollId, "Actual Poll Id");
			Assert.IsTrue(!string.IsNullOrEmpty(await dataPackageView.GetTextAsync()), "Text Exists");
			Assert.AreEqual(string.Format("myvote://poll/{0}", pollId), (await dataPackageView.GetApplicationLinkAsync()).ToString(), "Uri");
			Assert.IsTrue(!string.IsNullOrEmpty(await dataPackageView.GetHtmlFormatAsync()), "HTML Exists");
		}

		[TestMethod]
		public async Task PinPoll()
		{
			// Arrange
			var viewModel = GetViewModel();

			var random = new Random();

			var pollDataResults = new PollDataResultsMock
			{
				PollID = random.Next(),
				Question = Guid.NewGuid().ToString()
			};
			pollDataResults.ResultsDelegate = () =>
				{
					return new ReadOnlyListBaseCoreMock<IPollDataResult>(
						new List<IPollDataResult> 
						{
							new PollDataResultMock
							{
								ResponseCount = random.Next()
							}
						});
				};

			var pollResults = new PollResultsMock
			{
				PollID = pollDataResults.PollID,
				PollDataResults = pollDataResults
			};

			viewModel.PollResults = pollResults;

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
			Assert.AreEqual(pollDataResults.PollID, actualPollId, "Poll Id");
			Assert.AreEqual(pollDataResults.Question, actualPollQuestion, "Poll Question");
			Assert.IsTrue(viewModel.IsPollPinned, "Is Poll Pinned");
		}

		[TestMethod]
		public async Task UnpinPoll()
		{
			// Arrange
			var viewModel = GetViewModel();

			var random = new Random();

			var pollDataResults = new PollDataResultsMock
			{
				PollID = random.Next(),
				Question = Guid.NewGuid().ToString()
			};
			pollDataResults.ResultsDelegate = () =>
			{
				return new ReadOnlyListBaseCoreMock<IPollDataResult>(
					new List<IPollDataResult> 
						{
							new PollDataResultMock
							{
								ResponseCount = random.Next()
							}
						});
			};

			var pollResults = new PollResultsMock
			{
				PollID = pollDataResults.PollID,
				PollDataResults = pollDataResults
			};

			viewModel.PollResults = pollResults;

			int actualPollId = -1;
			SecondaryPinner.UnpinPollDelegate = (frameworkElement, pollId) =>
			{
				actualPollId = pollId;
				return true;
			};

			// Act
			await viewModel.UnpinPoll(null);

			// Assert
			Assert.AreEqual(pollDataResults.PollID, actualPollId, "Poll Id");
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

			viewModel.PollResults = new PollResultsMock();

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

			viewModel.PollResults = new PollResultsMock();

			// Act
			await viewModel.DeletePoll();

			// Assert
			Assert.IsFalse(pollWasDeleted);
		}
	}
}
