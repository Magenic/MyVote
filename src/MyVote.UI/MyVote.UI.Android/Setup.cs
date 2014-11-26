using Android.Content;
using Cirrious.CrossCore;
using Cirrious.CrossCore.Platform;
using Cirrious.MvvmCross.Droid.Platform;
using Cirrious.MvvmCross.Droid.Views;
using Cirrious.MvvmCross.ViewModels;

using MyVote.UI.MvxDroidAdaptation;

namespace MyVote.UI
{
    public class Setup : MvxAndroidSetup
    {
        public Setup(Context applicationContext) : base(applicationContext)
        {
        }

        protected override IMvxApplication CreateApp()
        {
            return new App();
        }
		
        protected override IMvxTrace CreateDebugTrace()
        {
            return new DebugTrace();
        }

        protected override IMvxAndroidViewPresenter CreateViewPresenter() 
        { 
            var presenter = new MvxPagePresenter(); 
            Mvx.RegisterSingleton<IMvxPageNavigationHost>(presenter); 
            return presenter; 
        }

        // Use Autofac for IoC
        protected override Cirrious.CrossCore.IoC.IMvxIoCProvider CreateIocProvider()
        {
            return new AutofacMvxProvider();
        }
    }
}