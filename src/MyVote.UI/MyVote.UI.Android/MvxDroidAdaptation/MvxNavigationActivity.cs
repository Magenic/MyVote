// NOTE: Created from Cheesebaron's MVVM Cross/Xamarin.Forms example at:
// https://github.com/Cheesebaron/Xam.Forms.Mvx

using Android.App;
using Android.Content.PM;
using Android.OS;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.ViewModels;

using MyVote.UI.Helpers;
using MyVote.UI.Services;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace MyVote.UI.MvxDroidAdaptation
{
    [Activity(Label = "MyVote"
        , ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MvxNavigationActivity
        : AndroidActivity
        , IMvxPageNavigationProvider
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            Xamarin.Forms.Forms.Init(this, bundle);

            var uiContext = new UIContext
            {
                CurrentContext = this
            };

            Mvx.Resolve<IMvxPageNavigationHost>().NavigationProvider = this;
            Mvx.Resolve<IMvxAppStart>().Start();
        }

        public async void Push(Page page)
        {
            if (VMPageMappings.NavigationPage != null)
            {
                await VMPageMappings.NavigationPage.PushAsync(page);
                return;
            }

            VMPageMappings.NavigationPage = new NavigationPage(page);
            SetPage(VMPageMappings.NavigationPage);
        }

        public async void Pop()
        {
            if (VMPageMappings.NavigationPage == null)
                return;

            await VMPageMappings.NavigationPage.PopAsync();
        }
    }
}