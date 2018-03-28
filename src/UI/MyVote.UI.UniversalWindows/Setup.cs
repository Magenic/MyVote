using MyVote.UI.Services;
using Windows.UI.Xaml.Controls;

namespace MyVote.UI
{
	// TODO: Work this out without MvvmCross
	/*public class Setup : MvxWindowsSetup
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
        protected override IMvxIoCProvider CreateIocProvider()
        {
            return new AutofacMvxProvider();
        }

        protected override IMvxNameMapping CreateViewToViewModelNaming()
        {
            return new MvxViewToViewModelNameMapping();
        }
    }*/
}
