using MyVote.UI.Contracts;
using MyVote.UI.Helpers;
using MyVote.UI.Services;
using MyVote.UI.ViewModels;
using Xamarin.Forms;

namespace MyVote.UI.Views
{
	public partial class App : Application
    {
	    private static App MainApplication;

        public App()
		{
            InitializeComponent();
            MainApplication = this;

            var app = new MyVoteApp();
            app.Initialize();

            SetMainPage(new NavigationPage());
		}

        public void Start()
        {
            var navigationService = Ioc.Resolve<INavigationService>();
            navigationService.ShowViewModel<LandingPageViewModel>();           
        }

	    public static void SetMainPage(NavigationPage navigation)
	    {
            VmPageMappings.NavigationPage = navigation;
            VmPageMappings.NavigationPage.BarBackgroundColor = Color.FromHex("#434342");
            VmPageMappings.NavigationPage.BarTextColor = Color.FromHex("#FF6600");
            MainApplication.MainPage = VmPageMappings.NavigationPage;
	    }
	}
}
