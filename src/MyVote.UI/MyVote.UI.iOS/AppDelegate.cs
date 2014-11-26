using Cirrious.CrossCore;
using Cirrious.MvvmCross.Touch.Platform;
using Cirrious.MvvmCross.ViewModels;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Xamarin;

namespace MyVote.UI
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : MvxApplicationDelegate
    {
        // class-level declarations
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
            Insights.Initialize("fbe45ea4c25df48a8eeb15f1bdd929cf14482e4b");

            window = new UIWindow(UIScreen.MainScreen.Bounds); 
 
            var setup = new Setup(this, window); 
            setup.Initialize(); 
 
            var startup = Mvx.Resolve<IMvxAppStart>(); 
            startup.Start(); 
 
            // make the window visible 
            window.MakeKeyAndVisible(); 
 
            return true; 
            
            //Xamarin.Forms.Forms.Init();

            //window = new UIWindow(UIScreen.MainScreen.Bounds);

            //var loginPage = new MyVote.UI.Forms.Login();
            //var page = new NavigationPage(loginPage);
            //var controller = page.CreateViewController();

            //var uiContext = new UIContext
            //{
            //    CurrentContext = controller
            //};

            //page.Icon = "Icon";
            //page.Title = "MyVote";

            //window.RootViewController = controller;

            //window.MakeKeyAndVisible();

            //return true;
        }
    }
}