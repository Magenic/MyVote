using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Autofac;
using Caliburn.Micro;
using MyVote.BusinessObjects;
using MyVote.UI;
using MyVote.UI.Helpers;
using MyVote.UI.NavigationCriteria;
using MyVote.UI.ViewModels;
using MyVote.UI.ViewModels.Settings;
using MyVote.UI.Views;
using Windows.ApplicationModel.Activation;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml.Controls;
using Csla;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.System;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=234227

namespace MyVote.Client.W8
{
	/// <summary>
	/// Provides application-specific behavior to supplement the default Application class.
	/// </summary>
	sealed partial class App
	{
		private ContainerBuilder ContainerBuilder { get; set; }
		private IContainer container { get; set; }
		private INavigationService NavigationService { get; set; }

		/// <summary>
		/// Initializes the singleton application object.  This is the first line of authored code
		/// executed, and as such is the logical equivalent of main() or WinMain().
		/// </summary>
		public App()
		{
			this.InitializeComponent();
			this.Suspending += OnSuspending;
		}

		protected override void Configure()
		{
			Csla.ApplicationContext.DataPortalProxy = typeof(Csla.DataPortalClient.WcfProxy).AssemblyQualifiedName;
#if DEBUG
			Csla.ApplicationContext.DataPortalUrlString = "http://localhost:15440/MobilePortal.svc";
#else
			
#if STAGING
			Csla.ApplicationContext.DataPortalUrlString = "http://mystagingserver.cloudapp.net/MobilePortal.svc";
#else
			Csla.ApplicationContext.DataPortalUrlString = "http://myserver.cloudapp.net/MobilePortal.svc";
#endif // STAGING

#endif // DEBUG

			ContainerBuilder = new ContainerBuilder();

			new UiContainerBuilderComposition().Compose(this.ContainerBuilder);
			new BusinessObjectsContainerBuilderComposition().Compose(this.ContainerBuilder);

			SettingsPane.GetForCurrentView().CommandsRequested += App_CommandsRequested;
		}

		private void App_CommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
		{
			SettingsCommand accountSetting = new SettingsCommand("Account", "Account", (x) =>
			{
				var viewModel = this.container.Resolve<AccountSettingsViewModel>();
				var view = ViewLocator.LocateForModel(viewModel, null, null) as SettingsFlyout;
				ViewModelBinder.Bind(viewModel, view, null);
				view.Show();
			});
			args.Request.ApplicationCommands.Add(accountSetting);

			SettingsCommand privacySetting = new SettingsCommand("Privacy", "Privacy Policy", this.OpenPrivacyPolicy);
			args.Request.ApplicationCommands.Add(privacySetting);
		}

		protected override void PrepareViewFirst(Frame rootFrame)
		{
			this.NavigationService = new FrameAdapter(this.RootFrame);
			this.ContainerBuilder.RegisterInstance<INavigationService>(this.NavigationService).SingleInstance();
			this.container = this.ContainerBuilder.Build();
			ApplicationContext.DataPortalActivator = new ObjectActivator(this.container);
		}

		protected override object GetInstance(Type service, string key)
		{
			object instance = null;

			if (string.IsNullOrWhiteSpace(key))
			{
				if (this.container.TryResolve(service, out instance))
				{
					return instance;
				}
			}
			else
			{
				if (this.container.TryResolveNamed(key, service, out instance))
				{
					return instance;
				}
			}

			if (service == null)
			{
				throw new ArgumentNullException("service");
			}

			throw new InvalidOperationException(string.Format(
				CultureInfo.CurrentCulture, "Could not locate any instances of contract {0}.", key ?? service.Name));
		}

		protected override System.Collections.Generic.IEnumerable<object> GetAllInstances(Type service)
		{
			return this.container.Resolve(typeof(IEnumerable<>).MakeGenericType(service)) as IEnumerable<object>;
		}

		protected override void BuildUp(object instance)
		{
			this.container.InjectProperties(instance);
		}

		protected override void OnLaunched(LaunchActivatedEventArgs args)
		{
			string serializedCriteria = null;
			if (!string.IsNullOrEmpty(args.Arguments))
			{
				int pollId;
				if (int.TryParse(args.Arguments, out pollId))
				{
					if (args.PreviousExecutionState == ApplicationExecutionState.Running)
					{
						var criteria = new ViewPollPageNavigationCriteria
						{
							PollId = pollId
						};
						serializedCriteria = Serializer.Serialize(criteria);
					}
					else
					{
						var criteria = new LandingPageNavigationCriteria
						{
							PollId = pollId
						};
						serializedCriteria = Serializer.Serialize(criteria);
					}
				}
			}

			if (args != null && args.PreviousExecutionState == ApplicationExecutionState.Running)
			{
				this.NavigationService.NavigateToViewModel<ViewPollPageViewModel>(serializedCriteria);
			}
			else
			{
				this.DisplayRootView<LandingPage>(serializedCriteria);
			}
		}

		protected override void OnActivated(IActivatedEventArgs args)
		{
			base.OnActivated(args);

			string serializedCriteria = null;
			if (args != null && args.Kind == ActivationKind.Protocol)
			{
				var protocolArgs = args as ProtocolActivatedEventArgs;
				if (protocolArgs.Uri.Authority.ToLower() == "poll")
				{
					int pollId;
					if (int.TryParse(protocolArgs.Uri.LocalPath.Substring(1), out pollId))
					{
						if (args.PreviousExecutionState == ApplicationExecutionState.Running)
						{
							var criteria = new ViewPollPageNavigationCriteria
							{
								PollId = pollId
							};
							serializedCriteria = Serializer.Serialize(criteria);
						}
						else
						{
							var criteria = new LandingPageNavigationCriteria
							{
								PollId = pollId
							};
							serializedCriteria = Serializer.Serialize(criteria);
						}
					}
				}
			}

			if (args != null && args.PreviousExecutionState == ApplicationExecutionState.Running)
			{
				this.NavigationService.NavigateToViewModel<ViewPollPageViewModel>(serializedCriteria);
			}
			else
			{
				this.DisplayRootView<LandingPage>(serializedCriteria);
			}
		}

		protected override IEnumerable<Assembly> SelectAssemblies()
		{
			return new[] { typeof(LandingPageViewModel).GetTypeInfo().Assembly };
		}

		protected override void OnSearchActivated(SearchActivatedEventArgs args)
		{
			base.OnSearchActivated(args);
			if (args != null && args.Kind == ActivationKind.Search)
			{
				var searchArgs = args as SearchActivatedEventArgs;

				string serializedCriteria = null;
				if (args.PreviousExecutionState == ApplicationExecutionState.Running)
				{
					var criteria = new PollsPageSearchNavigationCriteria
											 {
												 SearchQuery = searchArgs.QueryText
											 };
					serializedCriteria = Serializer.Serialize(criteria);
					NavigationService.NavigateToViewModel<PollsPageViewModel>(serializedCriteria);
				}
				else
				{
					var criteria = new LandingPageNavigationCriteria
					{

					};
					serializedCriteria = Serializer.Serialize(criteria);
					DisplayRootView<LandingPage>(serializedCriteria);
				}
			}
		}

		private async void OpenPrivacyPolicy(IUICommand command)
		{
			Uri uri = new Uri("http://myvote.azurewebsites.net/Privacy");
			await Launcher.LaunchUriAsync(uri);
		}
	}
}
