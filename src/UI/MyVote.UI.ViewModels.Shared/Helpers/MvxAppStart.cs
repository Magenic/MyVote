using Cirrious.CrossCore;
using Cirrious.CrossCore.Platform;
using Cirrious.MvvmCross.ViewModels;
using MyVote.UI.NavigationCriteria;
using MyVote.UI.ViewModels;
using System.Collections.Generic;

namespace MyVote.UI.Helpers
{
    public sealed class MvxAppStart
		: MvxNavigatingObject, IMvxAppStart
    {
		public void Start(object hint = null)
		{
			if (hint != null)
			{
				if (hint is LandingPageNavigationCriteria)
				{
					ShowViewModel<LandingPageViewModel>(hint);
				}
				else if (hint is PollsPageSearchNavigationCriteria)
				{
					ShowViewModel<PollsPageViewModel>(hint);
				}
			}
			else
			{
				ShowViewModel<LandingPageViewModel>();
			}
		}

		private void ShowViewModel<TViewModel>(object parameter)
			where TViewModel : IMvxViewModel
		{
			var json = Mvx.Resolve<IMvxJsonConverter>().SerializeObject(parameter);
			base.ShowViewModel<TViewModel>(new Dictionary<string, string>()
            {
                {"parameter", json}
            });
		}
    }
}
