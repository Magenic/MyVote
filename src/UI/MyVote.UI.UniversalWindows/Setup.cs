using MvvmCross.Core.ViewModels;
using MvvmCross.Platform.IoC;
using MvvmCross.Platform.Platform;
using MvvmCross.WindowsUWP.Platform;
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
        protected override IMvxIoCProvider CreateIocProvider()
        {
            return new AutofacMvxProvider();
        }

        protected override IMvxNameMapping CreateViewToViewModelNaming()
        {
            return new MvxViewToViewModelNameMapping();
        }
    }
}
