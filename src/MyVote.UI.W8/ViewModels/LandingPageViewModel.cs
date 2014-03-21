using Caliburn.Micro;
using Csla;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Core;
using MyVote.UI.Helpers;
using MyVote.UI.NavigationCriteria;
using MyVote.UI.Services;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MyVote.UI.ViewModels
{
	public sealed class LandingPageViewModel : PageViewModelBase
	{
		private readonly IMessageBox messageBox;
		private readonly IMobileService mobileService;
		private readonly IObjectFactory<IUserIdentity> objectFactory;
		private readonly IAppSettings appSettings;

		public LandingPageViewModel(
			INavigation navigation,
			IMessageBox messageBox,
			IMobileService mobileService,
			IObjectFactory<IUserIdentity> objectFactory,
			IAppSettings appSettings)
			: base(navigation)
		{
			this.messageBox = messageBox;
			this.mobileService = mobileService;
			this.objectFactory = objectFactory;
			this.appSettings = appSettings;
		}

		protected override void OnInitialize()
		{
			string profileId;
			if (this.appSettings.TryGetValue<string>(SettingsKeys.ProfileId, out profileId))
			{
				var task = LoadIdentityAndGo(profileId);
				var awaiter = task.GetAwaiter();
				awaiter.OnCompleted(() =>
					{
						if (task.Exception != null && task.Exception.InnerExceptions.Any(e => e.GetType() == typeof(DataPortalException)))
						{
#if WINDOWS_PHONE
							this.messageBox.Show("There was an error loading your profile.", "Error");
#else
							this.messageBox.ShowAsync("There was an error loading your profile.", "Error");
#endif // WINDOWS_PHONE
						}
					});
			}
		}

		public async Task SignInWithTwitter()
		{
			await AuthenticateAndGo(AuthenticationProvider.Twitter);
		}

		public async Task SignInWithFacebook()
		{
			await AuthenticateAndGo(AuthenticationProvider.Facebook);
		}

		public async Task SignInWithMicrosoft()
		{
			await AuthenticateAndGo(AuthenticationProvider.Microsoft);
		}

		public async Task SignInWithGoogle()
		{
			await AuthenticateAndGo(AuthenticationProvider.Google);
		}

		protected override void DeserializeParameter(string parameter)
		{
			this.NavigationCriteria = Serializer.Deserialize<LandingPageNavigationCriteria>(parameter);
		}

		private async Task AuthenticateAndGo(AuthenticationProvider provider)
		{
			var result = await Authenticate(provider);

			if (result.Error != null)
			{
				if (result.Error.HResult != -2146233079) // Cancelled by user
				{
#if WINDOWS_PHONE
					this.messageBox.Show("There was an error authenticating.", "Error");
#else
					await this.messageBox.ShowAsync("Error authenticating.", "Error");
#endif // WINDOWS_PHONE
				}
			}
			else
			{
				this.appSettings.Add(SettingsKeys.ProfileId, result.UserId);

				await LoadIdentityAndGo(result.UserId);
			}
		}

		private async Task LoadIdentityAndGo(string profileId)
		{
			IUserIdentity identity = null;
			IsBusy = true;
			try
			{
				identity = await this.objectFactory.FetchAsync(profileId);

				var principal = new CslaPrincipalCore(identity);
				Csla.ApplicationContext.User = principal;
			}
			catch (DataPortalException ex)
			{
				Debug.WriteLine(ex.Message);
				identity = null;
			}
			IsBusy = false;

			if (identity == null)
			{
#if WINDOWS_PHONE
				this.messageBox.Show("There was an error retrieving your profile.", "Error");
#else
				await this.messageBox.ShowAsync("There was an error retrieving your profile.", "Error");
#endif // WINDOWS_PHONE
			}
			else if (identity.IsAuthenticated)
			{
				// If there is a PollId, the user is coming in from a URI.
				// Navigate to the Polls page, which adds it to the back stack.
				// Then, navigate to the View Poll page. This allows the user to be able
				// to back out of the View Poll page and land on the Polls page, rather than
				// leaving the app.
				if (this.NavigationCriteria != null && this.NavigationCriteria.PollId.HasValue)
				{
					var criteria = new ViewPollPageNavigationCriteria
					{
						PollId = this.NavigationCriteria.PollId.Value
					};

					this.Navigation.NavigateToViewModel<PollsPageViewModel>();
					this.Navigation.NavigateToViewModel<ViewPollPageViewModel>(criteria);
				}

                else if (this.NavigationCriteria != null && !string.IsNullOrEmpty(this.NavigationCriteria.SearchQuery))
                {
                    var criteria = new PollsPageSearchNavigationCriteria
                    {
                        SearchQuery = this.NavigationCriteria.SearchQuery
                    };

                    this.Navigation.NavigateToViewModel<PollsPageViewModel>();
					this.Navigation.NavigateToViewModel<ViewPollPageViewModel>(criteria);
                }
                else
				{
					this.Navigation.NavigateToViewModel<PollsPageViewModel>();
				}
			}
			else
			{
				var criteria = new RegistrationPageNavigationCriteria
				{
					ProfileId = profileId
				};

				if (this.NavigationCriteria != null)
				{
					criteria.PollId = this.NavigationCriteria.PollId;
				}

				this.Navigation.NavigateToViewModel<RegistrationPageViewModel>(criteria);
			}
		}

		private async Task<AuthenticationResult> Authenticate(AuthenticationProvider provider)
		{
			var result = new AuthenticationResult();

			try
			{
				result.UserId = await this.mobileService.AuthenticateAsync(provider);
			}
			catch (InvalidOperationException ex)
			{
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
