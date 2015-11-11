using Cirrious.CrossCore;
using Cirrious.MvvmCross.ViewModels;
using Cirrious.MvvmCross.Views;
using MyVote.UI.NavigationCriteria;
using MyVote.UI.Services;
using MyVote.UI.ViewModels;
using MyVote.UI.ViewModels.Settings;
using MyVote.UI.Views;
using MyVote.UI.Views.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

#if WINDOWS_APP
using Windows.UI.ApplicationSettings;
#endif // WINDOWS_APP

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=234227

namespace MyVote.UI
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application
    {
#if WINDOWS_PHONE_APP
        private TransitionCollection transitions;
#endif

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += this.OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
		protected override void OnLaunched(LaunchActivatedEventArgs args)
		{
			var rootFrame = Window.Current.Content as Frame;

			// Do not repeat app initialization when the Window already has content,
			// just ensure that the window is active
			if (rootFrame == null)
			{
				// Create a Frame to act as the navigation context and navigate to the first page
				rootFrame = new Frame();

				if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
				{
					//TODO: Load state from previously suspended application
				}

				// Place the frame in the current Window
				Window.Current.Content = rootFrame;
			}

			if (rootFrame.Content == null)
			{
				// When the navigation stack isn't restored navigate to the first page,
				// configuring the new page by passing required information as a navigation
				// parameter

				LandingPageNavigationCriteria hintCriteria = null;
				if (!string.IsNullOrEmpty(args.Arguments))
				{
					int pollId;
					if (int.TryParse(args.Arguments, out pollId))
					{
						hintCriteria = new LandingPageNavigationCriteria
						{
							PollId = pollId
						};
					}
				}

				var setup = new Setup(rootFrame);
				setup.Initialize();

				var start = Mvx.Resolve<IMvxAppStart>();
				start.Start(hintCriteria);
			}
			// Ensure the current window is active
			Window.Current.Activate();
		}

		protected override void OnWindowCreated(WindowCreatedEventArgs args)
		{
#if WINDOWS_APP
			SettingsPane.GetForCurrentView().CommandsRequested += App_CommandsRequested;
#endif // WINDOWS_APP
		}

		protected override void OnSearchActivated(SearchActivatedEventArgs args)
		{
			base.OnSearchActivated(args);
			if (args != null && args.Kind == ActivationKind.Search)
			{
				var searchArgs = args as SearchActivatedEventArgs;

				var start = Mvx.Resolve<IMvxAppStart>();
				if (args.PreviousExecutionState == ApplicationExecutionState.Running)
				{
					var criteria = new PollsPageSearchNavigationCriteria
					{
						SearchQuery = searchArgs.QueryText
					};
					start.Start(criteria);
				}
				else
				{
					var criteria = new LandingPageNavigationCriteria
					{
						SearchQuery = searchArgs.QueryText
					};
					start.Start(criteria);
				}
			}
		}

#if WINDOWS_APP
		private void App_CommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
		{
			SettingsCommand accountSetting = new SettingsCommand("Account", "Account", (x) =>
			{
				var viewModel = Mvx.Resolve<AccountSettingsPageViewModel>();
				var view = new AccountSettingsPage
				{
					DataContext = viewModel
				};

				view.Show();
			});
			args.Request.ApplicationCommands.Add(accountSetting);

			SettingsCommand privacySetting = new SettingsCommand("Privacy", "Privacy Policy", this.OpenPrivacyPolicy);
			args.Request.ApplicationCommands.Add(privacySetting);
		}
#endif // WINDOWS_APP

		private async void OpenPrivacyPolicy(IUICommand command)
		{
			Uri uri = new Uri("http://myvote.azurewebsites.net/Privacy");
			await Launcher.LaunchUriAsync(uri);
		}

#if WINDOWS_PHONE_APP
        /// <summary>
        /// Restores the content transitions after the app has launched.
        /// </summary>
        /// <param name="sender">The object where the handler is attached.</param>
        /// <param name="e">Details about the navigation event.</param>
        private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e)
        {
            var rootFrame = sender as Frame;
            rootFrame.ContentTransitions = this.transitions ?? new TransitionCollection() { new NavigationThemeTransition() };
            rootFrame.Navigated -= this.RootFrame_FirstNavigated;
        }

		protected override void OnActivated(IActivatedEventArgs args)
		{
			// Windows Phone 8.1 requires you to handle the respose from the WebAuthenticationBroker.
			if (args.Kind == ActivationKind.WebAuthenticationBrokerContinuation)
			{
				// Completes the sign-in process started by LoginAsync.
				Mvx.Resolve<IMobileService>().AuthenticationComplete(args as WebAuthenticationBrokerContinuationEventArgs);
			}

			base.OnActivated(args);
		}
#endif

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

            // TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}