using Android.App;
using Android.Content.PM;
using MyVote.UI.MvxDroidAdaptation;

namespace MyVote.UI
{
    [Activity(
        Label = "MyVote"
        , MainLauncher = true
        , Icon = "@drawable/icon"
        , Theme = "@style/Theme.Splash"
        , NoHistory = true
        , ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class SplashScreen : MvxFormsSplashScreenActivity
    {
        public SplashScreen()
            : base(Resource.Layout.SplashScreen)
        {
        }
    }
}