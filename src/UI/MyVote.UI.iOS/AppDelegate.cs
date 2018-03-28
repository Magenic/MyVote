using System;
using Autofac;
using Foundation;
using MyVote.UI.Contracts;
using MyVote.UI.Helpers;
using MyVote.UI.Services;
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
    public class AppDelegate :FormsApplicationDelegate
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
            Insights.Initialize("fbe45ea4c25df48a8eeb15f1bdd929cf14482e4b");

            global::Xamarin.Forms.Forms.Init();

            var context = new UiContext();

            var formsApp = new Views.App();
            var containerBuilder = new Autofac.ContainerBuilder();

            containerBuilder.RegisterType<NavigationService>().As<INavigationService>();
            containerBuilder.RegisterType<MessageBox>().As<IMessageBox>();
            containerBuilder.RegisterType<ViewPresenter>().As<IViewPresenter>();
            containerBuilder.RegisterType<PhotoChooser>().As<IPhotoChooser>();
            containerBuilder.RegisterType<ImageResize>().As<IImageResize>();

            containerBuilder.Register(c => new MobileService(context)).As<IMobileService>();

            containerBuilder.Update(Ioc.Container);

            formsApp.Start();

            LoadApplication(formsApp);

            UITabBar.Appearance.SelectedImageTintColor = ((Color)formsApp.Resources["AppOrange"]).ToUIColor();
            UIPickerView.Appearance.TintColor = ((Color)formsApp.Resources["AppOrange"]).ToUIColor();
            UIDatePicker.Appearance.TintColor = ((Color)formsApp.Resources["AppOrange"]).ToUIColor();
            UISwitch.Appearance.OnTintColor = ((Color)formsApp.Resources["AppOrange"]).ToUIColor();

            return base.FinishedLaunching(app, options); ;
        }

        public static Func<NSUrl, bool> ResumeWithURL;

        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            return ResumeWithURL != null && ResumeWithURL(url);
        }
    }
}