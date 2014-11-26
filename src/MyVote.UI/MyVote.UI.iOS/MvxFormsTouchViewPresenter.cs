using System.Threading.Tasks;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Touch.Views.Presenters;
using Cirrious.MvvmCross.ViewModels;
using MonoTouch.UIKit;

using MyVote.UI.Helpers;
using MyVote.UI.Services;

using Xamarin.Forms;

namespace MyVote.UI
{
    public class MvxFormsTouchViewPresenter : IMvxTouchViewPresenter
    {
        private readonly UIWindow uiWindow;

        public MvxFormsTouchViewPresenter(UIWindow window)
        {
            uiWindow = window;
        }

        public async void Show(MvxViewModelRequest request)
        {
            if (await TryShowPage(request))
                return;

            Mvx.Error("Skipping request for {0}", request.ViewModelType.Name);
        }

        private async Task<bool> TryShowPage(MvxViewModelRequest request)
        {
            var page = MvxPresenterHelpers.CreatePage(request);
            if (page == null)
                return false;

            if (VMPageMappings.NavigationPage == null)
            {
                Xamarin.Forms.Forms.Init();
                VMPageMappings.NavigationPage = new NavigationPage(page);
                var controller = VMPageMappings.NavigationPage.CreateViewController();
                
                var uiContext = new UIContext
                {
                    CurrentContext = controller
                };
                
                uiWindow.RootViewController = controller;
            }
            else
            {
                await VMPageMappings.NavigationPage.PushAsync(page);
            }

            var viewModel = MvxPresenterHelpers.LoadViewModel(request);

            page.BindingContext = viewModel;
            return true;
        }

        public async void ChangePresentation(MvxPresentationHint hint)
        {
            if (hint is MvxClosePresentationHint)
            {
                // TODO - perhaps we should do more here... also async void is a boo boo
                await VMPageMappings.NavigationPage.PopAsync();
            }
        }

        public bool PresentModalViewController(UIViewController controller, bool animated)
        {
            return false;
        }

        public void NativeModalViewControllerDisappearedOnItsOwn()
        {

        }
    }
}