using MvvmCross.Forms.Presenter.Core;
using MyVote.UI.Helpers;
using Xamarin.Forms;

namespace MyVote.UI.Views
{
	public partial class App : MvxFormsApp
    {
	    private static App MainApplication;

        public App()
		{
            InitializeComponent();
            MainApplication = this;
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
