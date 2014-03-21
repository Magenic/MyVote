using MyVote.UI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVote.UI.W8.Tests.Mocks
{
	public class NavigationMock : INavigation
	{
		public Action<Type> NavigateToViewModelDelegate { get; set; }
		public void NavigateToViewModel<TViewModel>()
		{
			if (NavigateToViewModelDelegate != null)
			{
				NavigateToViewModelDelegate(typeof(TViewModel));
			}
		}

		public Action<Type, object> NavigateToViewModelWithParameterDelegate { get; set; }
		public void NavigateToViewModel<TViewModel>(object parameter) where TViewModel : UI.ViewModels.PageViewModelBase
		{
			if (NavigateToViewModelWithParameterDelegate != null)
			{
				NavigateToViewModelWithParameterDelegate(typeof(TViewModel), parameter);
			}
		}

		public Action<Type> NavigateToViewModelAndRemoveCurrentDelegate { get; set; }
		public void NavigateToViewModelAndRemoveCurrent<TViewModel>()
		{
			if (NavigateToViewModelAndRemoveCurrentDelegate != null)
			{
				NavigateToViewModelAndRemoveCurrentDelegate(typeof(TViewModel));
			}
		}

		public Action<Type, object> NavigateToViewModelAndRemoveCurrentWithParameterDelegate { get; set; }
		public void NavigateToViewModelAndRemoveCurrent<TViewModel>(object parameter) where TViewModel : UI.ViewModels.PageViewModelBase
		{
			if (NavigateToViewModelAndRemoveCurrentWithParameterDelegate != null)
			{
				NavigateToViewModelAndRemoveCurrentWithParameterDelegate(typeof(TViewModel), parameter);
			}
		}

		public Action GoBackDelegate { get; set; }
		public void GoBack()
		{
			if (GoBackDelegate != null)
			{
				GoBackDelegate();
			}
		}

		public bool CanGoBack { get; set; }
	}
}
