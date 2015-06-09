using Cirrious.CrossCore;
using Cirrious.MvvmCross.Platform;
using Cirrious.MvvmCross.ViewModels;
using Foundation;
using MyVote.UI.Services;
using MyVote.UI.Views;
using UIKit;
using Xamarin;
using Xamarin.Forms.Platform.iOS;

namespace MyVote.UI.Ios
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : FormsApplicationDelegate, IMvxLifetime
    {
        // class-level declarations
        public event System.EventHandler<MvxLifetimeEventArgs> LifetimeChanged;

        UIWindow window;

        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            // create a new window instance based on the screen size
            window = new UIWindow(UIScreen.MainScreen.Bounds);

            var setup = new MvxFormsSetup(this, window);
            setup.Initialize();

            var startup = Mvx.Resolve<IMvxAppStart>();
            Xamarin.Forms.Forms.Init();
            Insights.Initialize("fbe45ea4c25df48a8eeb15f1bdd929cf14482e4b");

            var newApp = new App();
            startup.Start();

            LoadApplication(newApp);

            return base.FinishedLaunching(app, options);
        }

        public override void WillEnterForeground(UIApplication application)
        {
            FireLifetimeChanged(MvxLifetimeEvent.ActivatedFromMemory);
        }

        public override void DidEnterBackground(UIApplication application)
        {
            FireLifetimeChanged(MvxLifetimeEvent.Deactivated);
        }

        public override void WillTerminate(UIApplication application)
        {
            FireLifetimeChanged(MvxLifetimeEvent.Closing);
        }

        public override void FinishedLaunching(UIApplication application)
        {
            FireLifetimeChanged(MvxLifetimeEvent.Launching);
        }

        private void FireLifetimeChanged(MvxLifetimeEvent which)
        {
            var handler = LifetimeChanged;
            if (handler != null)
                handler(this, new MvxLifetimeEventArgs(which));
        }
    }
}