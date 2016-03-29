using Android.Content;
using MyVote.UI.Helpers;
using MyVote.UI.Services;
using MvvmCross.Core.ViewModels;
using MvvmCross.Core.Views;
using MvvmCross.Droid.Platform;
using MvvmCross.Droid.Views;
using MvvmCross.Platform;
using MvvmCross.Platform.IoC;
using MvvmCross.Platform.Platform;

namespace MyVote.UI
{
    public class Setup : MvxAndroidSetup
    {
        public Setup(Context applicationContext) : base(applicationContext)
        {
        }

        protected override IMvxApplication CreateApp()
        {
            return new MyVoteApp();
        }
		
        protected override IMvxTrace CreateDebugTrace()
        {
            return new DebugTrace();
        }

		protected override IMvxAndroidViewPresenter CreateViewPresenter()
		{
			var presenter = new ViewPresenter();
			Mvx.RegisterSingleton<IMvxViewPresenter>(presenter);
			return presenter;
		}

		// Use Autofac for IoC
        protected override IMvxIoCProvider CreateIocProvider()
        {
            return new AutofacMvxProvider();
        }
    }
}