using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using MyVote.UI.ViewModels;
using MyVote.UI.W8.Tests.Mocks;
using MyVote.UI.W8.Tests.Mocks.Base;

namespace MyVote.UI.W8.Tests.ViewModels
{
	[TestClass]
	public sealed class PageViewModelBaseTests
	{
		private NavigationMock Navigation { get; set; }

		private PageViewModelBase GetViewModel()
		{
			return new PageViewModelBaseMock(Navigation);
		}

		[TestInitialize]
		public void Init()
		{
			Navigation = new NavigationMock();
		}

		[TestMethod]
		public void GoBack()
		{
			// Arrange
			var viewModel = GetViewModel();

			var goBackCalled = false;
			Navigation.GoBackDelegate = () =>
				{
					goBackCalled = true;
				};

			// Act
			viewModel.GoBack();

			// Assert
			Assert.IsTrue(goBackCalled);
		}

		[TestMethod]
		public void CanGoBack()
		{
			// Arrange
			var viewModel = GetViewModel();

			Navigation.CanGoBack = true;

			// Act

			// Assert
			Assert.IsTrue(viewModel.CanGoBack);
		}
	}
}
