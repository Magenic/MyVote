using Caliburn.Micro;
using MyVote.UI.ViewModels;

namespace MyVote.UI.Helpers
{
	public sealed class Navigation : INavigation
	{
		private readonly INavigationService navigationService;

		public Navigation(INavigationService navigationService)
		{
			this.navigationService = navigationService;
		}

		public void NavigateToViewModel<TViewModel>()
		{
			this.navigationService.NavigateToViewModel<TViewModel>();
		}

		public void NavigateToViewModel<TViewModel>(object parameter)
			where TViewModel : PageViewModelBase
		{
			var serializedParameter = Serializer.Serialize(parameter);

			this.navigationService.NavigateToViewModel<TViewModel>(serializedParameter);
		}

		public void NavigateToViewModelAndRemoveCurrent<TViewModel>()
		{
			// Clear this page from the navigation stack.
			this.navigationService.GoBack();

			this.navigationService.NavigateToViewModel<TViewModel>();
		}

		public void NavigateToViewModelAndRemoveCurrent<TViewModel>(object parameter) where TViewModel : PageViewModelBase
		{
			var serializedParameter = Serializer.Serialize(parameter);

			// Clear this page from the navigation stack.
			this.navigationService.GoBack();

			this.navigationService.NavigateToViewModel<TViewModel>(serializedParameter);
		}

		public void GoBack()
		{
			this.navigationService.GoBack();
		}

		public bool CanGoBack
		{
			get { return this.navigationService.CanGoBack; }
		}
	}
}
