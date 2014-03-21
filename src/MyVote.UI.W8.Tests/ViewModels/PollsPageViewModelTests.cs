using Caliburn.Micro;
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

namespace MyVote.UI.W8.Tests.ViewModels
{
	[TestClass]
	public sealed class PollsPageViewModelTests
	{
		private NavigationMock Navigation { get; set; }
		private ObjectFactoryMock<IPollSearchResults> ObjectFactory { get; set; }

	    private PollsPageViewModel GetViewModel()
		{
			return new PollsPageViewModel(Navigation, ObjectFactory);
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
			ObjectFactory = new ObjectFactoryMock<IPollSearchResults>();

			PropertiesChanged = new List<string>();

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
		public async Task SearchPolls()
		{
			// Arrange
			var viewModel = GetViewModel();
			
			var expectedQueryType = PollSearchResultsQueryType.MostPopular;

			viewModel.SelectedSearchOption = new PollSearchOptionViewModel
			{
				QueryType = expectedQueryType
			};

			var expectedSearchResults = new PollSearchResultsMock();

			PollSearchResultsQueryType? actualQueryType = null;
			ObjectFactory.FetchAsyncWithCriteriaDelegate = (criteria) =>
				{
					actualQueryType = (PollSearchResultsQueryType)criteria;
					return expectedSearchResults;
				};

			// Act
			await viewModel.SearchPollsAsync();

			// Assert
			Assert.AreSame(expectedSearchResults, viewModel.PollSearchResults, "PollSearchResults");
			Assert.AreEqual(expectedQueryType, actualQueryType, "Query Type");
		}

		[TestMethod]
		public void AddPoll()
		{
			// Arrange
			var viewModel = GetViewModel();

			Type actualType = null;
			Navigation.NavigateToViewModelDelegate = ((type) =>
			{
				actualType = type;
			});

			// Act
			viewModel.AddPoll();

			// Assert
			Assert.AreEqual(typeof(AddPollPageViewModel), actualType, "Navigation Type");
		}

		[TestMethod]
		public void ViewPoll()
		{
			// Arrange
			var viewModel = GetViewModel();

			var expectedPoll = new PollSearchResultMock { Id = 1 };

			Type actualType = null;
			ViewPollPageNavigationCriteria actualPoll = null;
			Navigation.NavigateToViewModelWithParameterDelegate = ((type, param) =>
			{
				actualType = type;
				actualPoll = (ViewPollPageNavigationCriteria)param;
			});

			// Act
			viewModel.ViewPoll(expectedPoll);

			// Assert
			Assert.AreEqual(typeof(ViewPollPageViewModel), actualType, "Navigation Type");
			Assert.AreEqual(expectedPoll.Id, actualPoll.PollId, "Poll Id");
		}

		[TestMethod]
		public void SelectedSearchOption()
		{
			// Arrange
			var viewModel = GetViewModel();

			var wasSearchExecuted = false;
			ObjectFactory.FetchAsyncWithCriteriaDelegate = (criteria) =>
			{
				wasSearchExecuted = true;
				return new PollSearchResultsMock();
			};

			var expectedSearchOption = new PollSearchOptionViewModel();

			// Act
			viewModel.PropertyChanged += OnPropertyChanged;
			viewModel.SelectedSearchOption = expectedSearchOption;
			viewModel.PropertyChanged -= OnPropertyChanged;

			// Assert
			Assert.AreSame(expectedSearchOption, viewModel.SelectedSearchOption, "Selected Search Option");
			Assert.IsTrue(PropertiesChanged.Contains("SelectedSearchOption"), "Property Changed");
			Assert.IsTrue(wasSearchExecuted, "Was Search Executed");
		}

		[TestMethod]
		public void PollSearchResults()
		{
			// Arrange
			var viewModel = GetViewModel();

			var expectedSearchResults = new PollSearchResultsMock();

			// Act
			viewModel.PropertyChanged += OnPropertyChanged;
			viewModel.PollSearchResults = expectedSearchResults;
			viewModel.PropertyChanged -= OnPropertyChanged;

			// Assert
			Assert.AreSame(expectedSearchResults, viewModel.PollSearchResults, "Poll Search Results");
			Assert.IsTrue(PropertiesChanged.Contains("PollSearchResults"), "Property Changed");
		}

		[TestMethod]
		public void IsBusy()
		{
			// Arrange
			var viewModel = GetViewModel();

			// Act
			viewModel.PropertyChanged += OnPropertyChanged;
			viewModel.IsBusy = true;
			viewModel.PropertyChanged -= OnPropertyChanged;

			// Assert
			Assert.IsTrue(viewModel.IsBusy, "Is Busy");
			Assert.IsTrue(PropertiesChanged.Contains("IsBusy"), "Property Changed");
		}
	}
}
