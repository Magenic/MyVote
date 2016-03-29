using Foundation;
using MvvmCross.Core.ViewModels;
using MvvmCross.iOS.Platform;
using MvvmCross.Platform;
using UIKit;
using Xamarin;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace MyVote.UI
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public class AppDelegate : MvxApplicationDelegate
    {
        public override UIWindow Window
        {
            get;
            set;
        }

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
            Window = new UIWindow(UIScreen.MainScreen.Bounds);

            var setup = new Setup(this, Window);
            setup.Initialize();
            var newApp = setup.FormsApp;

            var startup = Mvx.Resolve<IMvxAppStart>();
            Insights.Initialize("fbe45ea4c25df48a8eeb15f1bdd929cf14482e4b");

            UITabBar.Appearance.SelectedImageTintColor = ((Color)newApp.Resources["AppOrange"]).ToUIColor();
            UIPickerView.Appearance.TintColor = ((Color)newApp.Resources["AppOrange"]).ToUIColor();
            UIDatePicker.Appearance.TintColor = ((Color)newApp.Resources["AppOrange"]).ToUIColor();

            startup.Start();

            Window.MakeKeyAndVisible();

            return true;
        }
    }
}