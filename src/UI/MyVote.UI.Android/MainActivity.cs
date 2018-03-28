using Android.App;
using Android.Content.PM;
using Android.OS;
using Xamarin.Forms.Platform.Android;
using Autofac;
using MyVote.UI.Services;
using MyVote.UI.Helpers;
using MyVote.UI.Contracts;
using Xamarin;

namespace MyVote.UI
{
	[Activity(
		Label = "MyVote",
		MainLauncher = true,
		ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
        Theme = "@style/Theme.MyVoteAll")]
	public class MainActivity : FormsAppCompatActivity
	{
        protected override void OnCreate(Bundle savedInstanceState)
        {
            //TabLayoutResource = Resource.Layout.Tabbar;
            //ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Insights.Initialize("fbe45ea4c25df48a8eeb15f1bdd929cf14482e4b", ApplicationContext);

            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            var context = new UiContext();
            context.CurrentContext = this;

            var app = new Views.App();
            var containerBuilder = new Autofac.ContainerBuilder();

            containerBuilder.RegisterType<NavigationService>().As<INavigationService>();
            containerBuilder.RegisterType<MessageBox>().As<IMessageBox>();
            containerBuilder.RegisterType<ViewPresenter>().As<IViewPresenter>();
            containerBuilder.RegisterType<PhotoChooser>().As<IPhotoChooser>();
            containerBuilder.RegisterType<ImageResize>().As<IImageResize>();

            containerBuilder.Register(c => new MobileService(context)).As<IMobileService>();

            containerBuilder.Update(Ioc.Container);

            app.Start();

            LoadApplication(app);
        }
	}
}