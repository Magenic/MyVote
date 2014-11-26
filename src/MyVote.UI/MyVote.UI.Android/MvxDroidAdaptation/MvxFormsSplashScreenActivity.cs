// NOTE: Created from Cheesebaron's MVVM Cross/Xamarin.Forms example at:
// https://github.com/Cheesebaron/Xam.Forms.Mvx

using Cirrious.MvvmCross.Droid.Views;
using Xamarin;

namespace MyVote.UI.MvxDroidAdaptation
{
    public abstract class MvxFormsSplashScreenActivity
        : MvxSplashScreenActivity
    {
        protected MvxFormsSplashScreenActivity()
        {
        }

        protected MvxFormsSplashScreenActivity(int resourceId)
            : base(resourceId)
        {
        }

        public override void InitializationComplete()
        {
            Insights.Initialize("fbe45ea4c25df48a8eeb15f1bdd929cf14482e4b", ApplicationContext);
            StartActivity(typeof(MvxNavigationActivity));
        }
    }
}