

using MvvmCross.Droid.Views;

namespace MyVote.UI.Helpers
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
			StartActivity(typeof(MvxNavigationActivity));
		}
	}
}