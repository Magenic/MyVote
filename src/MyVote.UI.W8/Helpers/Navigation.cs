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
#if WINDOWS_PHONE
			this.navigationService.UriFor<TViewModel>().Navigate();
#else
			this.navigationService.NavigateToViewModel<TViewModel>();
#endif // WINDOWS_PHONE
		}

		public void NavigateToViewModel<TViewModel>(object parameter)
			where TViewModel : PageViewModelBase
		{
			var serializedParameter = Serializer.Serialize(parameter);

#if WINDOWS_PHONE
			this.navigationService.UriFor<TViewModel>()
				.WithParam<string>(vm => vm.Parameter, serializedParameter)
				.Navigate();
#else
			this.navigationService.NavigateToViewModel<TViewModel>(serializedParameter);
#endif // WINDOWS_PHONE
		}

		public void NavigateToViewModelAndRemoveCurrent<TViewModel>()
		{
#if WINDOWS_PHONE
			this.navigationService.UriFor<TViewModel>().Navigate();

			// Clear this page from the navigation stack.
			this.navigationService.RemoveBackEntry();
#else
			// Clear this page from the navigation stack.
			this.navigationService.GoBack();

			this.navigationService.NavigateToViewModel<TViewModel>();
#endif // WINDOWS_PHONE
		}

		public void NavigateToViewModelAndRemoveCurrent<TViewModel>(object parameter) where TViewModel : PageViewModelBase
		{
			var serializedParameter = Serializer.Serialize(parameter);

#if WINDOWS_PHONE
			this.navigationService.UriFor<TViewModel>()
				.WithParam<string>(vm => vm.Parameter, serializedParameter)
				.Navigate();

			// Clear this page from the navigation stack.
			this.navigationService.RemoveBackEntry();
#else
			// Clear this page from the navigation stack.
			this.navigationService.GoBack();

			this.navigationService.NavigateToViewModel<TViewModel>(serializedParameter);
#endif // WINDOWS_PHONE
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
