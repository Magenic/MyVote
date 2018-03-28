using Csla;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Core;
using MyVote.UI.Helpers;
using MyVote.UI.NavigationCriteria;
using MyVote.UI.Services;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using MyVote.UI.Contracts;

namespace MyVote.UI.ViewModels
{
	public sealed class LandingPageViewModel : NavigatingViewModelBase
	{
		private readonly IMessageBox messageBox;
		private readonly IMobileService mobileService;
		private readonly IObjectFactory<IUserIdentity> objectFactory;
		private readonly IAppSettings appSettings;
		private readonly ILogger logger;

		public LandingPageViewModel(
			IMessageBox messageBox,
			IMobileService mobileService,
			IObjectFactory<IUserIdentity> objectFactory,
			IAppSettings appSettings,
			ILogger logger,
            INavigationService navigationService) : base(navigationService)
		{
			this.messageBox = messageBox;
			this.mobileService = mobileService;
			this.objectFactory = objectFactory;
			this.appSettings = appSettings;
			this.logger = logger;
		}

		public override void Init(object parameter)
		{
			NavigationCriteria = (LandingPageNavigationCriteria)parameter;
		}

		public async override void Start()
		{
            base.Start();
			string profileId;
			if (appSettings.TryGetValue(SettingsKeys.ProfileId, out profileId))
			{
				// Start an exit.
				await StartLoadProfileAsync(profileId);
			}
		}

		public async Task StartLoadProfileAsync(string profileId)
		{
			try
			{
				await LoadIdentityAndGo(profileId);
			}
			catch (Exception ex)
			{
				this.logger.Log(ex);
				this.IsBusy = false;
				await messageBox.ShowAsync("There was an error loading your profile.", "Error");
			}
		}

		public ICommand SignInWithTwitter
		{
			get
			{
				return new Command(async () =>
						 await AuthenticateAndGoAsync(AuthenticationProvider.Twitter));
			}
		}

		public ICommand SignInWithFacebook
		{
			get
			{
				return new Command(async () =>
						 await AuthenticateAndGoAsync(AuthenticationProvider.Facebook));
			}
		}

		public ICommand SignInWithMicrosoft
		{
			get
			{
				return new Command(async () =>
						 await AuthenticateAndGoAsync(AuthenticationProvider.Microsoft));
			}
		}

		public ICommand SignInWithGoogle
		{
			get
			{
				return new Command(async () =>
						 await AuthenticateAndGoAsync(AuthenticationProvider.Google));
			}
		}

		private async Task AuthenticateAndGoAsync(AuthenticationProvider provider)
		{
			var result = await Authenticate(provider);

			if (result.Error != null)
			{
				if (result.Error.HResult != -2146233079) // Cancelled by user
				{
					await messageBox.ShowAsync("Error authenticating.", "Error");
				}
                await messageBox.ShowAsync("Unexpected error.", result.Error.Message);
			}
			else
			{
				appSettings.Add(SettingsKeys.ProfileId, result.UserId);

				await LoadIdentityAndGo(result.UserId);
			}
		}

		private async Task LoadIdentityAndGo(string profileId)
		{
			IUserIdentity identity = null;
			IsBusy = true;
            try
            {
                identity = await objectFactory.FetchAsync(profileId);

                var principal = new CslaPrincipalCore(identity);
                ApplicationContext.User = principal;
            }
            catch (DataPortalException ex)
            {
                logger.Log(ex);
                identity = null;
                await messageBox.ShowAsync("DataPortal error.", ex.Message);
            }
            catch (Exception e)
            {
                var a = e;
            }

			IsBusy = false;

			if (identity == null)
			{
				await messageBox.ShowAsync("There was an error retrieving your profile.", "Error");
			}
			else if (identity.IsAuthenticated)
			{
#if MOBILE
				//TODO:Change logging 
                Xamarin.Insights.Identify(profileId, Xamarin.Insights.Traits.Name, identity.UserName);
#endif // MOBILE

				// If there is a PollId, the user is coming in from a URI.
				// Navigate to the Polls page, which adds it to the back stack.
				// Then, navigate to the View Poll page. This allows the user to be able
				// to back out of the View Poll page and land on the Polls page, rather than
				// leaving the app.
				if (NavigationCriteria != null && NavigationCriteria.PollId.HasValue)
				{
					var criteria = new ViewPollPageNavigationCriteria
					{
						PollId = NavigationCriteria.PollId.Value
					};

					navigationService.ShowViewModel<PollsPageViewModel>();
#if MOBILE
                    navigationService.ChangePresentation(new ClearBackstackHint());
#endif
                    navigationService.ShowViewModel<ViewPollPageViewModel>(criteria);
#if !MOBILE
					//this.Close(this);
#endif
				}

				else if (NavigationCriteria != null && !string.IsNullOrEmpty(NavigationCriteria.SearchQuery))
				{
					var criteria = new PollsPageSearchNavigationCriteria
					{
						SearchQuery = NavigationCriteria.SearchQuery
					};

					navigationService.ShowViewModel<PollsPageViewModel>();
#if MOBILE
                    navigationService.ChangePresentation(new ClearBackstackHint());
#endif
                    navigationService.ShowViewModel<ViewPollPageViewModel>(criteria);
#if !MOBILE
					//this.Close(this);
#endif
				}
				else
				{
					navigationService.ShowViewModel<PollsPageViewModel>();
#if MOBILE
                    navigationService.ChangePresentation(new ClearBackstackHint());
#endif
#if !MOBILE
					//this.Close(this);
#endif
                }
            }
			else
			{
				var criteria = new RegistrationPageNavigationCriteria
				{
					ProfileId = profileId
				};

				if (NavigationCriteria != null)
				{
					criteria.PollId = NavigationCriteria.PollId;
				}

				navigationService.ShowViewModel<RegistrationPageViewModel>(criteria);
			}
		}

		private async Task<AuthenticationResult> Authenticate(AuthenticationProvider provider)
		{
			var result = new AuthenticationResult();

			try
			{
				result.UserId = await mobileService.AuthenticateAsync(provider);
			}
			catch (InvalidOperationException ex)
			{
				logger.Log(ex);
				result.Error = ex;
			}

			return result;
		}

		private LandingPageNavigationCriteria NavigationCriteria { get; set; }

		private class AuthenticationResult
		{
			public string UserId { get; set; }
			public Exception Error { get; set; }
		}
	}
}
