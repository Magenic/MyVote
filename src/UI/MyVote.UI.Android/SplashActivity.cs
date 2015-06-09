using Android.App;
using Android.Content.PM;
using MyVote.UI.Helpers;

namespace MyVote.UI
{
	[Activity(
		Label = "MyVote",
		MainLauncher = true,
		NoHistory = true,
		ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class SplashActivity : MvxFormsSplashScreenActivity
	{
		public SplashActivity()
            : base(Resource.Layout.SplashScreen)
        {
        }
	}
}