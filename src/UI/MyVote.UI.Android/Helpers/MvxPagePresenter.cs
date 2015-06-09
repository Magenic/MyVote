using System.Linq;
using Cirrious.MvvmCross.Droid.Views;
using Cirrious.MvvmCross.ViewModels;
using Cirrious.CrossCore;
using Xamarin.Forms;
using MyVote.UI.Views;

namespace MyVote.UI.Helpers
{
	public sealed class MvxPagePresenter
		: MvxAndroidViewPresenter
	{
		public override void Show(MvxViewModelRequest request)
		{
            if (TryShowPage(request))
				return;

			Mvx.Error("Skipping request for {0}", request.ViewModelType.Name);
		}

		private bool TryShowPage(MvxViewModelRequest request)
		{
			var page = MvxPresenterHelpers.CreatePage<Page>(request);
			if (page == null)
				return false;

			var viewModel = MvxPresenterHelpers.LoadViewModel(request);
			page.BindingContext =  viewModel;

            if (VmPageMappings.NavigationPage == null)
            {
                VmPageMappings.NavigationPage = new NavigationPage(page);
                App.SetMainPage(VmPageMappings.NavigationPage);
            }
            else
            {
                VmPageMappings.NavigationPage.PushAsync(page);
            }

			return true;
		}

	    public override void Close(IMvxViewModel viewModel)
	    {
	        if (VmPageMappings.NavigationPage == null)
	            return;

	        var page = VmPageMappings.NavigationPage.Navigation.NavigationStack.Single(p => p.BindingContext == viewModel);

            page.BindingContext = null;

            VmPageMappings.NavigationPage.Navigation.RemovePage(page);
	    }
	}
}