using MvvmCross.Core.ViewModels;
using MvvmCross.Core.Views;
using MvvmCross.iOS.Platform;
using MvvmCross.iOS.Views.Presenters;
using MvvmCross.Platform;
using MvvmCross.Platform.IoC;
using MvvmCross.Platform.Platform;
using UIKit;
using MyVote.UI.Services;
using MyVote.UI.Views;
using Xamarin.Forms;

namespace MyVote.UI
{
    public class Setup : MvxIosSetup
    {
        public Setup(IMvxApplicationDelegate applicationDelegate, UIWindow window)
            : base(applicationDelegate, window)
        {
        }

        public Application FormsApp { get; private set; }

        protected override IMvxTrace CreateDebugTrace()
        {
            return new DebugTrace();
        }

        protected override IMvxIosViewPresenter CreatePresenter()
        {
            Forms.Init();
            FormsApp = new App();
            var presenter = new ViewPresenter(Window, FormsApp);
            Mvx.RegisterSingleton<IMvxViewPresenter>(presenter);
            Mvx.RegisterType<IImageResize, ImageResize>();
            return presenter;
        }

        protected override IMvxApplication CreateApp()
        {
            return new MyVoteApp();
        }

        protected override IMvxIoCProvider CreateIocProvider()
        {
            return new AutofacMvxProvider();
        }
	}
}