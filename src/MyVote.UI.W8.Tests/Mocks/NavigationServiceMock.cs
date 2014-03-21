using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace MyVote.UI.W8.Tests.Mocks
{
	public class NavigationServiceMock : INavigationService
	{
		public bool CanGoBack { get; set; }

		public bool CanGoForward { get; set; }

		public Type CurrentSourcePageType { get; set; }

		public System.Action GoBackDelegate { get; set; }
		public void GoBack()
		{
			if (GoBackDelegate != null)
			{
				GoBackDelegate();
			}
		}

		public System.Action GoForwardDelegate { get; set; }
		public void GoForward()
		{
			if (GoForwardDelegate != null)
			{
				GoForwardDelegate();
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Param")]
		public Func<Type, object, bool> NavigateWithParamDelegate { get; set; }
		public bool Navigate(Type sourcePageType, object parameter)
		{
			if (NavigateWithParamDelegate != null)
			{
				return NavigateWithParamDelegate(sourcePageType, parameter);
			}
			else
			{
				return false;
			}
		}

		public Func<Type, bool> NavigateDelegate { get; set; }
		public bool Navigate(Type sourcePageType)
		{
			if (NavigateDelegate != null)
			{
				return NavigateDelegate(sourcePageType);
			}
			else
			{
				return false;
			}
		}

		public event NavigatedEventHandler Navigated;
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
		public void RaiseNavigated(NavigationEventArgs args)
		{
			if (Navigated != null)
			{
				Navigated(this, args);
			}
		}

		public event NavigatingCancelEventHandler Navigating;
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
		public void RaiseNavigating(NavigatingCancelEventArgs args)
		{
			if (Navigating != null)
			{
				Navigating(this, args);
			}
		}

		public event NavigationFailedEventHandler NavigationFailed;
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
		public void RaiseNavigationFailed(NavigationFailedEventArgs args)
		{
			if (NavigationFailed != null)
			{
				NavigationFailed(this, args);
			}
		}

		public event NavigationStoppedEventHandler NavigationStopped;
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
		public void RaiseNavigationStopped(NavigationEventArgs args)
		{
			if (NavigationStopped != null)
			{
				NavigationStopped(this, args);
			}
		}

		public Type SourcePageType { get; set; }
	}
}
