using System;
using System.Linq;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Touch.Views.Presenters;
using Cirrious.MvvmCross.ViewModels;
using MyVote.UI.Views;
using UIKit;
using Xamarin.Forms;

namespace MyVote.UI.Helpers
{
    public class MvxPagePresenter : IMvxTouchViewPresenter
    {
        private readonly UIWindow window;

        public MvxPagePresenter(UIWindow window)
        {
            this.window = window;
        }

        public void Show(MvxViewModelRequest request)
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
            page.BindingContext = viewModel;

            if (VmPageMappings.NavigationPage == null)
            {
                VmPageMappings.NavigationPage = new NavigationPage(page);
                App.SetMainPage(VmPageMappings.NavigationPage);
            }
            else
            {
                VmPageMappings.NavigationPage.Navigation.PushAsync(page);
            }

            return true;
        }

        public void ChangePresentation(MvxPresentationHint hint)
        {
            if (VmPageMappings.NavigationPage == null)
                return;

            if (hint is MvxClosePresentationHint)
            {
                var vm = ((MvxClosePresentationHint) hint).ViewModelToClose;
                if (VmPageMappings.NavigationPage.Navigation.NavigationStack.Any(p => p.BindingContext == vm))
                {
                    var page = VmPageMappings.NavigationPage.Navigation.NavigationStack.Single(p => p.BindingContext == vm);

                    page.BindingContext = null;
                    VmPageMappings.NavigationPage.Navigation.RemovePage(page);
                }
            }
        }

        public bool PresentModalViewController(UIViewController controller, bool animated)
        {
            return false;
        }

        public void NativeModalViewControllerDisappearedOnItsOwn()
        {

        }

        public void AddPresentationHintHandler<THint>(Func<THint, bool> action) where THint : MvxPresentationHint
        {
        }
    }
}