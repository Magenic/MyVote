﻿using MyVote.UI.Contracts;
using MyVote.UI.Helpers;
using MyVote.UI.NavigationCriteria;
using MyVote.UI.ViewModels;
using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Autofac;
using MyVote.UI.Services;
using MyVote.UI.Views;

namespace MyVote.UI
{
	/// <summary>
	/// Provides application-specific behavior to supplement the default Application class.
	/// </summary>
	sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            Microsoft.ApplicationInsights.WindowsAppInitializer.InitializeAsync(
                Microsoft.ApplicationInsights.WindowsCollectors.Metadata |
                Microsoft.ApplicationInsights.WindowsCollectors.Session);
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {

#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                LandingPageNavigationCriteria hintCriteria = null;
                if (!string.IsNullOrEmpty(e.Arguments))
                {
                    int pollId;
                    if (int.TryParse(e.Arguments, out pollId))
                    {
                        hintCriteria = new LandingPageNavigationCriteria
                        {
                            PollId = pollId
                        };
                    }
                }

				var app = new MyVoteApp();
				app.Initialize();
				InitContainer();

				var navigationService = Ioc.Resolve<INavigationService>();
				navigationService.ShowViewModel<LandingPageViewModel>();
				// TODO: Work this out without MvvmCross
				/*var setup = new Setup(rootFrame);
                setup.Initialize();

                var start = Mvx.Resolve<IMvxAppStart>();
                start.Start(hintCriteria);*/
			}
			// Ensure the current window is active
			Window.Current.Activate();
        }

		protected override void OnActivated(IActivatedEventArgs args)
		{
			if (args.Kind == ActivationKind.Protocol)
			{
				ProtocolActivatedEventArgs protocolArgs = args as ProtocolActivatedEventArgs;
				Frame content = Window.Current.Content as Frame;
				if (content.Content.GetType() == typeof(LandingPage))
				{
					content.Navigate(typeof(LandingPage), protocolArgs.Uri);
				}
			}
			Window.Current.Activate();
			base.OnActivated(args);
		}

		/// <summary>
		/// Invoked when Navigation to a certain page fails
		/// </summary>
		/// <param name="sender">The Frame which failed navigation</param>
		/// <param name="e">Details about the navigation failure</param>
		void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

		private void InitContainer()
		{

			var containerBuilder = new Autofac.ContainerBuilder();

			containerBuilder.RegisterType<NavigationService>().As<INavigationService>();
			containerBuilder.RegisterType<MessageBox>().As<IMessageBox>();
			containerBuilder.RegisterInstance(new ViewPresenter((Frame)Window.Current.Content)).As<IViewPresenter>();
			containerBuilder.RegisterType<PhotoChooser>().As<IPhotoChooser>();
			//containerBuilder.RegisterType<ImageResize>().As<IImageResize>();

			containerBuilder.RegisterType<MobileService>().As<IMobileService>();

			containerBuilder.Update(Ioc.Container);
		}
    }
}
