using System.Threading.Tasks;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using MyVote.UI.Helpers;
using Xamarin.Forms;

namespace MyVote.UI
{
    public partial class ViewPresenter
    {
        public override async void Show(MvxViewModelRequest request)
        {
            if (await AdaptiveTryShowPage(request))
                return;

            Mvx.Error("Skipping request for {0}", request.ViewModelType.Name);
        }

        private async Task<bool> AdaptiveTryShowPage(MvxViewModelRequest request)
        {
            var page = MvxPresenterHelpers.CreatePage(request);
            if (page == null)
                return false;

            var viewModel = MvvmCross.Forms.Presenter.Core.MvxPresenterHelpers.LoadViewModel(request);

            var mainPage = _mvxFormsApp.MainPage as NavigationPage;
            page.BindingContext = viewModel;

            if (mainPage == null)
            {
                _mvxFormsApp.MainPage = new NavigationPage(page);
                mainPage = (NavigationPage)_mvxFormsApp.MainPage;
                CustomPlatformInitialization(mainPage);
            }
            else
            {
                try
                {
                    await mainPage.PushAsync(page);
                }
                catch (System.Exception e)
                {
                    Mvx.Error("Exception pushing {0}: {1}\n{2}", page.GetType(), e.Message, e.StackTrace);
                }
            }

            return true;
        }

        public override void ChangePresentation(MvxPresentationHint hint)
        {
            if (hint is ClearBackstackHint)
            {
                var mainPage = _mvxFormsApp.MainPage as NavigationPage;
                if (mainPage != null && mainPage.CurrentPage != null)
                {
                    var navigation = mainPage.CurrentPage.Navigation;
                    for (var i = navigation.NavigationStack.Count - 1; i >= 0; i--)
                    {
                        var page = navigation.NavigationStack[i];
                        if (page != mainPage.CurrentPage)
                        {
                            navigation.RemovePage(page);
                        }
                    }
                }
            }
            base.ChangePresentation(hint);
        }
    }
}
