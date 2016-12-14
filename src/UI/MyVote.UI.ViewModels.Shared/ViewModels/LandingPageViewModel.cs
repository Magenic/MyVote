using Csla;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Core;
using MyVote.UI.Helpers;
using MyVote.UI.NavigationCriteria;
using MyVote.UI.Services;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using MvvmCross.Core.ViewModels;

namespace MyVote.UI.ViewModels
{
	public sealed class LandingPageViewModel : ViewModelBase<LandingPageNavigationCriteria>
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
			ILogger logger)
		{
			this.messageBox = messageBox;
			this.mobileService = mobileService;
			this.objectFactory = objectFactory;
			this.appSettings = appSettings;
			this.logger = logger;
		}

		public override void RealInit(LandingPageNavigationCriteria parameter)
		{
			NavigationCriteria = parameter;
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
				messageBox.ShowAsync("There was an error loading your profile.", "Error");
			}
		}

		public ICommand SignInWithTwitter
		{
			get
			{
				return new MvxCommand(async () =>
						 await AuthenticateAndGo(AuthenticationProvider.Twitter));
			}
		}

		public ICommand SignInWithFacebook
		{
			get
			{
				return new MvxCommand(async () =>
						 await AuthenticateAndGo(AuthenticationProvider.Facebook));
			}
		}

		public ICommand SignInWithMicrosoft
		{
			get
			{
				return new MvxCommand(async () =>
						 await AuthenticateAndGo(AuthenticationProvider.Microsoft));
			}
		}

		public ICommand SignInWithGoogle
		{
			get
			{
				return new MvxCommand(async () =>
						 await AuthenticateAndGo(AuthenticationProvider.Google));
			}
		}

		private async Task AuthenticateAndGo(AuthenticationProvider provider)
		{
			var result = await Authenticate(provider);

			if (result.Error != null)
			{
				if (result.Error.HResult != -2146233079) // Cancelled by user
				{
					await messageBox.ShowAsync("Error authenticating.", "Error");
				}
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
            }

			IsBusy = false;

			if (identity == null)
			{
				await messageBox.ShowAsync("There was an error retrieving your profile.", "Error");
			}
			else if (identity.IsAuthenticated)
			{
#if MOBILE
				Xamarin.Insights.Identify(profileId, Xamarin.Insights.Traits.Name, identity.UserName);
				Xamarin.Forms.MessagingCenter.Subscribe<VmPageMappings>(this, string.Format(Constants.Navigation.PageNavigated, typeof(PollsPageViewModel)), (sender) =>
				{
					Xamarin.Forms.MessagingCenter.Unsubscribe<VmPageMappings>(this, string.Format(Constants.Navigation.PageNavigated, typeof(PollsPageViewModel)));
					//this.Close(this);
					ChangePresentation(new ClearBackstackHint());
				});
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

					ShowViewModel<PollsPageViewModel>();
#if MOBILE
                    ChangePresentation(new ClearBackstackHint());
#endif
                    ShowViewModel<ViewPollPageViewModel>(criteria);
#if !MOBILE
					this.Close(this);
#endif
				}

				else if (NavigationCriteria != null && !string.IsNullOrEmpty(NavigationCriteria.SearchQuery))
				{
					var criteria = new PollsPageSearchNavigationCriteria
					{
						SearchQuery = NavigationCriteria.SearchQuery
					};

					ShowViewModel<PollsPageViewModel>();
#if MOBILE
                    ChangePresentation(new ClearBackstackHint());
#endif
                    ShowViewModel<ViewPollPageViewModel>(criteria);
#if !MOBILE
					this.Close(this);
#endif
				}
				else
				{
					ShowViewModel<PollsPageViewModel>();
#if MOBILE
                    ChangePresentation(new ClearBackstackHint());
#endif
#if !MOBILE
					this.Close(this);
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

				ShowViewModel<RegistrationPageViewModel>(criteria);
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
