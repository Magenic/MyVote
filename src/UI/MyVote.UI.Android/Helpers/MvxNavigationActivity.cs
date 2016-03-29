using Android.App;
using Android.OS;
using Android.Content.PM;
using MvvmCross.Core.ViewModels;
using MvvmCross.Core.Views;
using MvvmCross.Forms.Presenter.Droid;
using MvvmCross.Platform;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using MyVote.UI.Services;
using MyVote.UI.Views;
using Xamarin;

namespace MyVote.UI.Helpers
{
	[Activity(
        Label = "MyVote", Theme = "@style/Theme.MyVoteAll", 
		ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public sealed class MvxNavigationActivity : FormsApplicationActivity
    {
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
            Insights.Initialize("fbe45ea4c25df48a8eeb15f1bdd929cf14482e4b", ApplicationContext);

			Forms.Init(this, bundle);
		    if (Device.Idiom == TargetIdiom.Phone)
		    {
		        this.RequestedOrientation = ScreenOrientation.Portrait;
		    }

			var uiContext = new UiContext
			{
				CurrentContext = this
			};

			//Mvx.Resolve<IMvxPageNavigationHost>().NavigationProvider = this;
		    var formsApp = new App();
            LoadApplication(formsApp);
            var presenter = (MvxFormsDroidPagePresenter)Mvx.Resolve<IMvxViewPresenter>();
            presenter.MvxFormsApp = formsApp;
            Mvx.Resolve<IMvxAppStart>().Start();
		}

        //public async void Push(Page page)
        //{
        //    if (VmPageMappings.NavigationPage != null)
        //    {
        //        await VmPageMappings.NavigationPage.PushAsync(page);
        //        return;
        //    }
        //    VmPageMappings.NavigationPage = new NavigationPage(page);
        //    App.SetMainPage(VmPageMappings.NavigationPage);
        //}

        //public async void Pop()
        //{
        //    if (VmPageMappings.NavigationPage == null)
        //        return;

        //    await VmPageMappings.NavigationPage.PopAsync();
        //}
	}
}