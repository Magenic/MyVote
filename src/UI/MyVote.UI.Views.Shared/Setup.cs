using Cirrious.CrossCore.Platform;
using Cirrious.MvvmCross.ViewModels;
using Cirrious.MvvmCross.WindowsCommon.Platform;
using Cirrious.MvvmCross.WindowsCommon.Views;
using MyVote.UI.Helpers;
using MyVote.UI.Services;
using Windows.UI.Xaml.Controls;

namespace MyVote.UI
{
    public class Setup : MvxWindowsSetup
    {
		private readonly Frame rootFrame;

        public Setup(Frame rootFrame) : base(rootFrame)
        {
			this.rootFrame = rootFrame;
        }

        protected override IMvxApplication CreateApp()
        {
            return new MyVoteApp();
        }
		
        protected override IMvxTrace CreateDebugTrace()
        {
            return new DebugTrace();
        }

		// Use Autofac for IoC
		protected override Cirrious.CrossCore.IoC.IMvxIoCProvider CreateIocProvider()
		{
			return new AutofacMvxProvider();
		}

		protected override IMvxNameMapping CreateViewToViewModelNaming()
		{
			return new MvxViewToViewModelNameMapping();
		}
    }
}